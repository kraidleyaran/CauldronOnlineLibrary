using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class WorldTraitData
    {
        public const string DEFAULT = "Default";
        public const string EXTENSION = "wtd";

        public string Name;
        public int MaxStack;
        public virtual string Type => DEFAULT;
    }
}