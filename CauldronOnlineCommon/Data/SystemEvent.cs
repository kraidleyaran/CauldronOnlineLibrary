using System;
using Newtonsoft.Json;

namespace CauldronOnlineCommon.Data
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class SystemEvent
    {
        public SystemEventType Type { get; set; }
        public string Message { get; set; }
    }
}