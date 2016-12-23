using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using OpenRPG.Attributes;
using OpenRPG.Game;

namespace OpenRPG.Modules
{
    public class ModuleBattle : ModuleBase
    {
        private readonly Context _context;

        private readonly PlayerManager _playerManager;

        public ModuleBattle(PlayerManager playerManager, Context context)
        {
            _playerManager = playerManager;
            _context = context;
        }

        /// <summary>
        /// Register to the world.
        /// </summary>
        /// <returns></returns>
        [Command("attack"), Alias("a")]
        [MustBeRegistered, MustBeInBattle]
        public async Task Attack()
        {
            var player = _playerManager.GetPlayer(Context.User);
            var battle = player.Battle;
            await battle.SetAction(player, BattleAction.Attack);
        }

        /// <summary>
        /// Register to the world.
        /// </summary>
        /// <returns></returns>
        [Command("board"), Alias("b")]
        [MustBeRegistered, MustBeInBattle]
        public async Task Board()
        {
            var player = _playerManager.GetPlayer(Context.User);
            var battle = player.Battle;
            await ReplyAsync(battle.GetList(player));
        }

        /// <summary>
        /// Register to the world.
        /// </summary>
        /// <returns></returns>
        [Command("select"), Alias("s")]
        [MustBeRegistered, MustBeInBattle]
        public async Task Select(int id)
        {
            var player = _playerManager.GetPlayer(Context.User);
            var battle = player.Battle;
            var targets = battle.GetTargets(player);

            if (id > 0 && id <= targets.Count)
            {
                var target = targets[id - 1];
                if (target.Health > 0)
                {
                    battle.SetTarget(player, id - 1);
                    await ReplyAsync($"Selected **{target.Name}** as target.");
                }
                else
                {
                    await ReplyAsync($"Could not set **{target.Name}** as target; he is already defeated!");
                }
            }
            else
            {
                await ReplyAsync("Out of range.");
            }
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
            var battle = new Battle(_context, new [] {player}, new []
            {
                new Npc
                {
                    Name = "Man",
                    Attack = 5,
                    Defend = 5,
                    Health = 20,
                    MaxHealth = 20,
                    Speed = 1
                }
            }) {Leaveable = true};
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
        [Command("battle"), Alias("fight")]
        [MustBeRegistered, MustNotBeInBattle]
        public async Task Battle(
            [Summary("The user to battle"), ParameterMustBeRegistered, ParameterMustNotBeItself] IUser user
        )
        {
            var attacker = _playerManager.GetPlayer(Context.User);
            var target = _playerManager.GetPlayer(user);

            var battle = new Battle(_context, attacker, target);
            await battle.Start();
        }
    }
}