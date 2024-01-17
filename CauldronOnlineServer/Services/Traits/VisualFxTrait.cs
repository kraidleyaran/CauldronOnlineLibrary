using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Traits
{
    public class VisualFxTrait : WorldTrait
    {
        public override bool Instant => true;

        private string _visualFx = string.Empty;

        public VisualFxTrait(WorldTraitData data) : base(data)
        {
            if (data is VisualFxTraitData visualFx)
            {
                _visualFx = visualFx.VisualFx;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                zone.EventManager.RegisterEvent(new VisualFxEvent{Name = _visualFx, Position = _parent.Data.Position});
            }
        }
    }
}