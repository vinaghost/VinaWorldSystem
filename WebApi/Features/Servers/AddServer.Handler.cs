using MediatR;
using WebApi.Context;
using WebApi.Entities;

namespace WebApi.Features.Servers
{
    public static partial class AddServer
    {
        public class Handler(AppDbContext dbContext) : IRequestHandler<Request, Response>
        {
            private readonly AppDbContext _dbContext = dbContext;

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                await _dbContext.Database.EnsureCreatedAsync(cancellationToken);
                var server = new Server
                {
                    Url = request.Url,
                    CreatedAt = DateTime.UtcNow,
                    TileUpdateAt = DateTime.UtcNow,
                    VillageUpdateAt = DateTime.UtcNow
                };

                _dbContext.Servers.Add(server);
                await _dbContext.SaveChangesAsync(cancellationToken);
                return new(server.Id);
            }
        }
    }
}