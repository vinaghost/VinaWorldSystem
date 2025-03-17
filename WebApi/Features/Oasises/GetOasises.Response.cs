namespace WebApi.Features.Oasises
{
    public partial class GetOasises
    {
        public record Response(List<Oasis> Oasises);
    }
}