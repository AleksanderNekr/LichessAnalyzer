using System.Net.Mime;
using System.Text.Json.Serialization;
using Audit.Core;
using Audit.Http;
using Audit.Serilog.Configuration;
using Backend.DataManagement.LichessApi;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddHealthChecks();
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
                        });

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting()
   .UseEndpoints(static endpoints =>
                 {
                     endpoints.MapControllers();
                     endpoints.MapHealthChecks("/_health",
                                               new HealthCheckOptions
                                               {
                                                   ResponseWriter = WriteCheckResponse
                                               });
                 });

app.UseHttpLogging();

Log.Information("I'm alive!");

app.Run();

return;

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
            throw new ArgumentOutOfRangeException();
    }

    return UIResponseWriter.WriteHealthCheckUIResponse(httpContext, healthReport);
}
