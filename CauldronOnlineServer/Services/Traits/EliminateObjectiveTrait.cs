using CauldronOnlineServer.Services.Quests;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class EliminateObjectiveTrait : WorldTrait
    {
        private EliminateObjective _objective = null;
        private WorldObject _questParent = null;

        public EliminateObjectiveTrait(WorldObject questParent, EliminateObjective objective, string name)
        {
            Name = name;
            _objective = objective;
            _questParent = questParent;
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
            this.SendMessageTo(new ApplyObjectiveItemCompletetionMessage{Objective = _objective, Count = 1}, _questParent);
            _parent.RemoveTrait(this);
        }
    }
}