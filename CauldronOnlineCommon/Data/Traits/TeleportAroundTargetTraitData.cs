using System;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class TeleportAroundTargetTraitData : WorldTraitData
    {
        public const string TYPE = "TeleportAroundTarget";
        public override string Type => TYPE;

        public int Range;
        public int TeleportTicks;
        public string[] ApplyOnTeleportEnd;
    }
}