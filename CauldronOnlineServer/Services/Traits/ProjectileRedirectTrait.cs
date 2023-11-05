using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Traits
{
    public class ProjectileRedirectTrait : WorldTrait
    {
        private ProjectileRedirectParameter _parameter = new ProjectileRedirectParameter();

        public ProjectileRedirectTrait(WorldTraitData data) : base(data)
        {
            if (data is ProjectileRedirectTraitData projectileData)
            {
                _parameter.Direction = projectileData.Direction;
                _parameter.Hitbox = projectileData.Hitbox;
                _parameter.Tags = projectileData.Tags;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_parameter);
        }
    }
}