using System;
using System.Collections.Generic;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class MonsterScalingTrait : WorldTrait
    {
        private string[] _applyPerPlayer = new string[0];
        private CombatStats _bonusStatsPerPlayer = new CombatStats();

        private List<WorldTrait> _applied = new List<WorldTrait>();

        private int _playerCount = 0;

        public MonsterScalingTrait(WorldTraitData data) : base(data)
        {
            if (data is MonsterScalingTraitData monsterScaling)
            {
                _applyPerPlayer = monsterScaling.ApplyPerPlayer;
                _bonusStatsPerPlayer = monsterScaling.Stats;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            this.SubscribeWithFilter<PlayerEnteredWorldMessage>(PlayerEnteredWorld, _parent.ZoneId);
            this.SubscribeWithFilter<PlayerLeftWorldMessage>(PlayerLeftWorld, _parent.ZoneId);
        }

        private void PlayerEnteredWorld(PlayerEnteredWorldMessage msg)
        {
            _playerCount++;
            if (_playerCount > 1)
            {
                var traits = TraitService.GetWorldTraits(_applyPerPlayer);
                foreach (var trait in traits)
                {
                    _parent.AddTrait(trait, _parent);
                }

                this.SendMessageTo(new ApplyCombatStatsMessage { Stats = _bonusStatsPerPlayer, Bonus = true }, _parent);
            }

        }

        private void PlayerLeftWorld(PlayerLeftWorldMessage msg)
        {
            _playerCount--;
            if (_playerCount > 0)
            {
                foreach (var trait in _applyPerPlayer)
                {
                    var index = _applied.FindIndex(t => t.Name == trait);
                    if (index >= 0)
                    {
                        _applied.RemoveAt(index);
                        _parent.RemoveTrait(_applied[index]);
                    }
                }

                this.SendMessageTo(new ApplyCombatStatsMessage { Stats = _bonusStatsPerPlayer * -1, Bonus = true }, _parent);
            }

        }


    }
}