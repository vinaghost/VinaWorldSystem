using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Marker;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IWebApi>());

builder.Services
    .AddFastEndpoints()
    .SwaggerDocument();

// Configure EF Core with MySQL
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

builder.Services.AddAuthorization();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "redis:6379"; // redis is the container name of the redis service. 6379 is the default port
    options.InstanceName = "SampleInstance";
});
var app = builder.Build();

app.UseFastEndpoints()
   .UseSwaggerGen();

app.UseAuthorization();
app.Run();