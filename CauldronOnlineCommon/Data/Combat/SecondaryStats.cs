using System;

namespace CauldronOnlineCommon.Data.Combat
{
    [Serializable]
    public struct SecondaryStats
    {
        public int PhysicalDamage;
        public int MagicalDamage;
        public int PhysicalDefense;
        public int MagicalDefense;
        public float PhysicalCrit;
        public float MagicalCrit;
        public int ManaReduction;

        public static SecondaryStats operator +(SecondaryStats stats) => stats;

        public static SecondaryStats operator -(SecondaryStats stats) => new SecondaryStats
        {
            PhysicalDamage = stats.PhysicalDamage * -1,
            MagicalDamage = stats.MagicalDamage * -1,
            PhysicalDefense = stats.PhysicalDefense * -1,
            MagicalDefense = stats.MagicalDefense * -1,
            PhysicalCrit = stats.PhysicalCrit * -1,
            MagicalCrit = stats.MagicalCrit * -1,
            ManaReduction = stats.ManaReduction * -1
        };

        public static SecondaryStats operator +(SecondaryStats a, SecondaryStats b) => new SecondaryStats
        {
            PhysicalDamage = a.PhysicalDamage + b.PhysicalDamage,
            MagicalDamage = a.MagicalDamage + b.MagicalDamage,
            PhysicalDefense = a.PhysicalDefense + b.PhysicalDefense,
            MagicalDefense = a.MagicalDefense + b.MagicalDefense,
            PhysicalCrit = a.PhysicalCrit + b.PhysicalCrit,
            MagicalCrit = a.MagicalCrit + b.MagicalCrit,
            ManaReduction = a.ManaReduction + b.ManaReduction
        };

        public static SecondaryStats operator -(SecondaryStats a, SecondaryStats b) => new SecondaryStats
        {
            PhysicalDamage = a.PhysicalDamage - b.PhysicalDamage,
            MagicalDamage =  a.MagicalDamage - b.MagicalDamage,
            PhysicalDefense = a.PhysicalDefense - b.PhysicalDefense,
            MagicalDefense = a.MagicalDefense - b.MagicalDefense,
            PhysicalCrit = a.PhysicalCrit - b.PhysicalCrit,
            MagicalCrit = a.MagicalCrit - b.MagicalCrit,
            ManaReduction = a.ManaReduction - b.ManaReduction
        };

        public static SecondaryStats operator *(SecondaryStats a, int multiplier) => new SecondaryStats
        {
            PhysicalDamage = a.PhysicalDamage * multiplier,
            MagicalDamage = a.MagicalDamage * multiplier,
            PhysicalDefense = a.PhysicalDefense * multiplier,
            MagicalDefense = a.MagicalDefense * multiplier,
            PhysicalCrit = a.PhysicalCrit * multiplier,
            MagicalCrit = a.MagicalCrit * multiplier,
            ManaReduction = a.ManaReduction * multiplier
        };
    }
}