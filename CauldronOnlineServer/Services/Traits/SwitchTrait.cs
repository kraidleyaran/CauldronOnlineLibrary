using CauldronOnlineCommon.Data.ObjectParameters;
using CauldronOnlineCommon.Data.WorldEvents;
using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class SwitchTrait : WorldTrait
    {
        public const string SWITCH = "Switch";

        private SwitchParameter _parameter = new SwitchParameter();
        private string _filter = string.Empty;

        public SwitchTrait(SwitchParameter parameter)
        {
            Name = parameter.Type;
            _parameter = parameter;
        }

        public static string GenerateFilter(string signalName, string zoneId)
        {
            return $"{zoneId}-{SWITCH}-{signalName}";
        }

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            _parent.AddParameter(_parameter);
            _filter = GenerateFilter(_parameter.Name, _parent.ZoneId);
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                zone.SetBlockedTile(_parent.Tile.Position, _parent);
            }
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            _parent.SubscribeWithFilter<SetSwitchSignalMessage>(SetSwitchSignal, _filter);
            _parent.SubscribeWithFilter<SetSwitchLockStateMessage>(SetSwitchLockState, _filter);
            _parent.SubscribeWithFilter<AdvanceSwitchSignalMessage>(AdvanceSwitchSignal, _filter);
        }

        private void SetSwitchSignal(SetSwitchSignalMessage msg)
        {
            if ((!_parameter.Locked || msg.OverrideLock) && msg.Signal < _parameter.Signals.Length)
            {
                _parameter.CurrentSignal = msg.Signal;
                if (!msg.OverrideLock && _parameter.LockOnInteract)
                {
                    _parameter.Locked = true;
                }
            }

            if (!msg.IsEvent)
            {
                var zone = ZoneService.GetZoneById(_parent.Data.Id);
                if (zone != null)
                {
                    zone.EventManager.RegisterEvent(new SwitchSignalEvent{TargetId = _parent.Data.Id, Signal = _parameter.CurrentSignal, Locked = _parameter.Locked});
                }
            }
            _parent.RefreshParameters();
            this.SendMessageWithFilter(new UpdateSignalMessage{Signal = _parameter.CurrentSignal, SwitchName = _parameter.Name}, _filter);
        }

        private void SetSwitchLockState(SetSwitchLockStateMessage msg)
        {
            _parameter.Locked = msg.Locked;
            _parent.RefreshParameters();
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                zone.EventManager.RegisterEvent(new SwitchSignalEvent { TargetId = _parent.Data.Id, Signal = _parameter.CurrentSignal, Locked = _parameter.Locked });
            }
        }

        private void AdvanceSwitchSignal(AdvanceSwitchSignalMessage msg)
        {
            _parameter.CurrentSignal++;
            if (_parameter.CurrentSignal >= _parameter.Signals.Length)
            {
                _parameter.CurrentSignal = 0;
            }
            _parent.RefreshParameters();
            var zone = ZoneService.GetZoneById(_parent.ZoneId);
            if (zone != null)
            {
                zone.EventManager.RegisterEvent(new SwitchSignalEvent { TargetId = _parent.Data.Id, Signal = _parameter.CurrentSignal, Locked = _parameter.Locked });
            }
            this.SendMessageWithFilter(new UpdateSignalMessage { Signal = _parameter.CurrentSignal, SwitchName = _parameter.Name }, _filter);
        }

        public override void Destroy()
        {
            if (_parent.Tile != null)
            {
                var zone = ZoneService.GetZoneById(_parent.Data.Id);
                if (zone != null)
                {
                    zone.RemoveBlockedTile(_parent.Tile.Position, _parent);
                }
            }
            base.Destroy();
        }
    }
}