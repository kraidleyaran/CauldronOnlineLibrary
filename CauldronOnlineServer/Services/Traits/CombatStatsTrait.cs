using System;
using CauldronOnlineCommon.Data;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Zones;
using MessageBusLib;

namespace CauldronOnlineServer.Services.Traits
{
    public class CombatStatsTrait : WorldTrait
    {
        private const string NAME = "CustomCombatStats";

        private CombatStats _baseStats = new CombatStats();
        private CombatStats _bonusStats = new CombatStats();
        private SecondaryStats _bonusSecondary = new SecondaryStats();
        private CombatVitals _combatVitals = new CombatVitals();

        private CombatStatsParameter _parameter = new CombatStatsParameter();

        private bool _destroyOnDeath = false;

        public CombatStatsTrait(CombatStats stats, CombatVitals vitals)
        {
            Name = NAME;
            _baseStats = stats;
            _combatVitals = vitals;
            _parameter.Stats = _baseStats + _bonusStats;
            _parameter.Vitals = _combatVitals;
            _parameter.BonusSecondary = _bonusSecondary;
            _destroyOnDeath = false;
        }

        public CombatStatsTrait(WorldTraitData data) : base(data)
        {
            if (data is CombatStatsTraitData combatData)
            {
                _baseStats = combatData.Stats;
                _destroyOnDeath = true;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            var totalStats = _baseStats + _bonusStats;
            _combatVitals.Health = totalStats.Health;
            _combatVitals.Mana = totalStats.Mana;
            _parent.AddParameter(new CombatStatsParameter{Stats = totalStats, Vitals = _combatVitals, Monster = _destroyOnDeath});
            _parent.AddParameter(new ObjectDeathParameter());
            SubscribeToMessages();
        }

        private void ObjectDied(string ownerId)
        {
            _parent.SetObjectState(WorldObjectState.Destroying);
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                zone.EventManager.RegisterEvent(new ObjectDeathEvent { Id = _parent.Data.Id, OwnerId = ownerId});
                zone.ObjectManager.RequestDestroyObject(_parent.Data.Id);
            }
        }

        private void SubscribeToMessages()
        {
            _parent.SubscribeWithFilter<TakeDamageMessage>(TakeDamage, _id);
            _parent.SubscribeWithFilter<ObjectDeathMessage>(ObjectDeath, _id);
            _parent.SubscribeWithFilter<QueryCombatVitalsMessage>(QueryCombatVitals, _id);
            _parent.SubscribeWithFilter<HealMessage>(Heal, _id);
        }

        private void TakeDamage(TakeDamageMessage msg)
        {
            if (_parent.State == WorldObjectState.Active && _combatVitals.Health > 0)
            {
                _combatVitals.Health -= msg.Amount;
                _parameter.Vitals = _combatVitals;
                if (_combatVitals.Health <= 0)
                {
                    this.SendMessageTo(new ObjectDeathMessage{OwnerId = _parent.Data.Id}, _parent);
                }
                else
                {
                    var parameter = _parent.GetParamter<CombatStatsParameter>(CombatStatsParameter.TYPE);
                    if (parameter != null)
                    {
                        parameter.Vitals = _combatVitals;
                    }
                }
                _parent.AddParameter(_parameter);
            }

        }

        private void ObjectDeath(ObjectDeathMessage msg)
        {
            if (_destroyOnDeath && _parent.State != WorldObjectState.Destroying)
            {
                ObjectDied(msg.OwnerId);
            }

        }

        private void QueryCombatVitals(QueryCombatVitalsMessage msg)
        {
            msg.DoAfter.Invoke(_combatVitals);
        }

        private void Heal(HealMessage msg)
        {
            var combined = _baseStats + _bonusStats;
            _combatVitals.Health = Math.Max(Math.Min(_combatVitals.Health + msg.Amount, combined.Health), 0);
            _parameter.Vitals = _combatVitals;
            if (_destroyOnDeath && !msg.IsEvent)
            {
                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    zone.EventManager.RegisterEvent(new HealEvent { Amount = msg.Amount, OwnerId = msg.OwnerId, TargetId = _parent.ZoneId});
                }
            }
            _parent.AddParameter(_parameter);

        }
    }
}