using OpenRPG.Game;

namespace OpenRPG.Interfaces
{
    public interface IAttackable
    {
        /// <summary>
        /// The name of the attackable.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The amount of health of the entity.
        /// </summary>
        int Health { get; set; }

        /// <summary>
        /// The amount of health of the entity.
        /// </summary>
        int MaxHealth { get; set; }

        /// <summary>
        /// The attack points of the entity.
        /// </summary>
        int Attack { get; }

        /// <summary>
        /// The defend points of the entity.
        /// </summary>
        int Defend { get; }

        /// <summary>
        /// The speed points of the entity.
        /// </summary>
        int Speed { get; }

        /// <summary>
        /// The battle where the user is currently in.
        /// </summary>
        Battle Battle { get; set; }
    }
}