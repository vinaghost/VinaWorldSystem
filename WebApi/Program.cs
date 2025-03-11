using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using WebApi.Context;
using WebApi.Features.Authentication;
using WebApi.Marker;

var builder = WebApplication.CreateBuilder(args);

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
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

var scopes = new[]
{
    "read:servers",
    "read:tiles",
    "write:servers",
    "write:tiles",
};
builder.Services
    .AddAuthorization(options =>
    {
        foreach (var scope in scopes)
        {
            options.AddPolicy(scope, policy => policy.Requirements.Add(new HasScopeRequirement(scope, authority)));
        }
    });
builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IWebApi>());

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
var app = builder.Build();

app.UseAuthentication()
    .UseAuthorization();

app.UseFastEndpoints()
   .UseSwaggerGen();

app.Run();