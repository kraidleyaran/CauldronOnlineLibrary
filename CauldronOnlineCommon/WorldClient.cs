using System.Collections.Generic;
using Telepathy;

namespace CauldronOnlineCommon
{
    public delegate void OnClientState(WorldClientState state);
    public delegate void OnMessageData(int dataLength);

    public class WorldClient
    {
        public event OnClientState OnStateUpdate;
        public event OnMessageData OnMessageData;

        public WorldClientState State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnStateUpdate?.Invoke(_state);
                }

            }
        }

        private Client _client = null;
        private WorldClientState _state = WorldClientState.Disconnected;
        
        private Dictionary<string, List<ClientMultiPartMessage>> _multiPartMessages = new Dictionary<string, List<ClientMultiPartMessage>>();

        public void Connect(string ipAddress, int port)
        {
            if (_state == WorldClientState.Disconnected)
            {
                State = WorldClientState.Connecting;
                _client = new Client();
                _client.MaxMessageSize = CauldronUtils.MAX_MESSAGE_SIZE;
                _client.Connect(ipAddress, port);
            }

        }

        public ClientMessage[] ReadMessages()
        {
            var clientMessages = new List<ClientMessage>();
            if (_client != null && (_client.Connected || _client.Connecting))
            {
                while (_client.GetNextMessage(out var message))
                {
                    switch (message.eventType)
                    {
                        case EventType.Connected:
                            State = WorldClientState.Connected;
                            break;
                        case EventType.Data:
                            OnMessageData?.Invoke(message.data.Length);
                            clientMessages.Add(message.data.GenerateMessageFromData());
                            break;
                        case EventType.Disconnected:
                            State = WorldClientState.Disconnected;
                            break;
                    }
                }
            }
            return clientMessages.ToArray();
        }

        public void Send<T>(T clientMsg) where T : ClientMessage
        {
            if (State == WorldClientState.Connected)
            {
                _client.Send(clientMsg.ToByteArray());
            }
        }

        public void Disconnect()
        {
            if (_client != null)
            {
                if (_client.Connecting || _client.Connected)
                {
                    _client.Disconnect();
                }
                _client = null;
                State = WorldClientState.Disconnected;
            }
        }
    }
}