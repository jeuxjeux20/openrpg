using Microsoft.EntityFrameworkCore;
using OpenRPG.Entities;

namespace OpenRPG
{
    public class Context : DbContext
    {
        public DbSet<Player> Players { get; set; }

        public DbSet<Player> PlayerItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseMySql(@"Server=localhost;database=openrpg;uid=root;pwd=passwow;");
    }
}