using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class OnDamageTakenTrait : WorldTrait
    {
        private int _requiredAmount = 0;
        private string[] _applyOnDamageAmount = new string[0];
        private bool _resetOnApply = false;

        private int _currentAmount = 0;

        public OnDamageTakenTrait(WorldTraitData data) : base(data)
        {
            if (data is OnDamageTakenTraitData damage)
            {
                _requiredAmount = damage.RequiredAmount;
                _applyOnDamageAmount = damage.ApplyOnAmount;
                _resetOnApply = damage.ResetOnApply;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            _parent.SubscribeWithFilter<DamageTakenMessage>(DamageTaken, _id);
        }

        private void DamageTaken(DamageTakenMessage msg)
        {
            _currentAmount += msg.Amount;
            if (_currentAmount >= _requiredAmount)
            {
                _currentAmount -= _requiredAmount;
                var traits = TraitService.GetWorldTraits(_applyOnDamageAmount);
                foreach (var trait in traits)
                {
                    _parent.AddTrait(trait);
                }
            }

            if (!_resetOnApply)
            {
                _parent.RemoveTrait(this);
            }
        }
    }
}