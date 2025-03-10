using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Features.Shared.Errors;

namespace WebApi.Features.Servers
{
    public partial class GetServerById
    {
        public class Handler(AppDbContext dbContext) : IRequestHandler<Request, Result<string>>
        {
            private readonly AppDbContext _dbContext = dbContext;

            public async Task<Result<string>> Handle(Request request, CancellationToken cancellationToken)
            {
                var serverUrl = await _dbContext.Servers
                    .Where(x => x.Id == request.Id)
                    .Select(x => x.Url)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(serverUrl))
                {
                    return Result.Fail(new ItemNotFound("Server"));
                }

                return Result.Ok(serverUrl);
            }
        }
    }
}