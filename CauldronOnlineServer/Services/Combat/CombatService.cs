using System;
using System.IO;
using CauldronOnlineCommon.Data;
using CauldronOnlineCommon.Data.Combat;
using Newtonsoft.Json;

namespace CauldronOnlineServer.Services.Combat
{
    public class CombatService : WorldService
    {
        private const string NAME = "Combat";

        public static CombatSettings Settings { get; private set; }

        private static CombatService _instance = null;

        public override string Name => NAME;

        private string _settingsPath = string.Empty;

        public CombatService(string settingsPath)
        {
            _settingsPath = settingsPath;
        }

        public override void Start()
        {
            if (_instance == null)
            {
                _instance = this;
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    try
                    {
                        Settings = JsonConvert.DeserializeObject<CombatSettings>(json);
                        Log("Combat Settings loaded succesfully");
                    }
                    catch (Exception ex)
                    {
                        Log($"Error while loading Combat Settings - {ex}");
                        Settings = new CombatSettings();
                    }
                }
                base.Start();
            }
            
        }


    }
}