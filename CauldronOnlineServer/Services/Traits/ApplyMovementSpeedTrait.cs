using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class ApplyMovementSpeedTrait : WorldTrait
    {
        public override bool Instant => !_bonus;

        private int _amount = 0;
        private bool _bonus = true;

        public ApplyMovementSpeedTrait(WorldTraitData data) : base(data)
        {
            if (data is ApplyMovementSpeedTraitData moveData)
            {
                _amount = moveData.Amount;
                _bonus = moveData.Bonus;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            this.SendMessageTo(new ApplyMovementSpeedMessage{Speed = _amount, Bonus = _bonus}, _parent);
        }

        public override void Destroy()
        {
            if (_bonus && _parent != null)
            {
                this.SendMessageTo(new ApplyMovementSpeedMessage { Speed = _amount * -1, Bonus = _bonus }, _parent);
            }
            base.Destroy();
        }
    }
}