using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using OpenRPG.Entities;

namespace OpenRPG.Game
{
    public class World
    {
        /// <summary>
        /// The database context.
        /// </summary>
        private readonly Context _context;

        private readonly IGuild _guild;

        /// <summary>
        /// The players in this world. The id is the key.
        /// </summary>
        public ConcurrentDictionary<ulong, Player> Players = new ConcurrentDictionary<ulong, Player>();

        public World(Context context, IGuild guild)
        {
            _context = context;
            _guild = guild;
            Load();
        }

        /// <summary>
        /// Get the player.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Player GetPlayer(IUser user)
        {
            Player player;
            Players.TryGetValue(user.Id, out player);
            return player;
        }

        /// <summary>
        /// Load the world.
        /// </summary>
        private void Load()
        {
            var players = _context.Players
                .Where(player => player.GuildId == _guild.Id)
                .ToList();

            foreach (var player in players)
            {
                player.User = _guild.GetUserAsync(player.UserId).Result;
                Players.AddOrUpdate(player.UserId, player, (k, v) => player);
            }
        }

        /// <summary>
        /// Register a new user to the world.
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns></returns>
        public async Task<bool> Register(IUser user)
        {
            var player = new Player
            {
                UserId = user.Id,
                GuildId = _guild.Id,
                Attack = 10,
                Defend = 10,
                MaxHealth = 100,
                Health = 100
            };

            if (!Players.TryAdd(player.UserId, player)) return false;
            player.User = await _guild.GetUserAsync(player.UserId);
            await _context.Players.AddAsync(player);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}