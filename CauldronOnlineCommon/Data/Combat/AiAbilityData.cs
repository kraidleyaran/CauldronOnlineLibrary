using System;

namespace CauldronOnlineCommon.Data.Combat
{
    [Serializable]
    public class AiAbilityData
    {
        public string Ability;
        public int Range;
        public int Length;
        public int Cooldown;
        public int Mana;
        public int Priority;
        public int Ids;
        public AbilitySight Sight;
        public string[] ApplyAfterCast;
    }
}