using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Discord;
using OpenRPG.Game;
using OpenRPG.Interfaces;
namespace OpenRPG.Entities
{
    public class Player : IAttackable
    {
        /// <summary>
        /// The unique ID of the user.
        /// </summary>
        public int PlayerId { get; set; }

        /// <summary>
        /// The ID of the user.
        /// </summary>
        public ulong UserId { get; set; }

        /// <summary>
        /// The amount of money that the player owns.
        /// </summary>
        public int Money { get; set; }

        /// <summary>
        /// The points that the user can spent on stats.
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// The max amount of health of the user.
        /// </summary>
        public int MaxHealth { get; set; }

        /// <summary>
        /// The amount of health of the user.
        /// </summary>
        public int Health { get; set; }

        /// <summary>
        /// The attack points of the user.
        /// </summary>
        public int Attack { get; set; }

        /// <summary>
        /// The defend points of the user.
        /// </summary>
        public int Defend { get; set; }

        /// <summary>
        /// The speed points of the user.
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// The speed points of the user.
        /// </summary>
        public int Experience { get; set; }

        /// <summary>
        /// The speed points of the user.
        /// </summary>
        public int Level => 1 + (int) Math.Floor(Math.Pow(Experience, 1 / 3.0));

        /// <summary>
        /// The items of the player.
        /// </summary>
        public virtual List<PlayerItem> Items { get; set; }

        /// <summary>
        /// The name of the player.
        /// </summary>
        public string Name => User?.Username ?? "Player";

        /// <summary>
        /// The Discord User.
        /// </summary>
        public IUser User;

        /// <summary>
        /// The current battle.
        /// </summary>
        [NotMapped]
        public Battle Battle { get; set; }

        /// <summary>
        /// The channel where the user was last seen.
        /// </summary>
        [NotMapped]
        public IMessageChannel LastChannel { get; set; }

        /// <summary>
        /// The channel that is private.
        /// </summary>
        [NotMapped]
        public IMessageChannel PrivateChannel { get; set; }

        /// <summary>
        /// Add experience to the player.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>The amount of levels leveled up</returns>
        public int AddExperience(int amount)
        {
            var oldLevel = Level;
            Experience += amount;
            Points += Level - oldLevel;
            return Level - oldLevel;
            
        }

        public byte CriticalRate { get; set; } = 15;
    }
}