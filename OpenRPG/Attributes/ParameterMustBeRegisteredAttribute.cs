using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace OpenRPG.Attributes
{
    public class ParameterMustBeRegisteredAttribute : ParameterPreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(CommandContext context, ParameterInfo parameter, object value, IDependencyMap map)
        {
            var user = map.Get<Bot>().PlayerManager.GetPlayer(value as IUser);
            return Task.FromResult(user == null ? PreconditionResult.FromError("The user is not registered.") : PreconditionResult.FromSuccess());
        }
    }
}