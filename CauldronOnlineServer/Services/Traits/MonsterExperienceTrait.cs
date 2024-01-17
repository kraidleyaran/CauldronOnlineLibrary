using CauldronOnlineCommon;
using CauldronOnlineCommon.Data.Math;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class MonsterExperienceTrait : WorldTrait
    {
        private WorldIntRange _experience = new WorldIntRange(1,1);

        public MonsterExperienceTrait(WorldTraitData data) : base(data)
        {
            if (data is MonsterExperienceTraitData experienceData)
            {
                _experience = experienceData.Experience;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            _parent.SubscribeWithFilter<ObjectDeathMessage>(ObjectDeath, _id);
        }

        private void ObjectDeath(ObjectDeathMessage msg)
        {
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                var players = zone.ObjectManager.GetPlayersInZone();
                
                if (players.Length > 0)
                {
                    var experience = RNGService.Range(_experience.Min, _experience.Max + 1);
                    var clientExperienceMsg = new ClientExperienceMessage { Amount = experience, OriginId = _parent.Data.Id, Players = players.Length};
                    var applyExperienceMsg = new ApplyExperienceMessage {Message = clientExperienceMsg.ToByteArray()};
                    foreach (var player in players)
                    {
                        this.SendMessageTo(applyExperienceMsg, player);
                    }
                }

            }

            
        }
    }
}