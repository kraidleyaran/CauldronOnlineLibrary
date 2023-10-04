using System;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Interfaces;
using CauldronOnlineServer.Services.Zones;
using MessageBusLib;

namespace CauldronOnlineServer.Services.Traits
{
    public class WorldTrait : IDestroyable
    {
        public const string TRAIT = "Trait";

        public string Name;
        public int MaxStack = 1;
        public virtual bool Instant => false;

        protected internal WorldObject _parent = null;
        protected internal string _id = string.Empty;

        public WorldTrait()
        {
            Name = TRAIT;
        }

        public WorldTrait(WorldTraitData data)
        {
            Name = data.Name;
            MaxStack = data.MaxStack;
        }

        public virtual void Setup(WorldObject parent, object sender)
        {
            _parent = parent;
            _id = $"{TRAIT}-{Guid.NewGuid().ToString()}";
        }

        public virtual void Destroy()
        {
            _parent?.UnsubscribeFromAllMessagesWithFilter(_id);
            this.UnsubscribeFromAllMessages();
        }
    }
}