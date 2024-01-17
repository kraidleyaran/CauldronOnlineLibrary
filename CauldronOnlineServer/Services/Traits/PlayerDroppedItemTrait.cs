using CauldronOnlineCommon;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class PlayerDroppedItemTrait : WorldTrait
    {
        private DroppedItemParameter _parameter = new DroppedItemParameter();

        private bool _claimed = false;

        public PlayerDroppedItemTrait(string item, int stack)
        {
            _parameter.Item = item;
            _parameter.Stack = stack;
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parameter.ItemId = _parent.Data.Id;
            _parent.AddParameter(_parameter);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            _parent.SubscribeWithFilter<ClaimItemMessage>(ClaimItem, _id);
        }

        private void ClaimItem(ClaimItemMessage msg)
        {
            if (_claimed)
            {
                this.SendMessageTo(new ClientClaimItemResultMessage { Success = false, ObjectId = _parent.Data.Id}, msg.Owner);
            }
            else
            {
                _claimed = true;
                this.SendMessageTo(new ClientClaimItemResultMessage { Success = true, ObjectId = _parent.Data.Id}, msg.Owner);
                var zone = ZoneService.GetZoneById(_parent.ZoneId);
                if (zone != null)
                {
                    zone.EventManager.RegisterEvent(new DestroyObjectEvent { ObjectId = _parent.Data.Id, OwnerId = _parent.Data.Id });
                    zone.ObjectManager.RequestDestroyObject(_parent.Data.Id);
                }
            }
            
        }
    }
}