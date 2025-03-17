using FastEndpoints;
using WebApi.Features.Servers;

namespace WebApi.Features.Oasises
{
    public class OasisGroup : SubGroup<ServerGroup>
    {
        public OasisGroup()
        {
            Configure("{ServerId}/oasis", ep =>
            {
                ep.Description(x => x
                  .WithTags("oasis"));
            });
        }
    }
}