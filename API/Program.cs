using API;
using API.Infrastructure.Caching;
using API.Infrastructure.Middleware;
using API.Infrastructure.Services;
using FastEndpoints;
using FastEndpoints.Swagger;
using Immediate.Handlers.Shared;
using MySqlConnector;
using Serilog;
using Serilog.Events;

[assembly: Behaviors(
    typeof(QueryCachingBehavior<,>)
)]
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services
        .AddMemoryCache()
        .AddSingleton<CacheService>();

    builder.Services
        .AddFastEndpoints()
        .SwaggerDocument(o =>
        {
            o.ExcludeNonFastEndpoints = true;
            o.ShortSchemaNames = true;
            o.AutoTagPathSegmentIndex = 0;
            o.DocumentSettings = s =>
            {
                s.Title = "VinaWorldSystem API";
                s.Version = "v1";
            };
        })
        .AddServiceHealthChecks()
        .AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info.Title = "VinaWorldSystem API";
                document.Info.Version = "v1";
                return Task.CompletedTask;
            });
        });

    builder.Services
        .AddAPIBehaviors()
        .AddAPIHandlers();

    builder.Services
        .AddSerilog((services, lc) => lc
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

    builder.Services
        .AddMySqlDataSource(builder.Configuration.GetConnectionString("Servers")!)
        .AddScoped<DatabaseService>();

    var app = builder.Build();

    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
        };

        options.GetLevel = (httpContext, elapsed, ex) =>
        {
            if (httpContext.Request.Path.StartsWithSegments("/health"))
                return LogEventLevel.Verbose;

            return elapsed > 500 ? LogEventLevel.Warning : LogEventLevel.Information;
        };
    });

    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseFastEndpoints();

    app.MapOpenApi();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerGen();
    }

    app.UseReDoc(options =>
    {
        options.SpecUrl = "/openapi/v1.json"; // Path to your generated JSON
        options.RoutePrefix = "docs";        // UI will be at /docs
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