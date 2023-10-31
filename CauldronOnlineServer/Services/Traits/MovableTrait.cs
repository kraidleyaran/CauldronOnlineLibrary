using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class MovableTrait : WorldTrait
    {
        private MovableParameter _parameter = new MovableParameter();

        public MovableTrait(MovableParameter parameter)
        {
            Name = parameter.Type;
            _parameter = parameter;
        }

        public MovableTrait(WorldTraitData data) : base(data)
        {
            if (data is MovableTraitData movable)
            {
                _parameter.Hitbox = movable.Hitbox;
                _parameter.HorizontalHitbox = movable.HorizontalHitbox;
                _parameter.MoveSpeed = movable.MoveSpeed;
                _parameter.Offset = movable.Offset;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_parameter);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            _parent.SubscribeWithFilter<SetOwnerIdMessage>(SetOwnerId, _id);
            _parent.SubscribeWithFilter<RemoveOwnerIdMessage>(RemoveOwnerId, _id);
            _parent.SubscribeWithFilter<QueryOwnerIdMessage>(QueryOwnerId, _id);
        }

        private void SetOwnerId(SetOwnerIdMessage msg)
        {
            _parameter.OwnerId = msg.Id;
            _parent.RefreshParameters();
        }

        private void RemoveOwnerId(RemoveOwnerIdMessage msg)
        {
            if (!string.IsNullOrEmpty(_parameter.OwnerId) && _parameter.OwnerId == msg.Id)
            {
                _parameter.OwnerId = string.Empty;
                _parent.RefreshParameters();
            }
        }

        private void QueryOwnerId(QueryOwnerIdMessage msg)
        {
            msg.DoAfter.Invoke(_parameter.OwnerId);
        }
    }
}