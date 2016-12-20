using System;
using System.Threading.Tasks;
using Discord;
using OpenRPG.Interfaces;

namespace OpenRPG.Game
{
    public class Battle : IDisposable
    {
        public bool Leaveable;
        public readonly IAttackable Attacker;
        public readonly IAttackable Opponent;
        public IAttackable CurrentAttacker;
        private readonly Random _random;
        private readonly IMessageChannel _messageChannel;

        public Battle(IMessageChannel messageChannel, IAttackable attacker, IAttackable opponent)
        {
            _messageChannel = messageChannel;
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
            if (Attacker.Health <= 0 || Opponent.Health <= 0) return false;

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

            await _messageChannel.SendMessageAsync(message);
            return true;
        }

        /// <summary>
        /// Go to the next move.
        /// </summary>
        public async Task<bool> Next()
        {
            if (Attacker.Health <= 0 || Opponent.Health <= 0)
            {
                await _messageChannel.SendMessageAsync("Battle ended! Winner: " + GetWinner().Name);
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

        public async Task Leave()
        {
            if (Leaveable)
            {
                await _messageChannel.SendMessageAsync("You left the battle.");
                Dispose();
            }
            else
            {
                await _messageChannel.SendMessageAsync("You cannot leave this battle.");
            }
        }

        /// <summary>
        /// Dispose the battle.
        /// </summary>
        public void Dispose()
        {
            Attacker.Battle = null;
            Opponent.Battle = null;
        }
    }
}