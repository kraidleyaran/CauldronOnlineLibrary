using CauldronOnlineServer.Interfaces;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Zones.Managers
{
    public class WorldManager : IDestroyable
    {
        public virtual void Destroy()
        {
            this.UnsubscribeFromAllMessages();
        }
    }
}