using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Traits
{
    public class ToggleObjectStateTrait : WorldTrait
    {
        public override bool Instant => true;

        public ToggleObjectStateTrait(WorldTraitData data) : base(data)
        {

        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            var setState = !parent.Data.Active;
            parent.SetObjectState(setState ? WorldObjectState.Active : WorldObjectState.Disabled);
            var zone = ZoneService.GetZoneById(parent.ZoneId);
            if (zone != null)
            {
                zone.EventManager.RegisterEvent(new ObjectStateEvent{Active = setState, TargetId = _parent.Data.Id});
            }
        }
    }
}