using System;
using CauldronOnlineCommon.Data.ObjectParameters;

namespace CauldronOnlineCommon.Data.Zones
{
    [Serializable]
    public class ObjectSpawnData
    {
        public string DisplayName;
        public string[] Traits;
        public ObjectParameter[] Parameters;
        public bool IsMonster;
        public bool ShowOnClient;
        public bool ShowNameOnClient;
        public bool ShowAppearance;
        public bool StartActive;
        public string MinimapIcon;
    }
}