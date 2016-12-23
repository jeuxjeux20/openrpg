using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using OpenRPG.Attributes;
using OpenRPG.Game;

namespace OpenRPG.Modules
{
    public class ModuleGame : ModuleBase
    {
        private readonly Context _context;

        private readonly PlayerManager _playerManager;

        public ModuleGame(PlayerManager playerManager, Context context)
        {
            _playerManager = playerManager;
            _context = context;
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
        /// Reset the user.
        /// </summary>
        /// <returns></returns>
        [Command("reset")]
        [MustBeRegistered]
        public async Task Reset()
        {
            var player = _playerManager.GetPlayer(Context.User);
            player.Attack = 10;
            player.Defend = 10;
            player.Speed = 1;
            player.MaxHealth = 100;
            player.Health = 100;
            player.Money = 250;
            player.Experience = 0;
            player.Points = 0;
            _context.Players.Update(player);
            await _context.SaveChangesAsync();
            await ReplyAsync("Player has been reseted.");
        }

        /// <summary>
        /// Reset the user.
        /// </summary>
        /// <returns></returns>
        [Command("heal")]
        [MustBeRegistered]
        public async Task Heal()
        {
            var player = _playerManager.GetPlayer(Context.User);

            if (player.Health >= player.MaxHealth)
            {
                await ReplyAsync(":no_entry: You are already on full health.");
                return;
            }

            if (player.Money < 25)
            {
                await ReplyAsync(":no_entry: You don't have enough money to heal yourself.");
                return;
            }

            player.Money -= 25;
            player.Health = player.MaxHealth;
            await ReplyAsync(":pill: You bought and toke a HealMePls-pill. You're now on full HP!");

            _context.Players.Update(player);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Reset the user.
        /// </summary>
        /// <returns></returns>
        [Command("usepoints"), Alias("usepoint", "spendpoints", "spendpoint")]
        [MustBeRegistered]
        public async Task UsePoint(string skill, int amount = 1)
        {
            var player = _playerManager.GetPlayer(Context.User);
            if (player.Points < amount)
            {
                await ReplyAsync(":no_entry: You don't have enough points to spend.");
                return;
            }

            switch (skill.ToLower())
            {
                case "health":
                    player.MaxHealth += 5;
                    await ReplyAsync($":tada: You have now {player.MaxHealth} max HP!");
                    break;
                case "attack":
                    player.Attack++;
                    await ReplyAsync($":tada: You have now {player.Attack} attack points!");
                    break;
                case "defend":
                    player.Defend++;
                    await ReplyAsync($":tada: You have now {player.Defend} defend points!");
                    break;
                case "speed":
                    player.Speed++;
                    await ReplyAsync($":tada: You have now {player.Speed} speed points!");
                    break;
                default:
                    await ReplyAsync(":no_entry: You don't know this skill... Neither do we.");
                    return;
            }

            player.Points--;
            _context.Players.Update(player);
            await _context.SaveChangesAsync();
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
                    builder.Name = "Level:";
                    builder.Value = $":star: {player.Level}";
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Experience";
                    builder.Value = $":book: {player.Experience}";
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Health:";
                    builder.Value = $":heart: {player.Health} / {player.MaxHealth}";
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Points";
                    builder.Value = $":inbox_tray: {player.Points}";
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Attack points:";
                    builder.Value = $":crossed_swords: {player.Attack}";
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Defend points:";
                    builder.Value = $":shield: {player.Defend}";
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Speed points:";
                    builder.Value = $":mans_shoe: {player.Speed}";
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Money:";
                    builder.Value = $":money_with_wings: {player.Money}$";
                    builder.IsInline = true;
                }));
        }
    }
}