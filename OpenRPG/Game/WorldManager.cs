using System.Collections.Concurrent;
using System.Threading.Tasks;
using Discord;

namespace OpenRPG.Game
{
    public class WorldManager
    {
        /// <summary>
        /// The loaded worlds. The key is the Guild Id.
        /// </summary>
        private readonly ConcurrentDictionary<ulong, World> _worlds = new ConcurrentDictionary<ulong, World>();

        /// <summary>
        /// The database context.
        /// </summary>
        private readonly Context _context;

        public WorldManager(Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Get the world for the guild.
        /// </summary>
        /// <param name="guild">The guild</param>
        /// <returns>The world</returns>
        public World GetForGuild(IGuild guild)
        {
            return _worlds.GetOrAdd(guild.Id, id => new World(_context, guild));
        }
    }
}