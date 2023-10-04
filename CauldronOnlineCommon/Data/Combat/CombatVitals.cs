using System;

namespace CauldronOnlineCommon.Data.Combat
{
    [Serializable]
    public struct CombatVitals
    {
        public int Health;
        public int Mana;

        public void SetFromStats(CombatStats stats)
        {
            Health = stats.Health;
            Mana = stats.Mana;
        }
    }
}