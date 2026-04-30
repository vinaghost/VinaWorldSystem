using API.Groups.Server;
using FastEndpoints;

namespace API.Groups.Player
{
    public class PlayerGroup : SubGroup<ServerGroup>
    {
        public PlayerGroup()
        {
            Configure("players",
                ep =>
                {
                    ep.Description(x => x
                        .WithDescription("Players related")
                        .WithTags("Players"));
                });
        }
    }
}