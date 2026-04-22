using API.Features.Shared;
using API.Infrastructure.Services;
using Dapper;
using FastEndpoints;

namespace API.Domains.Processors
{
    public class ServerNameValidator : IGlobalPreProcessor
    {
        public async Task PreProcessAsync(IPreProcessorContext ctx, CancellationToken cancellationToken)
        {
            if (ctx.Request is not ServerNameRequest serverNameRequest) return;

            if (string.IsNullOrEmpty(serverNameRequest.ServerName))
            {
                ctx.ValidationFailures.Add(
                    new("ServerName", $"Server name is required!"));
                await ctx.HttpContext.Response.SendErrorsAsync(ctx.ValidationFailures, cancellation: cancellationToken);
                return;
            }

            var databaseService = ctx.HttpContext.RequestServices.GetRequiredService<DatabaseService>();
            await using var connection = await databaseService.OpenConnection("Servers");

            var serverId = await connection.QueryFirstOrDefaultAsync<int?>(
                "SELECT id FROM Servers WHERE Url = @Name",
                new { Name = serverNameRequest.ServerName });

            if (serverId is null)
            {
                ctx.ValidationFailures.Add(
                    new("ServerName", $"Server name is incorrect!"));

                await ctx.HttpContext.Response.SendErrorsAsync(ctx.ValidationFailures, cancellation: cancellationToken);
                return;
            }
        }
    }
}