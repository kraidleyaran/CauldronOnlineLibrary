using CauldronOnlineServer.Services.Zones;
using ConcurrentMessageBus;

namespace CauldronOnlineServer.Services.Traits
{
    public class AiStateTrait : WorldTrait
    {
        private AiState _state = AiState.Idle;

        private UpdateAiStateMessage _updateAiStateMsg = new UpdateAiStateMessage();

        public override void Setup(WorldObject parent, object sender)
        {
            base.Setup(parent, sender);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            _parent.SubscribeWithFilter<SetAiStateMessage>(SetAiState, _id);
            _parent.SubscribeWithFilter<QueryAiStateMessage>(QueryAiState, _id);
        }

        private void SetAiState(SetAiStateMessage msg)
        {
            if (_state != msg.State)
            {
                _state = msg.State;
                _updateAiStateMsg.State = _state;
                this.SendMessageTo(_updateAiStateMsg, _parent);
            }
        }

        private void QueryAiState(QueryAiStateMessage msg)
        {
            msg.DoAfter.Invoke(_state);
        }
    }
}