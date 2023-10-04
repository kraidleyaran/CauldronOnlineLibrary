using System;
using CauldronOnlineCommon.Data.Math;

namespace CauldronOnlineCommon.Data.Combat
{
    [Serializable]
    public class HitboxData
    {
        public WorldVector2Int Size;
        public WorldVector2Int Offset;
    }
}