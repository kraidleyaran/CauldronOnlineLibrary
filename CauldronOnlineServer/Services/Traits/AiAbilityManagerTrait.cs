using System;
using System.Collections.Generic;
using System.Linq;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Zones;
using CauldronOnlineServer.Services.Zones.Managers;
using MessageBusLib;

namespace CauldronOnlineServer.Services.Traits
{
    public class AiAbilityManagerTrait : WorldTrait
    {
        private AiAbilityData[] _abilities = new AiAbilityData[0];

        private Dictionary<AiAbilityData, TickTimer> _cooldowns = new Dictionary<AiAbilityData, TickTimer>();
        private ZoneTile[] _pov = new ZoneTile[0];

        private TickTimer _castingTimer = null;

        public AiAbilityManagerTrait(WorldTraitData data) : base(data)
        {
            if (data is AiAbilityManagerTraitData abilityData)
            {
                _abilities = abilityData.Abilities;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(new AbilityManagerParameter{ApplyTraitsOnClient = true});
            SubscribeToMessages();
        }

        private void OnAbilityTimerFinished(AiAbilityData ability)
        {
            if (_parent.State == WorldObjectState.Attacking)
            {
                _parent.SetObjectState(WorldObjectState.Active);
            }

            var timer = new TickTimer(ability.Cooldown, 0, _parent.ZoneId);
            timer.OnComplete += () => { OnCooldownTimerFinished(ability); };
            _cooldowns.Add(ability, timer);
           
        }

        private void OnCooldownTimerFinished(AiAbilityData ability)
        {
            if (_cooldowns.TryGetValue(ability, out var timer))
            {
                _cooldowns.Remove(ability);
                timer.Destroy();
            }
        }

        private bool AbilityCanbeUsed(AiAbilityData ability, int distance, ZoneTile tile)
        {
            var canUse = true;
            if (ability.Mana > 0)
            {
                canUse = false;
                this.SendMessageTo(new QueryCombatVitalsMessage{DoAfter = vitals => canUse = vitals.Mana >= ability.Mana }, _parent);
            }
            if (canUse && !_cooldowns.ContainsKey(ability) && ability.Range >= distance)
            {
                switch (ability.Sight)
                {
                    case AbilitySight.Cardinal:
                        var min = new WorldVector2Int(_parent.Tile.Position.X - WorldServer.Settings.TileSize / 2, _parent.Tile.Position.Y - WorldServer.Settings.TileSize);
                        var max = new WorldVector2Int(_parent.Tile.Position.X + WorldServer.Settings.TileSize / 2, _parent.Tile.Position.Y + WorldServer.Settings.TileSize);
                        canUse = tile.Position.X >= min.X && tile.Position.X <= max.X || tile.Position.Y >= min.Y && tile.Position.Y <= max.Y;
                        break;
                    case AbilitySight.Pov:
                        canUse = Array.IndexOf(_pov, tile) >= 0;
                        break;
                    default:
                        canUse = true;
                        break;
                }
            }
            else
            {
                canUse = false;
            }

            return canUse;
        }

        private void SubscribeToMessages()
        {
            _parent.SubscribeWithFilter<AbilityCheckMessage>(AbilityCheck, _id);
            _parent.SubscribeWithFilter<UpdatePovMessage>(UpdatePov, _id);
        }

        private void AbilityCheck(AbilityCheckMessage msg)
        {
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                var availableAbilities = _abilities.Where(a => AbilityCanbeUsed(a, msg.Distance, msg.Target.Tile)).OrderBy(a => a.Priority).ToArray();
                if (availableAbilities.Length > 0)
                {
                    AiAbilityData ability = null;
                    if (availableAbilities.Length > 1)
                    {
                        var baseAbility = availableAbilities[0];
                        var otherAbilities = availableAbilities.Where(a => a.Priority == baseAbility.Priority).ToList();
                        ability = otherAbilities[RNGService.Range(0, otherAbilities.Count)];
                    }
                    else
                    {
                        ability = availableAbilities[0];
                    }

                    var ids = new string[ability.Ids];
                    for (var i = 0; i < ids.Length; i++)
                    {
                        ids[i] = $"{_parent.Data.Id}-{Guid.NewGuid().ToString()}";
                    }
                    zone.EventManager.RegisterEvent(new AbilityEvent { OwnerId = _parent.Data.Id, TargetId = msg.Target.Data.Id, Direction = msg.Direction, Ability = ability.Ability, Position = _parent.Data.Position, Ids = ids });

                    _castingTimer = new TickTimer(ability.Length, 0, _parent.ZoneId);
                    _castingTimer.OnComplete += () => { OnAbilityTimerFinished(ability); };
                    _parent.SetObjectState(WorldObjectState.Attacking);
                    msg.DoAfter.Invoke(true);

                }
                else
                {
                    msg.DoAfter.Invoke(false);
                }
            }
            else
            {
                msg.DoAfter.Invoke(false);
            }
           
        }

        private void UpdatePov(UpdatePovMessage msg)
        {
            _pov = msg.Pov;
        }

        public override void Destroy()
        {
            if (_castingTimer != null)
            {
                _castingTimer.Destroy();
                _castingTimer = null;
            }

            if (_cooldowns.Count > 0)
            {
                var cooldowns = _cooldowns.Values.ToArray();
                foreach (var cooldown in cooldowns)
                {
                    cooldown.Destroy();
                }
                _cooldowns.Clear();
            }
            _abilities = null;
            base.Destroy();
        }
    }
}