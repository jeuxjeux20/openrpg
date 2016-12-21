using System.Threading.Tasks;
using Discord.Commands;

namespace OpenRPG.Attributes
{
    public class MustBeInBattleAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(CommandContext context, CommandInfo command,
            IDependencyMap map)
        {
            var me = map.Get<Bot>().PlayerManager.GetPlayer(context.User);
            return Task.FromResult(me?.Battle == null ? PreconditionResult.FromError("You aren't in battle.") : PreconditionResult.FromSuccess());
        }
    }
}