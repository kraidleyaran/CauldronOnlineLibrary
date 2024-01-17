using System.Linq;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Traits
{
    public class HitboxTrait : WorldTrait
    {
        private HitboxParameter _parameter = new HitboxParameter();

        public HitboxTrait(WorldTraitData data) : base(data)
        {
            if (data is HitboxTraitData hitbox)
            {
                _parameter.Hitboxes = hitbox.Hitboxes;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_parameter, Name);
        }

        public override void Destroy()
        {
            _parent?.RemoveParameter(HitboxParameter.TYPE);
            base.Destroy();
        }
    }
}