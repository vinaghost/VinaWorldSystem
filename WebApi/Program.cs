using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Security.Claims;
using WebApi.Context;
using WebApi.Features.Shared.Behaviors;
using WebApi.Marker;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
// Add services to the container.
var authority = Environment.GetEnvironmentVariable("AUTH0_AUTHORITY")!;
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authority;
        options.Audience = Environment.GetEnvironmentVariable("AUTH0_AUDIENCE");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier,
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<IWebApi>();
    cfg.AddOpenBehavior(typeof(QueryCachingPipelineBehavior<,>));
});

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

builder.Services
    .AddFastEndpoints()
    .SwaggerDocument();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = Environment.GetEnvironmentVariable("REDIS_URL");
    options.InstanceName = Environment.GetEnvironmentVariable("REDIS_INSTANCE_NAME");
});

builder.Services.AddHttpClient("Auth0", client =>
{
    client.BaseAddress = new Uri(authority);
});

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resources => resources.AddService("WebApi"))
    .WithTracing(tracing =>
    {
        tracing
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation();
        tracing
            .AddOtlpExporter();
    });
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseAuthentication()
    .UseAuthorization();

app.UseFastEndpoints()
   .UseSwaggerGen();

app.MapHealthChecks("/healthz");

app.Run();