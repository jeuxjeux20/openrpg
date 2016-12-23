using System.Collections.Concurrent;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using OpenRPG.Entities;

namespace OpenRPG.Game
{
    public class PlayerManager
    {
        /// <summary>
        /// The database context.
        /// </summary>
        private readonly Context _context;

        /// <summary>
        /// The Discord Client.
        /// </summary>
        private readonly DiscordSocketClient _client;

        public PlayerManager(Context context, DiscordSocketClient client)
        {
            _context = context;
            _client = client;
        }

        /// <summary>
        /// The players in this world. The id is the key.
        /// </summary>
        public ConcurrentDictionary<ulong, Player> Players = new ConcurrentDictionary<ulong, Player>();

        /// <summary>
        /// Load the world.
        /// </summary>
        public void Load()
        {
            foreach (var player in _context.Players)
            {
                player.User = _client.GetUser(player.UserId);
                Players.AddOrUpdate(player.UserId, player, (k, v) => player);
            }
        }

        /// <summary>
        /// Get the player.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Player GetPlayer(IUser user)
        {
            if (user == null) return null;
            Player player;
            Players.TryGetValue(user.Id, out player);
            return player;
        }

        /// <summary>
        /// Register a new user to the world.
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns></returns>
        public async Task<bool> Register(IUser user)
        {
            if (Players.ContainsKey(user.Id)) return false;

            var player = new Player
            {
                UserId = user.Id,
                Attack = 10,
                Defend = 10,
                Speed = 1,
                MaxHealth = 100,
                Health = 100,
                Money = 250
            };

            if (!Players.TryAdd(player.UserId, player)) return false;
            player.User = _client.GetUser(player.UserId);
            await _context.Players.AddAsync(player);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}