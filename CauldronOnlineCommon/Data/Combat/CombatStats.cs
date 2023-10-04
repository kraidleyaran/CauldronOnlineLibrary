using System;
using System.Collections.Generic;

namespace CauldronOnlineCommon.Data.Combat
{
    [Serializable]
    public struct CombatStats
    {
        public const string MAX_HEALTH = "Max Health";
        public const string MAX_MANA = "Max Mana";
        public const string ARMOR = "Armor";
        public const string STRENGTH = "Strength";
        public const string AGILITY = "Agility";
        public const string WISDOM = "Wisdom";
        public const string LUCK = "Luck";

        public int Health;
        public int Mana;
        public int Armor;
        public int Strength;
        public int Agility;
        public int Wisdom;
        public int Luck;

        public static CombatStats operator +(CombatStats stats) => stats;

        public static CombatStats operator -(CombatStats stats) => new CombatStats
        {
            Health = stats.Health * -1,
            Mana = stats.Mana * -1,
            Armor = stats.Armor * -1,
            Strength = stats.Strength * -1,
            Agility = stats.Agility * -1,
            Wisdom = stats.Wisdom * 1,
            Luck = stats.Luck * 1
        };

        public static CombatStats operator +(CombatStats a, CombatStats b) => new CombatStats
        {
            Health = a.Health + b.Health,
            Mana = a.Mana + b.Mana,
            Armor = a.Armor + b.Armor,
            Strength = a.Strength + b.Strength,
            Agility = a.Agility + b.Agility,
            Wisdom = a.Wisdom + b.Wisdom,
            Luck = a.Luck + b.Luck
        };

        public static CombatStats operator -(CombatStats a, CombatStats b) => new CombatStats
        {
            Health = a.Health - b.Health,
            Mana = a.Mana - b.Mana,
            Armor = a.Armor - b.Armor,
            Strength = a.Strength - b.Strength,
            Agility = a.Agility - b.Agility,
            Wisdom = a.Wisdom - b.Wisdom,
            Luck = a.Luck - b.Luck
        };

        public static CombatStats operator *(CombatStats a, int multiplier) => new CombatStats
        {
            Health = a.Health * multiplier,
            Mana = a.Mana * multiplier,
            Armor = a.Armor * multiplier,
            Strength = a.Strength * multiplier,
            Wisdom = a.Wisdom * multiplier,
            Luck = a.Luck * multiplier
        };

        
    }
}