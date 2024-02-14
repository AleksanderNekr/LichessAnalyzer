using Audit.Core;
using Audit.Http;
using Audit.Serilog.Configuration;
using Microsoft.AspNetCore.HttpLogging;
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

builder.Services.AddHttpLogging(static options =>
                                {
                                    options.LoggingFields = HttpLoggingFields.RequestMethod | HttpLoggingFields.Response;
                                    options.MediaTypeOptions.AddText("application/json");
                                });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/hello", () => "Hello World!");

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
