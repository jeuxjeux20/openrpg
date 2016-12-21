using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace OpenRPG.Attributes
{
    public class ParameterMustNotBeItselfAttribute : ParameterPreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(CommandContext context, ParameterInfo parameter, object value, IDependencyMap map)
        {
            var user = value as IUser;
            return Task.FromResult(user == null || user.Id == context.User.Id ? PreconditionResult.FromError("You cannot select yourself.") : PreconditionResult.FromSuccess());
        }
    }
}