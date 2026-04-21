using FastEndpoints;

namespace API.Features.WeatherForecast
{
    public class Endpoint : EndpointWithoutRequest<List<Response>>
    {
        public override void Configure()
        {
            Get("/weatherforecast");
            AllowAnonymous();
        }

        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        public override async Task<List<Response>> ExecuteAsync(CancellationToken ct)
        {
            return [.. Enumerable.Range(1, 5).Select(index => new Response
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })];
        }
    }
}