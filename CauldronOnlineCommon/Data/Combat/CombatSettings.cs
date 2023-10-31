using System;

namespace CauldronOnlineCommon.Data.Combat
{
    [Serializable]
    public class CombatSettings
    {
        public float PhysicalDamagePerStrength;
        public float PhysicalDamagePerAgility;
        public float PhysicalDefensePerStrength;
        public float PhysicalDefensePerAgility;
        public float PhysicalCritPerAgility;
        public float MagicalDamagePerWisdom;
        public float ManaReductionPerWisdom;
        public float CriticalStrikePerLuck;
        public float MagicalCritPerWisdom;
        public float ItemQuantityBonusPerLuck;
        public float PhysicalDefensePerArmor;
        public float MagicalDefensePerArmor;
        public float DamagePerDefense;
        public float OutOfPovAggro;
        public float PathChangeAggro;
        public float DamageAggro;
        public float CriticalStrikeDamagePercent;
        public int GlobalReApplyTicks;

        public int CalculateDamageResist(DamageType type, CombatStats stats)
        {
            var defense = 0f;
            switch (type)
            {
                case DamageType.Physical:
                    defense += PhysicalDefensePerAgility * stats.Agility;
                    defense += PhysicalDefensePerStrength * stats.Strength;
                    defense += PhysicalDefensePerArmor * stats.Armor;
                    break;
                case DamageType.Magical:
                    defense += MagicalDefensePerArmor * stats.Armor;
                    defense += MagicalDamagePerWisdom * stats.Wisdom;
                    break;
            }

            return (int) System.Math.Round(defense * DamagePerDefense);
        }

        public int CalculateDamageBonus(DamageType type, CombatStats stats)
        {
            var bonus = 0f;
            switch (type)
            {
                case DamageType.Physical:
                    bonus += PhysicalDamagePerStrength * stats.Strength;
                    bonus += PhysicalDamagePerAgility * stats.Agility;
                    break;
                case DamageType.Magical:
                    bonus += MagicalDamagePerWisdom * stats.Wisdom;
                    break;
            }

            return (int) System.Math.Round(bonus);
        }

        public float CalculateCriticalStrike(DamageType type, CombatStats stats)
        {
            var bonus = CriticalStrikePerLuck * stats.Luck;
            switch (type)
            {
                case DamageType.Physical:
                    bonus += PhysicalCritPerAgility * stats.Agility;
                    break;
                case DamageType.Magical:
                    bonus += MagicalCritPerWisdom * stats.Wisdom;
                    break;
            }

            return bonus;
        }

        public int CalculateManaReduction(CombatStats stats)
        {
            return (int)ManaReductionPerWisdom * stats.Wisdom;
        }

        public int CalculateCriticalStrikeDamage(int amount)
        {
            return (int) System.Math.Round(amount * CriticalStrikeDamagePercent);
        }

        public int AggroPerPovCheck(int value = 1)
        {
            return (int)System.Math.Round(value * OutOfPovAggro);
        }

        public int AggroPerPathChangeCheck(int value = 1)
        {
            return (int)System.Math.Round(value * PathChangeAggro);
        }

        public int AggroPerDamage(int value = 1)
        {
            return (int) System.Math.Round(value * DamageAggro);
        }
    }
}