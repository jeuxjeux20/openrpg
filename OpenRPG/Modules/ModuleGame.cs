using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using OpenRPG.Attributes;
using OpenRPG.Game;

namespace OpenRPG.Modules
{
    public class ModuleGame : ModuleBase
    {
        private readonly PlayerManager _playerManager;

        public ModuleGame(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        /// <summary>
        /// Help command
        /// </summary>
        /// <returns></returns>
        [Command("help")]
        public async Task Help()
        {
            await ReplyAsync(
                "`register` - Register to the world\n" +
                "`test` - Enter a test battle.\n" +
                "`attack` - Attack in the battle.\n" +
                "`leave` - Leave the battle.\n" +
                "`battle <@player>` - Battle a player.\n" +
                "`stats` - The stats of the player."
            );
        }

        /// <summary>
        /// Register to the world.
        /// </summary>
        /// <returns></returns>
        [Command("register")]
        public async Task Register()
        {
            if (await _playerManager.Register(Context.User))
                await ReplyAsync($"And so the adventure of {Context.User.Username} begon!");
            else
                await ReplyAsync("You already have an account.");
        }

        /// <summary>
        /// Get the stats for the player.
        /// </summary>
        /// <returns></returns>
        [Command("stats")]
        [MustBeRegistered]
        public async Task Stats()
        {
            var player = _playerManager.GetPlayer(Context.User);

            await ReplyAsync("", embed: new EmbedBuilder()
                .WithTitle($"Stats for {Context.User.Username}")
                .WithThumbnailUrl(Context.User.AvatarUrl)
                .AddField(builder =>
                {
                    builder.Name = "Health:";
                    builder.Value = string.Format(":heart: {0} / {1}", player.Health, player.MaxHealth);
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Points";
                    builder.Value = string.Format(":inbox_tray: {0}", player.Points);
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Attack points:";
                    builder.Value = string.Format(":crossed_swords: {0}", player.Attack);
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Defend points:";
                    builder.Value = string.Format(":shield: {0}", player.Defend);
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Speed:";
                    builder.Value = string.Format(":mans_shoe: {0}", player.Speed);
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Money:";
                    builder.Value = string.Format(":money_with_wings: {0}$", player.Money);
                    builder.IsInline = true;
                }));
        }
    }
}