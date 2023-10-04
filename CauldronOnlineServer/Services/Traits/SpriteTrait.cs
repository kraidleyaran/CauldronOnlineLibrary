using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Traits
{
    public class SpriteTrait : WorldTrait
    {
        public const string NAME = "CustomSprite";

        private string _sprite = string.Empty;

        public SpriteTrait(string sprite)
        {
            Name = NAME;
            _sprite = sprite;
        }

        public SpriteTrait(WorldTraitData data) : base(data)
        {
            if (data is SpriteTraitData spriteData)
            {
                _sprite = spriteData.Sprite;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.Data.Sprite = _sprite;
        }
    }
}