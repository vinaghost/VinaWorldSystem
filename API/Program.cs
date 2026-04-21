using API;
using API.Infrastructure.Middleware;
using API.Infrastructure.Services;
using FastEndpoints;
using MySqlConnector;
using Serilog;
using Serilog.Events;
using System.Data;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddFastEndpoints();
    builder.Services.AddServiceHealthChecks();
    builder.Services.AddOpenApi();
    builder.Services.AddAPIBehaviors();
    builder.Services.AddAPIHandlers();
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // Add database connection service
    builder.Services.AddMySqlDataSource(builder.Configuration.GetConnectionString("Servers")!);
    builder.Services.AddScoped<DatabaseService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }
    app.UseFastEndpoints();
    app.UseMiddleware<CorrelationIdMiddleware>();

    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
        };

        // Exclude health check endpoints from request logs
        options.GetLevel = (httpContext, elapsed, ex) =>
        {
            if (httpContext.Request.Path.StartsWithSegments("/health"))
                return LogEventLevel.Verbose;

            return elapsed > 500 ? LogEventLevel.Warning : LogEventLevel.Information;
        };
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}