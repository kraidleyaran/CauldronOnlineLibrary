﻿using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

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

        public virtual bool OpenChest(string player)
        {
            if (!_open)
            {
                _open = true;
            }
            return _open;
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            SubscribeToMessages();
        }

        protected internal virtual void SubscribeToMessages()
        {
            _parent.SubscribeWithFilter<OpenChestMessage>(OpenChest, _id);
        }

        protected internal virtual void OpenChest(OpenChestMessage msg)
        {
            OpenChest(msg.Player);
        }
    }
}