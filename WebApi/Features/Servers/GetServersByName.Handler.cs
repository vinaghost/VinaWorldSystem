using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Features.Servers.Shared;

namespace WebApi.Features.Servers
{
    public partial class GetServersByName
    {
        public class Handler(AppDbContext context) : IRequestHandler<Request, Response>
        {
            private readonly AppDbContext _context = context;

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var servers = await _context.Servers
                    .Where(x => x.Url.Contains(request.SearchTerm))
                    .Select(x => new ServerDto(x.Id, x.Url))
                    .ToListAsync(cancellationToken);
                return new Response(servers);
            }
        }
    }
}