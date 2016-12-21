using System.Threading.Tasks;
using Discord.Commands;

namespace OpenRPG.Attributes
{
    public class MustNotBeInBattleAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(CommandContext context, CommandInfo command,
            IDependencyMap map)
        {
            var me = map.Get<Bot>().PlayerManager.GetPlayer(context.User);
            return Task.FromResult(me?.Battle != null ? PreconditionResult.FromError("You are in battle.") : PreconditionResult.FromSuccess());
        }
    }
}