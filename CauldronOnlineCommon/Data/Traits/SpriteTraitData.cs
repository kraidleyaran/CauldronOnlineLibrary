using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class SpriteTraitData : WorldTraitData
    {
        public const string TYPE = "Sprite";
        public override string Type => TYPE;

        public string Sprite;
    }
}