using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Traits
{
    public class ZoneTransitionTrait : WorldTrait
    {
        private ZoneTransitionParameter _parameter = new ZoneTransitionParameter();

        public ZoneTransitionTrait(ZoneTransitionParameter parameter)
        {
            Name = parameter.Type;
            _parameter = parameter;
        }

        public ZoneTransitionTrait(WorldTraitData data) : base(data)
        {
            if (data is ZoneTransitionTraitData transitionData)
            {
                _parameter.Zone = transitionData.Zone;
                _parameter.Position = transitionData.Position;
                _parameter.Rotation = transitionData.Rotation;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_parameter);
        }
    }
}