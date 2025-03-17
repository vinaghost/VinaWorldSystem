namespace WebApi.Features.Oasises
{
    public partial class GetOasises
    {
        public record Oasis(int X, int Y, string Type, double Distance);
    }
}