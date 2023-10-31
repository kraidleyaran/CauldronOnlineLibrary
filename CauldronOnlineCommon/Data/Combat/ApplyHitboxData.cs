using System;

namespace CauldronOnlineCommon.Data.Combat
{
    [Serializable]
    public class ApplyHitboxData : HitboxData
    {
        public string[] ApplyOnServer;
        public string[] ApplyOnClient;
        public bool ReApply;
    }
}