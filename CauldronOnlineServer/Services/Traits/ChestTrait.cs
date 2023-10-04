using CauldronOnlineCommon.Data.Traits;
using MessageBusLib;

namespace CauldronOnlineServer.Services.Traits
{
    public class ChestTrait : WorldTrait
    {
        protected internal bool _open = false;

        public ChestTrait()
        {

        }

        public ChestTrait(WorldTraitData data) : base(data)
        {

        }

        public virtual bool OpenChest()
        {
            if (!_open)
            {
                _open = true;
            }
            return _open;
        }

        

        protected internal void SubscribeToMessages()
        {
            _parent.SubscribeWithFilter<OpenChestMessage>(OpenChest, _id);
        }

        private void OpenChest(OpenChestMessage msg)
        {
            OpenChest();
        }
    }
}