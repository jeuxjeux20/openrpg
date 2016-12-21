using System.Threading.Tasks;
using Discord.Commands;

namespace OpenRPG.Attributes
{
    public class MustBeRegisteredAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(CommandContext context, CommandInfo command,
            IDependencyMap map)
        {
            var me = Bot.Instance.PlayerManager.GetPlayer(context.User);
            return Task.FromResult(me == null ? PreconditionResult.FromError("You aren't registered.") : PreconditionResult.FromSuccess());
        }
    }
}