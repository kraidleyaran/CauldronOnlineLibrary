using System;
using CauldronOnlineCommon.Data.Math;

namespace CauldronOnlineCommon.Data
{
    [Serializable]
    public class RegisteredPlayerData
    {
        public string PlayerId;
        public string DisplayName;
        public SpriteColorData SpriteColors;
        public string Zone;
        public WorldVector2Int Position;

        public void Update(RegisteredPlayerData data)
        {
            DisplayName = data.DisplayName;
            SpriteColors = data.SpriteColors;
            Zone = data.Zone;
            Position = data.Position;
        }
    }
}