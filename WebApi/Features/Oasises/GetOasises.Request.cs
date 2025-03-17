using WebApi.Features.Shared.Cache;

namespace WebApi.Features.Oasises
{
    public partial class GetOasises
    {
        public record Request(int ServerId, int X, int Y, int Distance) : ICachedQuery<Response>
        {
            public string Key => $"{nameof(GetOasises)}_{ServerId}_{X}_{Y}_{Distance}";
        }
    }
}