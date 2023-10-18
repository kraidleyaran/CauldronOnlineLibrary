using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class SetObjectActiveStateTrait : WorldTrait
    {
        public override bool Instant => true;

        private bool _active = false;

        public SetObjectActiveStateTrait(WorldTraitData data) : base(data)
        {
            if (data is SetObjectActiveStateTraitData stateData)
            {
                _active = stateData.Active;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.SetObjectState(_active ? WorldObjectState.Active : WorldObjectState.Disabled);
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            zone?.EventManager.RegisterEvent(new ObjectStateEvent{Active = _active, TargetId = _parent.Data.Id});
        }
    }
}