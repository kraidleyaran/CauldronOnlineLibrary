using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Traits
{
    public class TerrainTrait : WorldTrait
    {
        private TerrainParameter _parameter = new TerrainParameter { Hitbox = new HitboxData { Size = WorldVector2Int.One} };

        public TerrainTrait(WorldTraitData data) : base(data)
        {
            if (data is TerrainTraitData terrainData)
            {
                _parameter.Hitbox = terrainData.Hitbox;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_parameter);
        }
    }
}