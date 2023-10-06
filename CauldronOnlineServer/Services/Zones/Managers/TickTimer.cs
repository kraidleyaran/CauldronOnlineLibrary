using System;
using CauldronOnlineServer.Interfaces;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Zones.Managers
{
    public delegate void OnTimerLoopFinish();
    public delegate void OnTimerComplete();

    public class TickTimer : IDestroyable
    {
        public int TotalTicks { get; private set; }
        public int TotalLoops { get; private set; }
        public int CurrentTick { get; private set; }
        public int CurrentLoop { get; private set; }

        public event OnTimerLoopFinish OnLoopFinish;
        public event OnTimerComplete OnComplete;

        private string _zoneId = string.Empty;
        private bool _active = false;

        public TickTimer(int ticks, int loops, string zoneId)
        {
            _active = true;
            _zoneId = zoneId;
            TotalTicks = ticks;
            TotalLoops = loops;
            SubscribeToMessages();
        }

        public void Restart()
        {
            if (TotalLoops >= 0)
            {
                CurrentLoop = 0;
            }

            CurrentTick = 0;
            if (!_active)
            {
                this.SubscribeWithFilter<ZoneUpdateTickMessage>(ZoneUpdateTick, _zoneId);
            }
        }

        private void SubscribeToMessages()
        {
            this.SubscribeWithFilter<ZoneUpdateTickMessage>(ZoneUpdateTick, _zoneId);
        }

        private void ZoneUpdateTick(ZoneUpdateTickMessage msg)
        {
            CurrentTick++;
            if (CurrentTick >= TotalTicks)
            {
                OnLoopFinish?.Invoke();
                if (TotalLoops >= 0)
                {
                    CurrentLoop++;
                    if (CurrentLoop >= TotalLoops)
                    {
                        _active = false;
                        this.UnsubscribeFromFilter<ZoneUpdateTickMessage>(_zoneId);
                        OnComplete?.Invoke();
                    }
                }
            }
        }


        public void Destroy()
        {
            OnLoopFinish = null;
            OnComplete = null;
            this.UnsubscribeFromAllMessages();
        }
    }
}