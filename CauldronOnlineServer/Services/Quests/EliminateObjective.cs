using System.Collections.Generic;
using CauldronOnlineCommon.Data.Quests;
using CauldronOnlineCommon.Data.Zones;
using CauldronOnlineServer.Services.Traits;
using CauldronOnlineServer.Services.Zones;

namespace CauldronOnlineServer.Services.Quests
{
    public class EliminateObjective : QuestObjective
    {
        public ZoneSpawnData Template;
        private bool _staticPosition = false;

        public EliminateObjective(QuestObjectiveData data) : base(data)
        {
            if (data is EliminateQuestObjectiveData eliminate)
            {
                Template = eliminate.Template;
                _staticPosition = eliminate.StaticPosition;
            }
        }

        private List<WorldObject> _created = new List<WorldObject>();

        public override WorldObject[] SpawnRequiredObjects(WorldZone zone, ZoneTile[] availableTiles, WorldObject questParent, string questName)
        {
            for (var i = 0; i < RequiredAmount; i++)
            {
                ZoneTile tile = null;
                if (_staticPosition)
                {
                    tile = Template.IsWorldPosition ? zone.GetTile(Template.Tile) : zone.GetTile(questParent.Tile.Position + Template.Tile);
                }
                if (tile == null)
                {
                    tile = availableTiles.Length > 1 ? availableTiles[RNGService.Range(0, availableTiles.Length)] : availableTiles[0];
                }
                
                zone.ObjectManager.RequestObject(Template.Spawn.DisplayName, Template.Spawn.Traits,
                    Template.Spawn.ShowNameOnClient, Template.Spawn.Parameters, tile.WorldPosition,
                    Template.Spawn.IsMonster, obj => ObjectCreated(obj, questParent, questName), Template.Spawn.ShowOnClient, true, Template.Spawn.ShowAppearance, true, Template.Spawn.MinimapIcon);
            }

            return _created.ToArray();

        }

        private void ObjectCreated(WorldObject obj, WorldObject questParent, string questName)
        {
            obj.AddTrait(new EliminateObjectiveTrait(questParent, this, questName));
            _created.Add(obj);
        }


    }
}