﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenRPG.Entities;
using OpenRPG.Interfaces;

namespace OpenRPG.Game
{
    public enum BattleAction
    {
        None,
        Attack
    } // No block action ?

    public class Battle : IDisposable
    {
        public bool Active;
        public bool Leaveable;
        private readonly Random _random;
        public List<IAttackable> Attackers { get; }
        public List<IAttackable> Opponents { get; }
        public Dictionary<IAttackable, BattleAction> Actions { get; }
        public Dictionary<IAttackable, int> Targets { get; }
        public Context Context { get; }
        public int Turn;

        public Battle(Context context, IAttackable attacker, IAttackable opponent)
            : this(context, new List<IAttackable>(new[] {attacker}), new List<IAttackable>(new[] {opponent}))
        {
        }

        public Battle(Context context, IEnumerable<IAttackable> attacker, IEnumerable<IAttackable> opponent)
        {
            Context = context;
            _random = new Random();
            Actions = new Dictionary<IAttackable, BattleAction>();
            Targets = new Dictionary<IAttackable, int>();
            Attackers = attacker.ToList();
            Opponents = opponent.ToList();
            foreach (var attackable in Attackables)
            {
                Actions.Add(attackable, BattleAction.None);
                Targets.Add(attackable, 0);
            }
        }

        /// <summary>
        /// All the attackables.
        /// </summary>
        public IEnumerable<IAttackable> Attackables => Attackers.Concat(Opponents).OrderByDescending(a => a.Speed);

        /// <summary>
        /// Check if everyone is ready.
        /// </summary>
        /// <returns></returns>
        public bool EveryoneReady => Attackables
            .Where(a => a.Health > 0)
            .All(a => Actions[a] != BattleAction.None);

        /// <summary>
        /// Get the winners.
        /// </summary>
        /// <returns></returns>
        public List<IAttackable> GetWinners()
        {
            return Opponents.All(a => a.Health <= 0)
                ? Attackers
                : (Attackers.All(a => a.Health <= 0) ? Opponents : null);
        }

        /// <summary>
        /// Progress the npcs.
        /// </summary>
        /// <returns></returns>
        private async Task ProgressNpcs()
        {
            var npcs = Attackables
                .Where(a => a is INpc)
                .Cast<INpc>();
            foreach (var npc in npcs) await npc.ProgressBattle(this);
        }

        /// <summary>
        /// Reset all the actions.
        /// </summary>
        private void ResetActions()
        {
            foreach (var attackable in Attackables) Actions[attackable] = BattleAction.None;
        }

        /// <summary>
        /// Set the action.
        /// </summary>
        /// <param name="attackable"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task SetAction(IAttackable attackable, BattleAction action)
        {
            var player = attackable as Player;
            List<IAttackable> targets = GetTargets(attackable);
            var target = targets.ElementAtOrDefault(Targets[attackable]) ?? targets.First();

            if (target.Health <= 0)
            {
                target = targets.First(a => a.Health > 0);
                Targets[attackable] = targets.FindIndex(a => a == target);

                if (player != null)
                {
                    await player.LastChannel.SendMessageAsync(
                        $"The current target was invalid. Auto-selected **{target.Name}**.\nPlease select your action again.");
                    return;
                }
            }

            Actions[attackable] = action;
            if (EveryoneReady) await Next();
            else if (player != null)
                await player.LastChannel.SendMessageAsync(":alarm_clock: Waiting for other players...");
        }

        /// <summary>
        /// Set the action.
        /// </summary>
        /// <param name="attackable"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public void SetTarget(IAttackable attackable, int id)
        {
            Targets[attackable] = id;
        }

        /// <summary>
        /// Calculate the damage done.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public Tuple<int,bool> CalculateDamage(IAttackable attacker, IAttackable target)
        {
            var damage = attacker.Attack / 2 + _random.Next(0, attacker.Attack / 2);
            var blocked = _random.Next(0, target.Defend / 2);
            bool isCritical = new Random(DateTime.Now.Millisecond).Next(0, 100) <= attacker.CriticalRate;
            var criticalRatio = isCritical ? 1.25 : 1;
            return new Tuple<int, bool>(Math.Max(damage - blocked, 0),isCritical);
        }

        /// <summary>
        /// Get the targets for the attackable.
        /// </summary>
        /// <param name="attacker"></param>
        /// <returns></returns>
        public List<IAttackable> GetTargets(IAttackable attacker)
        {
            return Attackers.Contains(attacker) ? Opponents : Attackers;
        }

        /// <summary>
        /// Get the team for the attackable.
        /// </summary>
        /// <param name="attacker"></param>
        /// <returns></returns>
        public List<IAttackable> GetTeam(IAttackable attacker)
        {
            return Attackers.Contains(attacker) ? Attackers : Opponents;
        }

        /// <summary>
        /// Attack action.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="target"></param>
        /// <param name="messages"></param>
        public void Attack(IAttackable attacker, IAttackable target, List<string> messages)
        {
            var damage = CalculateDamage(attacker, target);
            var criticalString = damage.Item2 ? "'s CRITICAL hit" : "";
            if (damage.Item1 <= 0)
            {
                messages.Add(
                    $":shield: **{target.Name}** ({target.Health} HP) blocked the incoming damage from **{attacker.Name + criticalString}**.");
            }
            else
            {
                target.Health -= damage.Item1;

                if (target.Health <= 0)
                {
                    target.Health = 0;
                    messages.Add(
                        $":skull_crossbones: **{target.Name}** received {damage} damage from **{attacker.Name + criticalString}** and died.");

                    var team = GetTargets(target);
                    var xp = (int) (target.Attack + target.Defend / 2.0 / team.Count);

                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var attackable in team)
                    {
                        var player = attackable as Player;
                        var levels = player?.AddExperience(xp);
                        if (levels > 0)
                        {
                            messages.Add(
                                $":tada: **{attackable.Name}** leveled up and is now level {player.Level}!");
                        }
                    }
                }
                else
                {
                    messages.Add(
                        $":crossed_swords: **{target.Name}** ({target.Health} HP) received {damage} damage from **{attacker.Name + criticalString}**.");
                }
            }
        }

