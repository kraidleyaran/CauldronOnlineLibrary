using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;
using MessageBusLib;

namespace CauldronOnlineServer.Services.Traits
{
    public class SetDoorStateTrait : WorldTrait
    {
        public override bool Instant => true;

        private bool _open = false;

        public SetDoorStateTrait(WorldTraitData data) : base(data)
        {
            if (data is SetDoorStateTraitData doorData)
            {
                _open = doorData.Open;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            parent.SendMessageTo(new SetDoorStateMessage{IsEvent = false, Open = _open}, _parent);
        }
    }
}