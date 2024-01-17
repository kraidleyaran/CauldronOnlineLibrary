using System;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.ObjectParameters;

namespace CauldronOnlineCommon.Data
{
    [Serializable]
    public class ClientObjectData
    {
        public string DisplayName;
        public string Id;
        public string Sprite;
        public string MinimapIcon;
        public WorldVector2Int Position;
        public WorldVector2Int Direction;
        public byte[][] Parameters = new byte[0][];
        public bool IsMonster = false;
        public bool ShowName = false;
        public bool Active = true;
        
    }
}