using WebApi.Features.Shared.Cache;

namespace WebApi.Features.Servers
{
    public partial class GetServerById
    {
        public record Request(int Id) : ICachedQuery<string>
        {
            public string Key => $"{nameof(GetServerById)}_{Id}";
        }
    }
}