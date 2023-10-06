using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;
using CauldronOnlineServer.Services.Zones.Managers;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class HurtboxTrait : WorldTrait
    {
        private WorldVector2Int _size = WorldVector2Int.Zero;
        private WorldVector2Int _offset = WorldVector2Int.Zero;
        private bool _knockback = false;

        private TickTimer _knockbackTimer = null;

        public HurtboxTrait(WorldTraitData data) : base(data)
        {
            if (data is HurtboxTraitData hurtBox)
            {
                _size = hurtBox.Size;
                _offset = hurtBox.Offset;
                _knockback = hurtBox.Knockback;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(new HurtboxParameter{Size = _size, Offset = _offset, Knockback = _knockback});
            _parent.AddParameter(new KnockbackReceiverParameter{ReceiveKnockback = _knockback});
            SubscribeToMessages();
        }

        private void KnockbackFinished(WorldVector2Int position)
        {
            _parent.SetPosition(position);
            this.SendMessageTo(KnockbackFinishedMessage.INSTANCE, _parent);
            _knockbackTimer.Destroy();
            _knockbackTimer = null;
        }

        private void SubscribeToMessages()
        {
            if (_knockback)
            {
                _parent.SubscribeWithFilter<ApplyKnockbackMessage>(ApplyKnockback, _id);
            }
        }

        private void ApplyKnockback(ApplyKnockbackMessage msg)
        {
            if (_knockbackTimer != null)
            {
                _knockbackTimer.Destroy();
                _knockbackTimer = null;
            }
            _knockbackTimer = new TickTimer(msg.Time, 0, _parent.ZoneId);
            var pos = msg.Position;
            _knockbackTimer.OnComplete += () => { KnockbackFinished(pos); };
        }

        public override void Destroy()
        {
            if (_knockbackTimer != null)
            {
                _knockbackTimer.Destroy();
                _knockbackTimer = null;
            }
            _parent?.RemoveParameter(HurtboxParameter.TYPE);
            _parent?.RemoveParameter(KnockbackReceiverParameter.TYPE);
            base.Destroy();
        }
    }
}