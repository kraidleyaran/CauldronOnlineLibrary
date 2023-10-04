using System;

namespace CauldronOnlineServer.Services.Commands
{
    [Serializable]
    public class WorldCommand
    {
        public string Command { get; set; }
        public string[] Args { get; set; }
    }
}