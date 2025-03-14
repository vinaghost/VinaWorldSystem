using MediatR;
using System.Diagnostics;
using WebApi.Context;
using WebApi.Entities;

namespace WebApi.Features.Servers
{
    public static partial class AddServer
    {
        public class Handler(AppDbContext dbContext, ActivitySource activitySource) : IRequestHandler<Request, Response>
        {
            private readonly AppDbContext _dbContext = dbContext;
            private readonly ActivitySource _activitySource = activitySource;

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var server = new Server
                {
                    Url = request.Url,
                    CreatedAt = DateTime.UtcNow,
                    TileUpdateAt = DateTime.UtcNow,
                    VillageUpdateAt = DateTime.UtcNow
                };

                using (var activity = _activitySource.StartActivity("Insert new server"))
                {
                    _dbContext.Servers.Add(server);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }

                return new(server.Id);
            }
        }
    }
}