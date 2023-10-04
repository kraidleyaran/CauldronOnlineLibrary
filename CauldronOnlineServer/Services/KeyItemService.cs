using System;
using System.Collections.Concurrent;

namespace CauldronOnlineServer.Services
{
    public class KeyItemService : WorldService
    {
        private static KeyItemService _instance = null;

        public override string Name => "KeyItem";

        

        public override void Start()
        {
            if (_instance == null)
            {
                _instance = this;
                base.Start();
            }
            
        }

        
    }
}