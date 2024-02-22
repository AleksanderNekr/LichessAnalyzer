using System.Net.Mime;
using System.Text.Json.Serialization;
using Audit.Core;
using Audit.Http;
using Audit.Serilog.Configuration;
using Backend;
using Backend.Auth;
using Backend.DataManagement.Analytics;
using Backend.DataManagement.LichessApi;
using Backend.DataManagement.Users;
using Backend.DataManagement.Users.Services;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("UserDataSource")!;
builder.Services.AddDbContext<UsersContext>(optionsBuilder =>
                                            {
                                                optionsBuilder.UseNpgsql(connectionString,
                                                                         static optionsBuilder =>
                                                                             optionsBuilder.EnableRetryOnFailure());
                                                optionsBuilder.LogTo(Log.Debug,
                                                                     new[] { DbLoggerCategory.Database.Command.Name },
                                                                     LogLevel.Information);
                                                optionsBuilder.EnableSensitiveDataLogging();
                                            });

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration)
                                      .CreateLogger();

builder.Services.AddLogging(static builder =>
                            {
                                builder.ClearProviders();
                                builder.AddSerilog(dispose: true);
                            });

Configuration.Setup().UseSerilog(ConfigureAuditSerilog);

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddHttpLogging(static options =>
                                {
                                    options.LoggingFields = HttpLoggingFields.RequestMethod | HttpLoggingFields.Response;
                                    options.MediaTypeOptions.AddText(MediaTypeNames.Application.Json);
                                });

builder.Services
       .AddControllers()
       .AddJsonOptions(static options =>
                           options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<GetDataService>(
            client =>
            {
                client.BaseAddress = new Uri("https://lichess.org/api/");
            })
       .AddAuditHandler(static configurator =>
                        {
                            configurator.IncludeRequestBody()
                                        .IncludeRequestHeaders()
                                        .IncludeContentHeaders()
                                        .IncludeResponseHeaders()
                                        .IncludeResponseBody();
                        })
       .AddRetryPolicy();

builder.Services.AddLichessAuthentication();

builder.Services.AddTransient<UsersRepository>();
builder.Services.AddTransient<AnalyticsListsService>();
builder.Services.AddTransient<AuthService>();

IConfigurationSection redisCacheSection = builder.Configuration.GetSection("RedisSettings");
builder.Services.Configure<CacheOptions>(redisCacheSection);
builder.Services.AddScoped<IAnalyticsCacheService, RedisAnalyticsCacheService>();

builder.Services.AddHealthChecks()
       .AddNpgSql(connectionString)
       .AddRedis(redisCacheSection.GetValue<string>("RedisConnectionsString")!);

WebApplication app = builder.Build();

app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}
else
{
    app.UseReDoc(options => options.RoutePrefix = "api");
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/_health",
                    new HealthCheckOptions
                    {
                        ResponseWriter = WriteCheckResponse
                    });

app.UseHttpLogging();

await ApplyMigrationsAsync(app.Services);

Log.Information("I'm alive!");

app.Run();

return;

async Task ApplyMigrationsAsync(IServiceProvider appServices)
{
    using IServiceScope scope    = appServices.CreateScope();
    IServiceProvider    services = scope.ServiceProvider;

    var context = services.GetRequiredService<UsersContext>();
    if ((await context.Database.GetPendingMigrationsAsync()).Any())
    {
        await context.Database.MigrateAsync();
    }
}

static void ConfigureAuditSerilog(ISerilogConfigurator configurator)
{
    configurator.Message(static @event =>
                         {
                             if (@event is not AuditEventHttpClient eventHttpClient)
                             {
                                 return @event.ToJson();
                             }

                             object? contentBody = eventHttpClient.Action?.Response?.Content?.Body;
                             if (contentBody is string { Length: > 500 } body)
                             {
                                 eventHttpClient.Action!.Response!.Content!.Body = body[..500] + "<...>";
                             }

                             return @event.ToJson();
                         });
}

static Task WriteCheckResponse(HttpContext httpContext, HealthReport healthReport)
{
    switch (healthReport.Status)
    {
        case HealthStatus.Unhealthy:
            Log.Error("HealthCheck executed, result: UNHEALTHY");

            break;
        case HealthStatus.Degraded:
            Log.Warning("HealthCheck executed, result: DEGRADED");

            break;
        case HealthStatus.Healthy:
            Log.Information("HealthCheck executed, result: healthy");

            break;
        default:
            throw new ArgumentOutOfRangeException(nameof(healthReport));
    }

    return UIResponseWriter.WriteHealthCheckUIResponse(httpContext, healthReport);
}
