using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class BombableDoorTrait : WorldTrait
    {
        private BombableDoorParameter _bombableParameter = new BombableDoorParameter();

        public BombableDoorTrait(BombableDoorParameter parameter)
        {
            Name = parameter.Type;
            _bombableParameter = parameter;
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_bombableParameter);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            _parent.SubscribeWithFilter<SetDoorStateMessage>(SetDoorState, _id);
        }

        private void SetDoorState(SetDoorStateMessage msg)
        {
            _bombableParameter.Open = msg.Open;
            _parent.RefreshParameters();
        }
    }
}