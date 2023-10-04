using System.Threading;
using CauldronOnlineCommon.Data;
using CauldronOnlineServer.Interfaces;
using MessageBusLib;

namespace CauldronOnlineServer.Services.Zones.Managers
{
    public class TickManager : WorldManager
    {
        public WorldTick CurrenTick;

        private Thread _tickThread = null;

        private int _tickEveryMs = 10;
        private bool _active = true;
        private string _zoneId = string.Empty;

        private UpdateCurrentZoneTickMessage _updateCurrentZoneTickMsg = new UpdateCurrentZoneTickMessage();

        public TickManager(int tickEvery, string zoneId)
        {
            _tickEveryMs = tickEvery;
            _zoneId = zoneId;
            _tickThread = new Thread(Tick);
            _tickThread.Start();
        }

        private void Tick()
        {
            while (_active)
            {
                CurrenTick.AddTick();
                _updateCurrentZoneTickMsg.Tick = CurrenTick;
                this.SendMessageWithFilter(_updateCurrentZoneTickMsg, _zoneId);
                this.SendMessageWithFilter(ZoneUpdateTickMessage.INSTANCE, _zoneId);
                this.SendMessageWithFilter(ZoneEventProcessTickMessage.INSTANCE, _zoneId);
                this.SendMessageWithFilter(ZonePlayerUpdateTickMessage.INSTANCE, _zoneId);
                this.SendMessageWithFilter(ZoneResolveTickMessage.INSTANCE, _zoneId);
                Thread.Sleep(_tickEveryMs);
            }

            _tickThread = null;
        }
        

        public override void Destroy()
        {
            _active = false;
            base.Destroy();
        }
    }
}