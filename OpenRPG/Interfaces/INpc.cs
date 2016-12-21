using System.Threading.Tasks;
using OpenRPG.Game;

namespace OpenRPG.Interfaces
{
    public interface INpc : IAttackable
    {
        Task ProgressBattle(Battle battle);
    }
}