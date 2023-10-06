using System.Threading;
using CauldronOnlineServer.Interfaces;
using CauldronOnlineServer.Logging;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services
{
    public class WorldService : IDestroyable
    {
        public const string DEFAULT_NAME = "WorldService";
        public const string SERVIER_STARTED = "Service Started";

        public virtual string Name => DEFAULT_NAME;

        public virtual void Start()
        {
            Log(SERVIER_STARTED);
        }

        public virtual void Stop()
        {

        }

        public void Destroy()
        {
            this.UnsubscribeFromAllMessages();
        }

        protected internal void Log(string message, LogType type = LogType.Info)
        {
            var logMessage = $"[{Name}] - {message}";
            switch (type)
            {
                case LogType.Info:
                    WorldServer.Logging.Info(logMessage);
                    break;
                case LogType.Warning:
                    WorldServer.Logging.Warning(logMessage);
                    break;
                case LogType.Error:
                    WorldServer.Logging.Error(logMessage);
                    break;
                case LogType.Debug:
                    WorldServer.Logging.Debug(logMessage);
                    break;
            }
        }
    }
}