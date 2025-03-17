using FluentResults;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Features.Shared.Cache;
using WebApi.Features.Shared.Models;

namespace WebApi.Features.Oasises
{
    public partial class GetOasises
    {
        public class Handler(AppDbContext context) : ICacheQueryHandler<Request, Response>
        {
            private readonly AppDbContext _context = context;

            public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
                var centerCoordinate = new Coordinates(request.X, request.Y);
                var oasises = await _context.Tiles
                    .AsExpandable()
                    .Where(t => t.ServerId == request.ServerId && t.Status == "Unoccupied")
                    .Where(t => CoordinatesExtenstion.Distance(request.X, request.Y, t.X, t.Y) <= request.Distance * request.Distance)
                    .Select(t => new Oasis(t.X, t.Y, t.Type, centerCoordinate.Distance(t.X, t.Y)))
                    .ToListAsync(cancellationToken);

                return new Response([.. oasises.OrderBy(x => x.Distance)]);
            }
        }
    }
}