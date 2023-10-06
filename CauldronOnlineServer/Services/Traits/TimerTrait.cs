using System.Collections.Generic;
using CauldronOnlineCommon.Data.Traits;
using CauldronOnlineServer.Services.Zones;
using CauldronOnlineServer.Services.Zones.Managers;

namespace CauldronOnlineServer.Services.Traits
{
    public class TimerTrait : WorldTrait
    {
        private string[] _applyOnStart;
        private string[] _applyOnLoop;
        private int _totalTicks;
        private int _totalLoops;

        private TickTimer _timer = null;

        private List<WorldTrait> _applied = new List<WorldTrait>();

        public TimerTrait(WorldTraitData data) : base(data)
        {
            if (data is TimerTraitData timerData)
            {
                _applyOnStart = timerData.ApplyOnStart;
                _applyOnLoop = timerData.ApplyOnLoop;
                _totalTicks = timerData.TotalTicks;
                _totalLoops = timerData.TotalLoops;
            }
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            if (_applyOnStart.Length > 0)
            {
                var traits = TraitService.GetWorldTraits(_applyOnStart);
                var applied = new List<WorldTrait>();
                foreach (var trait in traits)
                {
                    _parent.AddTrait(trait);
                    if (!trait.Instant)
                    {
                        applied.Add(trait);
                    }
                }
                _applied.AddRange(applied);
            }

            _timer = new TickTimer(_totalTicks, _totalLoops, _parent.ZoneId);
            _timer.OnLoopFinish += LoopFinished;
            _timer.OnComplete += TimerComplete;
        }

        private void LoopFinished()
        {
            if (_applyOnLoop.Length > 0)
            {
                var traits = TraitService.GetWorldTraits(_applyOnLoop);
                var applied = new List<WorldTrait>();
                foreach (var trait in traits)
                {
                    _parent.AddTrait(trait);
                    if (!trait.Instant)
                    {
                        applied.Add(trait);
                    }
                }
                _applied.AddRange(applied);
            }
        }

        private void TimerComplete()
        {
            _timer.Destroy();
            _timer = null;
            _parent.RemoveTrait(this);
        }




    }
}