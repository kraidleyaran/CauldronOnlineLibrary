using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Traits
{
    public class TilemapTrait : WorldTrait
    {
        private TilemapParameter _parameter = new TilemapParameter();

        public TilemapTrait(WorldTraitData data) : base(data)
        {
            if (data is TilemapTraitData tilemap)
            {
                _parameter.Tilemap = tilemap.Tilemap;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_parameter);
        }

        public override void Destroy()
        {
            _parent?.RemoveParameter(_parameter.Type);
            base.Destroy();
        }
    }
}