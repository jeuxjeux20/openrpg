using System.Threading.Tasks;
using OpenRPG.Interfaces;

namespace OpenRPG.Game
{
    public class Npc : INpc
    {
        public string Name { get; set; }

        public int Health { get; set; }

        public int MaxHealth { get; set; }

        public int Attack { get; set; }

        public int Defend { get; set; }

        public Battle Battle { get; set; }

        public async Task ProgressBattle(Battle battle)
        {
            // This is a stupid NPC. It only knows how to attack.
            // In the future we want the NPC to heal etc.
            await battle.Attack();
        }
    }
}