        /// <summary>
        /// Go to the next move.
        /// </summary>
        public async Task Next()
        {
            if (!EveryoneReady || !Active) return;

            var messages = new List<string>();

            foreach (var attacker in Attackables)
            {
                if (attacker.Health <= 0) continue;

                var action = Actions[attacker];
                var targets = GetTargets(attacker);
                var target = targets.ElementAtOrDefault(Targets[attacker]) ?? targets.First();

                switch (action)
                {
                    case BattleAction.None:
                        continue;
                    case BattleAction.Attack:
                        Attack(attacker, target, messages);
                        break;
                    default:
                        throw new InvalidProgramException($"The action is not implemented yet. | {nameof(action)}");
                }
            }

            var winners = GetWinners();
            string mention = null;
            if (winners != null)
            {
                var winnerList = string.Join(", ", winners.Select(a => a.Name));
                messages.Add($"\n:medal: Winners: {winnerList}");
                Dispose();
            }
            else
            {
                mention = ":clock: %m Next turn. Select your action.";
            }

            await SendMessage(string.Join("\n", messages), mention);

            ResetActions();
            await ProgressNpcs();
        }

        /// <summary>
        /// Add the attackables to the list.
        /// </summary>
        /// <param name="attackables"></param>
        /// <param name="messages"></param>
        /// <param name="selected"></param>
        private static void AddToList(IReadOnlyList<IAttackable> attackables, ICollection<string> messages,
            int selected)
        {
            for (var i = 0; i < attackables.Count; i++)
            {
                var a = attackables[i];
                var currentHealth = (int) Math.Round((double) a.Health / a.MaxHealth * 15);
                var current = new string('x', currentHealth);
                var left = new string(' ', 15 - currentHealth);
                var checkbox = selected == -1 || a.Health == 0 ? "-" : (selected == i ? "x" : " ");
                // TODO: Make this clearer (lol rider/resharper will put this in blue)
                messages.Add($"[{checkbox}] {i + 1}. {a.Name,-15} {a.Health,3} [{current + left}] {a.MaxHealth,-3}");
                messages.Add($"       A:{a.Attack,-3} D:{a.Defend,-3} S:{a.Speed,-3}");
            }
        }

        /// <summary>
        /// Get the list of both the attackers and opponents.
        /// </summary>
        /// <param name="attackable"></param>
        /// <returns></returns>
        public string GetList(IAttackable attackable)
        {
            var messages = new List<string> {"== Attackers =="};

            AddToList(Attackers.ToArray(), messages, Attackers.Contains(attackable) ? -1 : Targets[attackable]);
            messages.Add(string.Empty);
            messages.Add("== Opponents ==");
            AddToList(Opponents.ToArray(), messages, Opponents.Contains(attackable) ? -1 : Targets[attackable]);

            return $"```\n{string.Join("\n", messages)}\n```";
        }

        /// <summary>
        /// Leave the battle.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Leave(IAttackable attackable)
        {
            if (!Attackables.Contains(attackable)) throw new ArgumentException("The attackable is not in this battle!"); // lol this "attackable"

            if (!Leaveable)
            {
                var channel = (attackable as Player)?.LastChannel;
                if (channel != null) await channel.SendMessageAsync("You cannot leave this battle.");
                return false;
            }

            var message = $":door: **{attackable.Name}** left the battle.";
            var team = GetTeam(attackable);

            if (team.Count == 1)
            {
                message += "\n:heavy_minus_sign: The battle has ended.";
                Dispose();
            }

            await SendMessage(message);

            if (!Active) return true;
            team.Remove(attackable);
            attackable.Battle = null;

            if (EveryoneReady) await Next();
            return true;
        }

        /// <summary>
        /// Send a message to both the attacker and opponent.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="mention"></param>
        /// <returns></returns>
        protected async Task SendMessage(string message, string mention = null)
        {
            var groups = Attackables
                .Where(a => a is Player)
                .Cast<Player>()
                .Where(p => p.LastChannel != null)
                .GroupBy(p => p.LastChannel);

            foreach (var group in groups)
            {
                var finalMessage = message;
                if (mention != null)
                    finalMessage += $"\n {mention.Replace("%m", string.Join(", ", group.Select(a => a.User.Mention)))}";
                await group.Key.SendMessageAsync(finalMessage);
            }
        }

        /// <summary>
        /// Start the battle.
        /// </summary>
        public async Task Start()
        {
            foreach (var attackable in Attackables) attackable.Battle = this;
            Active = true;

            var attackers = string.Join(", ", Attackers.Select(a => $"**{a.Name}**"));
            var opponents = string.Join(", ", Opponents.Select(a => $"**{a.Name}**"));
            await SendMessage($":crossed_swords: {attackers} started to attack {opponents}!", ":clock: %m The battle has begun, please select your action.");
            await ProgressNpcs();
        }

        /// <summary>
        /// Dispose the battle.
        /// </summary>
        public void Dispose()
        {
            foreach (var attackable in Attackables)
            {
                attackable.Battle = null;
                var player = attackable as Player;
                if (player != null) Context.Players.Update(player);
            }
            Context.SaveChanges();
            Active = false;
        }
    }
}
