using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using System.Text.Json.Serialization;

namespace WebApi.Features.Authentication
{
    public class Login
    {
        public class TokenContent
        {
            [JsonPropertyName("access_token")]
            public required string AccessToken { get; set; }

            [JsonPropertyName("token_type")]
            public required string TokenType { get; set; }

            [JsonPropertyName("expires_in")]
            public required int ExpiresIn { get; set; }
        }

        public class Endpoint(IHttpClientFactory httpClientFactory, IConfiguration configuration) : Endpoint<LoginRequest, Results<Ok<TokenContent>, NotFound, BadRequest>>
        {
            private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Auth0");
            private readonly IConfiguration _configuration = configuration;

            public override void Configure()
            {
                Post("/login");
                AllowAnonymous();
            }

            public override async Task<Results<Ok<TokenContent>, NotFound, BadRequest>> ExecuteAsync(LoginRequest req, CancellationToken ct)
            {
                var clientId = _configuration["AUTH0_CLIENT_ID"]!;
                var clientSecret = _configuration["AUTH0_CLIENT_SECRET"]!;
                var audience = _configuration["AUTH0_AUDIENCE"]!;

                var parameters = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "audience", audience }
                };

                var content = new FormUrlEncodedContent(parameters);

                var response = await _httpClient.PostAsync("oauth/token", content, ct);

                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadFromJsonAsync<TokenContent>(ct);
                    return TypedResults.Ok(token);
                }
                else
                {
                    return TypedResults.BadRequest();
                }
            }
        }
    }
}