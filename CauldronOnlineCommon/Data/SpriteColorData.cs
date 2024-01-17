using System;
using Newtonsoft.Json;

namespace CauldronOnlineCommon.Data
{
    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class SpriteColorData
    {
        public string Hair { get; set; }
        public string Eyes { get; set; }
        public string PrimaryShirt { get; set; }
        public string SecondaryShirt { get; set; }
        public string Pants { get; set; }
        public string Shoes { get; set; }
    }
}