using System;
using System.Threading.Tasks;
using Discord.API;
using OpenRPG.Entities;
using OpenRPG.Interfaces;

namespace OpenRPG.Game
{
    public class Battle : IDisposable
    {
        public bool Active;
        public bool Leaveable;
        public readonly IAttackable Attacker;
        public readonly IAttackable Opponent;
        public IAttackable CurrentAttacker;
        private readonly Random _random;

        public Battle(IAttackable attacker, IAttackable opponent)
        {
            _random = new Random();
            Attacker = attacker;
            Opponent = opponent;
            CurrentAttacker = attacker;
        }

        /// <summary>
        /// Get the next attacker.
        /// </summary>
        /// <returns>The next attacker.</returns>
        public IAttackable GetNextAttacker()
        {
            return CurrentAttacker == Attacker ? Opponent : Attacker;
        }

        /// <summary>
        /// Get the winner of this battle.
        /// </summary>
        /// <returns></returns>
        public IAttackable GetWinner()
        {
            return Opponent.Health <= 0 ? Attacker : (Attacker.Health <= 0 ? Opponent : null);
        }

        /// <summary>
        /// Attack the target.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Attack()
        {
            if (!Active || Attacker.Health <= 0 || Opponent.Health <= 0) return false;

            string message;
            var target = GetNextAttacker();

            // Calculate the damage done by the attacker.
            var damage = CurrentAttacker.Attack / 2 + _random.Next(0, CurrentAttacker.Attack / 2);

            // Get the damage blocked by the target.
            var blocked = _random.Next(0, target.Defend / 2);
            var totalDamage = damage - blocked;

            if (totalDamage <= 0)
            {
                message = string.Format(":shield: {0} blocked the incoming damage from {1}.", target.Name,
                    CurrentAttacker.Name);
            }
            else
            {
                target.Health -= totalDamage;
                if (target.Health < 0) target.Health = 0;
                message = $":crossed_swords: {target.Name} received {damage} damage from {CurrentAttacker.Name}.";
            }
            message += $"\n:heavy_minus_sign: {Attacker.Name} - {Attacker.Health} / {Attacker.MaxHealth}";
            message += $"\n:heavy_minus_sign: {Opponent.Name} - {Opponent.Health} / {Opponent.MaxHealth}";

            await SendMessage(message);
            return true;
        }

        /// <summary>
        /// Go to the next move.
        /// </summary>
        public async Task<bool> Next()
        {
            if (!Active) return false;

            if (Attacker.Health <= 0 || Opponent.Health <= 0)
            {
                await SendMessage("Battle ended! Winner: " + GetWinner().Name);
                Dispose();
                return false;
            }

            CurrentAttacker = GetNextAttacker();

            var bot = CurrentAttacker as INpc;
            if (bot == null) return true;
            await Task.Delay(1500);
            await bot.ProgressBattle(this);

            // ReSharper disable once TailRecursiveCall
            return await Next();
        }

        /// <summary>
        /// Leave the battle.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Leave(IAttackable attackable)
        {
            if (attackable != Attacker && attackable != Opponent)
                throw new ArgumentException("The attackable is not in this battle!");

            if (!Leaveable)
            {
                var channel = (Attacker as Player)?.LastChannel;
                if (channel != null) await channel.SendMessageAsync("You cannot leave this battle.");
                return false;
            }

            await SendMessage($"{attackable.Name} left the battle.");
            Dispose();
            return true;
        }

        /// <summary>
        /// Send a message to both the attacker and opponent.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected async Task SendMessage(string message)
        {
            var attacker = Attacker as Player;
            var opponent = Opponent as Player;

            if (attacker?.LastChannel != null)
                await attacker.LastChannel.SendMessageAsync(message);

            if (opponent?.LastChannel != null && opponent.LastChannel != attacker?.LastChannel)
                await opponent.LastChannel.SendMessageAsync(message);
        }

        /// <summary>
        /// Start the battle.
        /// </summary>
        public async Task Start()
        {
            Attacker.Battle = this;
            Opponent.Battle = this;
            Active = true;
            await SendMessage($":crossed_swords: {Attacker.Name} started to attack {Opponent.Name}!");
        }

        /// <summary>
        /// Dispose the battle.
        /// </summary>
        public void Dispose()
        {
            Attacker.Battle = null;
            Opponent.Battle = null;
            Active = false;
        }
    }
}