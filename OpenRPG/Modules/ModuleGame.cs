﻿using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using OpenRPG.Game;

namespace OpenRPG.Modules
{
    public class ModuleGame : ModuleBase
    {
        private readonly Context _context;
        private readonly PlayerManager _playerManager;

        public ModuleGame(Context context, PlayerManager playerManager)
        {
            _context = context;
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
        /// Register to the world.
        /// </summary>
        /// <returns></returns>
        [Command("attack")]
        public async Task Attack()
        {
            var player = _playerManager.GetPlayer(Context.User);
            var battle = player?.Battle;

            if (battle == null)
            {
                await ReplyAsync("You are not in a battle.");
                return;
            }

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
        public async Task Test()
        {
            var player = _playerManager.GetPlayer(Context.User);

            if (player.Battle != null)
            {
                await ReplyAsync("You are already in a battle.");
                return;
            }

            await ReplyAsync("You are now in a test battle.");
            player.Battle = new Battle(Context.Channel, player, new Npc
            {
                Name = "NPC",
                Attack = 10,
                Defend = 10,
                Health = 50,
                MaxHealth = 50
            })
            {
                Leaveable = true
            };
        }

        /// <summary>
        /// Register to the world.
        /// </summary>
        /// <returns></returns>
        [Command("leave")]
        public async Task Leave()
        {
            var player = _playerManager.GetPlayer(Context.User);
            var battle = player?.Battle;

            if (battle == null)
            {
                await ReplyAsync("You are not in a battle.");
                return;
            }

            await battle.Leave();
        }

        /// <summary>
        /// Get the stats for the player.
        /// </summary>
        /// <returns></returns>
        [Command("battle")]
        public async Task Battle([Summary("The (optional) user to get stats for")] IUser user = null)
        {
            var attacker = _playerManager.GetPlayer(Context.User);
            var target = _playerManager.GetPlayer(user);

            if (attacker == null)
            {
                await ReplyAsync("You're not part of the world.");
                return;
            }

            if (target == null)
            {
                await ReplyAsync("This user isn't part of the world.");
                return;
            }

            var battle = new Battle(Context.Channel, attacker, target);
            attacker.Battle = battle;
            target.Battle = battle;
            await ReplyAsync("You are now in battle!");
        }

        /// <summary>
        /// Get the stats for the player.
        /// </summary>
        /// <returns></returns>
        [Command("stats")]
        public async Task Stats([Summary("The (optional) user to get stats for")] IUser user = null)
        {
            var player = _playerManager.GetPlayer(user ?? Context.User);

            if (player == null)
            {
                await ReplyAsync("This user isn't part of the world.");
                return;
            }

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
                    builder.Name = "Money:";
                    builder.Value = string.Format(":money_with_wings: {0}", player.Money);
                    builder.IsInline = true;
                }));
        }
    }
}