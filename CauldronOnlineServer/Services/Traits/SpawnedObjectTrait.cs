using CauldronOnlineServer.Services.Zones;
using MessageBusLib;

namespace CauldronOnlineServer.Services.Traits
{
    public class SpawnedObjectTrait : WorldTrait
    {
        public const string NAME = "SpawnedObject";

        private WorldObject _spawner = null;

        public SpawnedObjectTrait(WorldObject spawner)
        {
            Name = NAME;
            _spawner = spawner;
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            _parent.SubscribeWithFilter<ObjectDeathMessage>(ObjectDeath, _id);
        }

        private void ObjectDeath(ObjectDeathMessage msg)
        {
            if (_spawner != null)
            {
                this.SendMessageTo(new RemoveObjectFromSpawnerMessage { ObjectId = _parent.Data.Id }, _spawner);
                _spawner = null;
            }

        }

        public override void Destroy()
        {
            if (_spawner != null)
            {
                this.SendMessageTo(new RemoveObjectFromSpawnerMessage { ObjectId = _parent.Data.Id }, _spawner);
                _spawner = null;
            }
            base.Destroy();
        }
    }
}