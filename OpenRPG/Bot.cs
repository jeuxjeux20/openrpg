using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OpenRPG.Game;

namespace OpenRPG
{
    public class Bot
    {
        /// <summary>
        /// The commands service.
        /// </summary>
        private CommandService _commands;

        /// <summary>
        /// The discord client.
        /// </summary>
        private DiscordSocketClient _client;

        /// <summary>
        /// The dependency map.
        /// </summary>
        private DependencyMap _map;

        /// <summary>
        /// The player manager.
        /// </summary>
        public PlayerManager PlayerManager { get; private set; }

        /// <summary>
        /// The database context.
        /// </summary>
        public readonly Context Context;

        public Bot()
        {
            Context = new Context();
        }

        /// <summary>
        /// Start the bot.
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            await Context.Database.EnsureCreatedAsync();

            _client = new DiscordSocketClient();
            PlayerManager = new PlayerManager(Context, _client);

            await InstallCommands();
            await _client.LoginAsync(TokenType.Bot, File.ReadAllText("token.txt"));
            await _client.ConnectAsync();
            PlayerManager.Load();
            Console.WriteLine("OpenRPG Loaded.");
            await Task.Delay(-1);
        }

        /// <summary>
        /// Install all the commands.
        /// </summary>
        /// <returns></returns>
        public async Task InstallCommands()
        {
            _map = new DependencyMap();
            _map.Add(this);
            _map.Add(Context);
            _map.Add(PlayerManager);
            _commands = new CommandService();
            _client.MessageReceived += HandleCommand;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        /// <summary>
        /// Handle a command.
        /// </summary>
        /// <param name="messageParam"></param>
        /// <returns></returns>
        public async Task HandleCommand(SocketMessage messageParam)
        {
            if (messageParam.Author.IsBot) return;

            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            var isPrivateMessage = message.Channel is ISocketPrivateChannel;
            var argPos = 0;
            if (!(isPrivateMessage
                  || message.HasCharPrefix('•', ref argPos)
                  || message.HasStringPrefix("r_", ref argPos)
                  || message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
                return;

            var player = PlayerManager.GetPlayer(message.Author);
            if (player != null) player.LastChannel = message.Channel;
            var context = new CommandContext(_client, message);
            var result = await _commands.ExecuteAsync(context, argPos, _map);
            if (!result.IsSuccess && (result.Error != CommandError.UnknownCommand || isPrivateMessage))
                await message.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}