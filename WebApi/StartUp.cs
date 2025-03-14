using Microsoft.EntityFrameworkCore;
using WebApi.Context;

namespace WebApi
{
    public class StartUp(IServiceScopeFactory scopeFactory, ILogger<StartUp> logger) : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly ILogger<StartUp> _logger = logger;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            _logger.LogInformation("Ensuring database is created");
            await context.Database.EnsureCreatedAsync(cancellationToken);
            _logger.LogInformation("Setting local_infile to true");
            await context.Database.ExecuteSqlRawAsync("SET GLOBAL local_infile = true;", cancellationToken);

            var serversCount = await context.Servers.CountAsync(cancellationToken);
            _logger.LogInformation("Database has {Count} servers", serversCount);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}