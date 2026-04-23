
# Endpoint Template Instructions

**Note:** This template is for endpoints that require `ServerName` in the request (i.e., the request type should inherit from `ServerNameRequest`).


This document describes the standard template for creating a new endpoint in the API. Use the placeholders (e.g., `[name_endpoint]`) and follow the structure below for consistency.

You can use this template for endpoints that return a single object or a list. See the code snippets below for both cases. Replace `[name_endpoint]Response` with `List<[name_endpoint]Response>` in the endpoint and query if your endpoint returns a list.


## Step 1: Create Feature Folder
Create a new folder under `API/Features` named `[name_endpoint]` to contain all files for the new endpoint:

```
API/Features/[name_endpoint]/
```

## Step 2: Files to Create
1. `[name_endpoint]Endpoint.cs` (Endpoint class)
2. `[name_endpoint]Request.cs` (Request record)
3. `[name_endpoint]Response.cs` (Response record)
4. `[name_endpoint]Query.cs` (Query handler)

---


## 1. `[name_endpoint]Endpoint.cs`

### For Single Response
```csharp
using API.Domains.EndpointGroups;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Features.[name_endpoint]
{
    public class [name_endpoint]Endpoint([name_endpoint]Query.Handler handler) : Endpoint<
        [name_endpoint]Request,
        Results<Ok<[name_endpoint]Response>, NotFound>>
    {
        public override void Configure()
        {
            Get("/[name_endpoint]/path");
            AllowAnonymous();
            Group<ServerGroup>();
        }

        public override async Task<Results<Ok<[name_endpoint]Response>, NotFound>> ExecuteAsync(
            [name_endpoint]Request request, CancellationToken cancellationToken)
        {
            var response = await handler.HandleAsync(new(request.ServerName, /* other params */), cancellationToken);
            if (response is null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(new [name_endpoint]Response(/* map response fields */));
        }
    }
}
```

### For List Response
```csharp
using API.Domains.EndpointGroups;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Features.[name_endpoint]
{
    public class [name_endpoint]Endpoint([name_endpoint]Query.Handler handler) : Endpoint<
        [name_endpoint]Request,
        Results<Ok<List<[name_endpoint]Response>>, NotFound>>
    {
        public override void Configure()
        {
            Get("/[name_endpoint]/path");
            AllowAnonymous();
            Group<ServerGroup>();
        }

        public override async Task<Results<Ok<List<[name_endpoint]Response>>, NotFound>> ExecuteAsync(
            [name_endpoint]Request request, CancellationToken cancellationToken)
        {
            var response = await handler.HandleAsync(new(request.ServerName, /* other params */), cancellationToken);
            if (response is null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(response.Select(r => new [name_endpoint]Response(/* map fields */)).ToList());
        }
    }
}
```

## 2. `[name_endpoint]Request.cs`
```csharp
using API.Features.Shared;

namespace API.Features.[name_endpoint]
{
    public record [name_endpoint]Request(string ServerName, /* other params */) : ServerNameRequest(ServerName);
}
```

## 3. `[name_endpoint]Response.cs`
```csharp
namespace API.Features.[name_endpoint]
{
    public record [name_endpoint]Response
    {
        // Define response properties here
    }
}
```


## 4. `[name_endpoint]Query.cs`

### For Single Response
```csharp
using API.Infrastructure.Services;
using Dapper;
using Immediate.Handlers.Shared;

namespace API.Features.[name_endpoint]
{
    [Handler]
    public static partial class [name_endpoint]Query
    {
        public sealed record Query(string ServerName, /* other params */);
        public record Response
        {
            // Define response properties here
        }

        private static async ValueTask<Response?> HandleAsync(
            Query query,
            DatabaseService databaseService,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await using var connection = await databaseService.OpenConnection(query.ServerName);
            // Implement query logic here
            return null;
        }
    }
}
```

### For List Response
```csharp
using API.Infrastructure.Services;
using Dapper;
using Immediate.Handlers.Shared;

namespace API.Features.[name_endpoint]
{
    [Handler]
    public static partial class [name_endpoint]Query
    {
        public sealed record Query(string ServerName, /* other params */);
        public record Response
        {
            // Define response properties here
        }

        private static async ValueTask<List<Response>> HandleAsync(
            Query query,
            DatabaseService databaseService,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await using var connection = await databaseService.OpenConnection(query.ServerName);
            // Implement query logic here
            return new List<Response>();
        }
    }
}
```

---

**Note:** Replace `[name_endpoint]` and placeholders with your actual endpoint name and parameters.
