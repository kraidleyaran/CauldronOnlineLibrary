using System.Linq;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Traits
{
    public class HitboxTrait : WorldTrait
    {
        private ApplyHitboxData[] _hitboxes = new ApplyHitboxData[0];

        public HitboxTrait(WorldTraitData data) : base(data)
        {
            if (data is HitboxTraitData hitbox)
            {
                _hitboxes = hitbox.Hitboxes.ToArray();
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(new HitboxParameter{Hitboxes = _hitboxes});
        }

        public override void Destroy()
        {
            _parent?.RemoveParameter(HitboxParameter.TYPE);
            base.Destroy();
        }
    }
}