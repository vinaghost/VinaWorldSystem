using FluentResults;
using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Features.Shared.Cache;
using WebApi.Features.Shared.Errors;

namespace WebApi.Features.Tiles
{
    public partial class GetTiles
    {
        public class Handler(AppDbContext context) : ICacheQueryHandler<Request, Response>
        {
            private readonly AppDbContext _context = context;

            public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
                var server = await _context.Servers
                    .AnyAsync(x => x.Id == request.ServerId);

                if (!server)
                {
                    return new ItemNotFound("server");
                }

                var count = await _context.Tiles
                    .CountAsync(x => x.ServerId == request.ServerId, cancellationToken);

                return new Response(count);
            }
        }
    }
}