using System;
using CauldronOnlineCommon.Data.Combat;
using CauldronOnlineCommon.Data.Math;

namespace CauldronOnlineCommon.Data
{
    [Serializable]
    public class ClientCharacterData
    {
        public string DisplayName;
        public string Sprite;
        public SpriteColorData Colors;
        public CombatStats Stats;
        public CombatVitals Vitals;
    }
}