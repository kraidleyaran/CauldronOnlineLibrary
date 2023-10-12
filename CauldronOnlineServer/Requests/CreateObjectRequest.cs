﻿using System;
using System.Security.Permissions;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Requests
{
    public class CreateObjectRequest
    {
        public string DisplayName;
        public string[] Traits;
        public WorldVector2Int Position;
        public bool ShowOnClient;
        public bool IsMonster;
        public Action<WorldObject> DoAfter;
        public ObjectParameter[] Parameters;
        public bool ShowName;
        public bool ShowAppearance;

        public CreateObjectRequest(string displayName, string[] traits, bool showName, ObjectParameter[] parameters, WorldVector2Int position, bool isMonster, Action<WorldObject> doAfter, bool showOnClient, bool showAppearance)
        {
            DisplayName = displayName;
            Traits = traits;
            Position = position;
            Parameters = parameters;
            ShowOnClient = showOnClient;
            DoAfter = doAfter;
            IsMonster = isMonster;
            ShowName = showName;
            ShowAppearance = showAppearance;
        }
    }
}