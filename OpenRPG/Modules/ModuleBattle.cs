using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using OpenRPG.Attributes;
using OpenRPG.Game;

namespace OpenRPG.Modules
{
    public class ModuleBattle : ModuleBase
    {
        private readonly PlayerManager _playerManager;

        public ModuleBattle(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        /// <summary>
        /// Register to the world.
        /// </summary>
        /// <returns></returns>
        [Command("attack")]
        [MustBeRegistered, MustBeInBattle]
        public async Task Attack()
        {
            var player = _playerManager.GetPlayer(Context.User);
            var battle = player.Battle;

            if (battle.CurrentAttacker != player)
            {
                await ReplyAsync("It's not your turn!");
                return;
            }

            await battle.Attack();
            await battle.Next();
        }

        /// <summary>
        /// Register to the world.
        /// </summary>
        /// <returns></returns>
        [Command("test")]
        [MustBeRegistered, MustNotBeInBattle]
        public async Task Test()
        {
            var player = _playerManager.GetPlayer(Context.User);

            await ReplyAsync("You are now in a test battle.");
            var npc = new Npc
            {
                Name = "Goblin",
                Attack = 5,
                Defend = 5,
                Health = 50,
                MaxHealth = 50
            };
            var battle = new Battle(player, npc) {Leaveable = true};
            await battle.Start();
        }

        /// <summary>
        /// Register to the world.
        /// </summary>
        /// <returns></returns>
        [Command("leave")]
        [MustBeRegistered, MustBeInBattle]
        public async Task Leave()
        {
            var player = _playerManager.GetPlayer(Context.User);
            var battle = player.Battle;
            await battle.Leave(player);
        }

        /// <summary>
        /// Get the stats for the player.
        /// </summary>
        /// <returns></returns>
        [Command("battle")]
        [MustBeRegistered, MustNotBeInBattle]
        public async Task Battle(
            [Summary("The user to battle"), ParameterMustBeRegistered, ParameterMustNotBeItself] IUser user
        )
        {
            var attacker = _playerManager.GetPlayer(Context.User);
            var target = _playerManager.GetPlayer(user);

            var battle = new Battle(attacker, target);
            await battle.Start();
        }
    }
}