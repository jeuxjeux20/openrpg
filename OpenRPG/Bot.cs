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
        private CommandService _commands;
        private DiscordSocketClient _client;
        private DependencyMap _map;
        public readonly WorldManager WorldManager;
        public readonly Context Context;

        public Bot()
        {
            Context = new Context();
            WorldManager = new WorldManager(Context);
        }

        /// <summary>
        /// Start the bot.
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            await Context.Database.EnsureCreatedAsync();

            _client = new DiscordSocketClient();

            await InstallCommands();
            await _client.LoginAsync(TokenType.Bot, File.ReadAllText("token.txt"));
            await _client.ConnectAsync();
            await Task.Delay(-1);
        }

        /// <summary>
        /// Install all the commands.
        /// </summary>
        /// <returns></returns>
        public async Task InstallCommands()
        {
            _map = new DependencyMap();
            _map.Add(Context);
            _map.Add(WorldManager);
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
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            var argPos = 0;
            if (!(message.HasCharPrefix('•', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
            {
                return;
            }

            var context = new CommandContext(_client, message);
            var result = await _commands.ExecuteAsync(context, argPos, _map);
            if (!result.IsSuccess)
            {
                await message.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}