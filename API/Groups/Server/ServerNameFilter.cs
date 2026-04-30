namespace API.Groups.Server
{
    public class ServerNameFilter(ValidateServerNameQuery.Handler validateServerQuery) : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
        {
            var value = ctx.HttpContext.GetRouteData().Values;
            if (value is null || !value.ContainsKey(nameof(ServerNameRequest.ServerName)))
            {
                return Results.BadRequest($"{nameof(ServerNameRequest.ServerName)} parameter is missing in the route.");
            }

            var serverNameObj = value[nameof(ServerNameRequest.ServerName)];

            if (serverNameObj is not string serverName || string.IsNullOrEmpty(serverName))
            {
                return Results.BadRequest($"{nameof(ServerNameRequest.ServerName)} parameter is empty.");
            }

            var isValid = await validateServerQuery.HandleAsync(new(serverName));
            if (!isValid)
            {
                return Results.NotFound($"Server {serverName} is not available in database");
            }

            return await next(ctx);
        }
    }
}