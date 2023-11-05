using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class ApplyCombatStatsTrait : WorldTrait
    {
        public override bool Instant => !_bonus;

        private CombatStats _stats = new CombatStats();
        private bool _bonus = false;

        public ApplyCombatStatsTrait(WorldTraitData data) : base(data)
        {
            if (data is ApplyCombatStatsTraitData combatData)
            {
                _stats = combatData.Stats;
                _bonus = combatData.Bonus;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            this.SendMessageTo(new ApplyCombatStatsMessage{Stats = _stats, Bonus = _bonus}, _parent);
        }

        public override void Destroy()
        {
            if (_bonus)
            {
                this.SendMessageTo(new ApplyCombatStatsMessage { Stats = _stats * -1, Bonus = _bonus }, _parent);
            }
            base.Destroy();
        }
    }
}