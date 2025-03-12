using WebApi.Features.Shared.Cache;

namespace WebApi.Features.Servers
{
    public partial class GetServersByName
    {
        public record Request(string SearchTerm) : ICachedQuery<Response>
        {
            public string Key => $"{nameof(GetServersByName)}_{SearchTerm}";
        }
    }
}