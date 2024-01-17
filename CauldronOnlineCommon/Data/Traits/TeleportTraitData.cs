using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class TeleportTraitData : WorldTraitData
    {
        public const string TYPE = "Teleport";
        public override string Type => TYPE;

        public int Range;
        public int TeleportTicks;
        public string[] ApplyOnTeleportEnd;
    }
}