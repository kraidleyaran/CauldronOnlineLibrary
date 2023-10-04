using System;
using CauldronOnlineCommon.Data.Combat;

namespace CauldronOnlineCommon.Data.Traits
{
    [Serializable]
    public class DialogueTraitData : WorldTraitData
    {
        public const string TYPE = "Dialogue";
        public override string Type => TYPE;

        public string[] Dialogue;
        public HitboxData Hitbox;
    }
}