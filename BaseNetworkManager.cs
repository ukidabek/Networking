using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

using BaseGameLogic.Singleton;
using BaseGameLogic.SceneManagement;

namespace BaseGameLogic.Networking
{
    public abstract class BaseNetworkManager : Singleton<BaseNetworkManager>
    {
        [SerializeField, Header("Network settings.")]
        protected NetworkManagerSettings _settings = new NetworkManagerSettings();

        public bool IsSever { get { return _settings.ManagerType == NetworkManagerTypeEnum.Server; } }

        public NetworkManagerSettings Settings { get { return _settings; } }

        [SerializeField]
        protected BroadcastCredentials _broadcastCredentials = new BroadcastCredentials();

        [SerializeField]
        protected MatchSettings matchSettings = new MatchSettings();

        // Match making.

        protected List<MatchInfoSnapshot> matchList = new List<MatchInfoSnapshot>();
        protected MatchInfo matchInfo;

        [SerializeField, Header("Run time values."), Tooltip("List of connected peers. Do not setup!")]
        protected List<ConnectionInfo> connectedPeers = new List<ConnectionInfo>();

        [SerializeField]
        protected int hostID = 0;

        [SerializeField]
        protected List<string> _logs = new List<string>();

        protected int port = 0;
        protected byte error = 0;
        protected string address = string.Empty;
        protected int connectionID = 0;
        protected int recHostId;
        protected int channelID;
        protected int dataSize;
        protected byte[] receiveBuffer = null;

        protected ConnectionInfo newPear = null;

        protected Dictionary<QosType, int> channelDictionary = new Dictionary<QosType, int>();

        [SerializeField, Space]
        private GameObject _messageHandlersHolder = null;
        [SerializeField]
        private GameObject _messageSendersHolder = null;

        [SerializeField, Space]
        protected List<BaseMessageHandler> _messageHandlersList = new List<BaseMessageHandler>();
        protected Dictionary<int, BaseMessageHandler> _messageHandlersDictionary = new Dictionary<int, BaseMessageHandler>();

        [SerializeField]
        protected List<BaseMessageSender> _messageSendersList = new List<BaseMessageSender>();
        protected Dictionary<int, BaseMessageSender> _messageSendersDictionary = new Dictionary<int, BaseMessageSender>();

        [SerializeField]
        protected List<BaseNetworkUpdater> _networkUpdaterList = new List<BaseNetworkUpdater>();

        public ConnectionEvent ClientConnectedCallback = new ConnectionEvent();
        public ConnectionEvent ClientDisconnectedCallback = new ConnectionEvent();
        public UnityEvent LoadGameCallback = new UnityEvent();

        protected virtual void Initialize()
        {
            // Transport layer initialization.
            NetworkTransport.Init();

            // Connection configuration.
            ConnectionConfig config = new ConnectionConfig();

            AddChanel(ref config, QosType.Reliable);
            AddChanel(ref config, QosType.UnreliableSequenced);

            // Topology configuration.
            HostTopology topology = new HostTopology(config, _settings.ConnectionsCount);

            // Set get port settings. If master use value for settings if not use first free port.
            int portToUse = _settings.ManagerType == NetworkManagerTypeEnum.Server ? _settings.Port : 0;
            hostID = NetworkTransport.AddHost(topology, portToUse);

            this.enabled = true;
        }

        public virtual void StartSession()
        {
            CreateMatch();
        }

        public virtual void StartGame()
        {
            LoadGameCallback.Invoke();
        }

        public virtual void JoinSession()
        {
            _settings.ManagerType = NetworkManagerTypeEnum.Server;

            matchSettings.NetworkMatch.ListMatches(0, 1, "", true, 0, 0, (success, info, matches) =>
            {
                if (success && matches.Count > 0)
                {
                    matchSettings.NetworkMatch.JoinMatch(matches[0].networkId, "", "", "", 0, 0, OnMatchJoined);
                }
            });
        }

        protected void AddChanel(ref ConnectionConfig connectionConfig, QosType type)
        {
            int channelId = connectionConfig.AddChannel(type);
            channelDictionary.Add(type, channelId);
        }

        protected override void Awake()
        {
            base.Awake();

            matchSettings.NetworkMatch = gameObject.AddComponent<NetworkMatch>();

            this.enabled = false;

            // Make sure if connected pears list is empty
            connectedPeers.Clear();

            for (int i = 0; i < _messageHandlersList.Count; i++)
            {
                _messageHandlersDictionary.Add(
                    _messageHandlersList[i].MessageID,
                    _messageHandlersList[i]);
            }

            for (int i = 0; i < _messageSendersList.Count; i++)
            {
                _messageSendersDictionary.Add(
                    _messageSendersList[i].MessageID,
                    _messageSendersList[i]);
            }
        }

        protected virtual void OnDestroy() {}

        protected virtual void OnApplicationQuit()
        {
            NetworkTransport.Shutdown();
        }

        private void Reset()
        {
            if (_messageHandlersHolder == null)
            {
                _messageHandlersHolder = new GameObject();
                _messageHandlersHolder.transform.SetParent(this.transform);
            }

            if (_messageSendersHolder == null)
            {
                _messageSendersHolder = new GameObject();
                _messageSendersHolder.transform.SetParent(this.transform);
            }
        }

        protected virtual void ClientConnected(int connectionId)
        {
            if(IsSever)
                ClientConnectedCallback.Invoke(connectionId);
        }

        protected virtual void ClientDisconnected(int connectionID)
        {
            if (IsSever)
                ClientDisconnectedCallback.Invoke(connectionID);
            
        }

        protected virtual void HandleConnection()
        {
            NetworkID networkID;
            NodeID node;

            NetworkTransport.GetConnectionInfo(
                hostID,
                connectionID,
                out address,
                out port,
                out networkID,
                out node,
                out error);

            string log = string.Format(
                NetworkManagerLogs.NEW_CONNECTION_APPEARED,
                NetworkUtility.GetIPAdress(address),
                port,
                System.DateTime.Now.ToString());

            _logs.Add(log);

            NetworkError networkError = NetworkUtility.GetNetworkError(error);
            if (networkError == NetworkError.Ok)
            {
                newPear = new ConnectionInfo(connectionID, address, port);

                log = string.Format(
                    NetworkManagerLogs.CONNECTING_TO_PEER_SUCCEEDED,
                    newPear.IPAdres,
                    newPear.Port,
                    System.DateTime.Now.ToString());

                _logs.Add(log);

                connectedPeers.Add(newPear);

                ClientConnected(newPear.ConnectionID);
            }
        }

        protected virtual void HandleDisconnection()
        {
            for (int i = 0; i < connectedPeers.Count; i++)
            {
                ConnectionInfo info = connectedPeers[i];
                if (info.ConnectionID == connectionID)
                {
                    connectedPeers.RemoveAt(i);
                    break;
                }
            }

            ClientDisconnected(connectionID);
        }

        protected virtual void HandleMessages(byte[] buffer, int size)
        {
            int messageId = receiveBuffer[0];
            BaseMessageHandler baseMessageHandler = null;
            if (_messageHandlersDictionary.TryGetValue(messageId, out baseMessageHandler))
            {
                baseMessageHandler.HandleMessage(receiveBuffer, dataSize, connectionID);
            }
        }

        protected virtual NetworkError ConnectToPear(ref ConnectionInfo peer)
        {
            peer.ConnectionID = NetworkTransport.Connect(
                hostID,
                peer.IPAdres,
                peer.Port,
                0,
                out error);

            return NetworkUtility.GetNetworkError(error);
        }

        protected virtual void SendMessage(int messageID, int connectionID = -1)
        {
            BaseMessageSender baseMessageSender = null;
            if(_messageSendersDictionary.TryGetValue(messageID, out baseMessageSender))
            {
                if(connectionID > -1)
                {
                    baseMessageSender.SendMessage(connectionID);
                }
                else
                {
                    baseMessageSender.SendMessage();
                }
            }
        }

        public virtual void Broadcast(bool reliable = true, bool update = false)
        {
            if (Settings.ManagerType == NetworkManagerTypeEnum.Client)
                return;

            if (update)
            {
                if (reliable)
                {

                }
                else
                {
                    UpdateForAllUnreiable(receiveBuffer, connectionID, dataSize);
                }
            }
            else
            {
                if (reliable)
                {
                    SendToAllReliable(receiveBuffer, connectionID, dataSize);
                }
                else
                {

                }
            }
        }

        public virtual void SendToAllReliable(byte[] message, int skipConnectionID = -1, int size = 0)
        {
            for (int i = 0; i < connectedPeers.Count; i++)
            {
                if (connectedPeers[i].ConnectionID == skipConnectionID)
                {
                    continue;
                }

                SendReliable(message, connectedPeers[i].ConnectionID, size);
            }
        }

        public virtual NetworkError SendReliable(byte[] message, int connectionId, int size = 0)
        {
            NetworkTransport.Send(
                hostID,
                connectionId,
                channelDictionary[QosType.Reliable],
                message,
                size > 0 ? size : message.Length,
                out error);

            NetworkError networkError = NetworkUtility.GetNetworkError(error);

            return networkError;
        }

        public virtual void UpdateForAllUnreiable(byte[] message, int skipConnectionID = -1, int size = 0)
        {
            for (int i = 0; i < connectedPeers.Count; i++)
            {
                if(connectedPeers[i].ConnectionID == skipConnectionID)
                {
                    continue;
                }

                UpdateUnreiable(message, connectedPeers[i].ConnectionID, size);
            }
        }

        public virtual NetworkError UpdateUnreiable(byte[] message, int connectionId, int size = 0)
        {
            if(message == null || message.Length == 0)
            {
                return NetworkError.Ok;
            }

            NetworkTransport.Send(
                hostID,
                connectionId,
                channelDictionary[QosType.UnreliableSequenced],
                message,
                size > 0 ? size : message.Length,
                out error);

            NetworkError networkError = NetworkUtility.GetNetworkError(error);

            return networkError;
        }

        protected virtual void Update()
        {
            if (hostID == -1)
            {
                this.enabled = false;
                return;
            }

            receiveBuffer = new byte[_settings.BufferSize];

            // Get events from the relay connection
            NetworkEventType networkEvent = NetworkTransport.ReceiveRelayEventFromHost(hostID, out error);

            if (networkEvent == NetworkEventType.ConnectEvent)
                Debug.Log("Relay server connected");

            if (networkEvent == NetworkEventType.DisconnectEvent)
                Debug.Log("Relay server disconnected");

            do
            {
                // Get events from the server/client game connection
                networkEvent = NetworkTransport.ReceiveFromHost(
                    hostID,
                    out connectionID,
                    out channelID,
                    receiveBuffer,
                    receiveBuffer.Length,
                    out dataSize,
                    out error);

                if ((NetworkError)error != NetworkError.Ok)
                {
                    Debug.LogError("Error while receiving network message: " + (NetworkError)error);
                }

                switch (networkEvent)
                {
                    case NetworkEventType.ConnectEvent:
                        HandleConnection();
                        break;

                    case NetworkEventType.DataEvent:
                        HandleMessages(receiveBuffer, dataSize);
                        break;

                    case NetworkEventType.DisconnectEvent:
                        HandleDisconnection();
                        break;

                    case NetworkEventType.Nothing:
                        break;
                }
            }
            while (networkEvent != NetworkEventType.Nothing);
        }

        // Match making 
        public virtual void CreateMatch()
        {
            if (matchSettings.NetworkMatch != null)
            {
                matchSettings.NetworkMatch.CreateMatch(
                    matchSettings.MatchName,
                    matchSettings.MatchSize,
                    matchSettings.MatchAdresatice,
                    "",
                    "",
                    "",
                    0,
                    0,
                    OnMatchCreate);
            }
        }

        public virtual void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            if (success)
            {
                Debug.Log("Create match succeeded");
                Utility.SetAccessTokenForNetwork(matchInfo.networkId, matchInfo.accessToken);

                matchSettings.MatchCreated = true;
                this.matchInfo = matchInfo;

                StartServer(
                    matchInfo.address,
                    matchInfo.port,
                    matchInfo.networkId,
                    matchInfo.nodeId);
            }
            else
            {
                Debug.LogError("Create match failed: " + extendedInfo);
            }
        }

        public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
        {
            if (success && matches != null)
            {
                matchList = matches;
            }
            else if (!success)
            {
                Debug.LogError("List match failed: " + extendedInfo);
            }
        }

        // When we've joined a match we connect to the server/host
        public virtual void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            if (success)
            {
                Debug.Log("Join match succeeded");
                Utility.SetAccessTokenForNetwork(matchInfo.networkId, matchInfo.accessToken);

                matchSettings.MatchJoined = true;
                this.matchInfo = matchInfo;

                Debug.Log(
                    "Connecting to Address:" + matchInfo.address +
                    " Port:" + matchInfo.port +
                    " NetworKID: " + matchInfo.networkId +
                    " NodeID: " + matchInfo.nodeId);

                ConnectThroughRelay(
                    matchInfo.address,
                    matchInfo.port,
                    matchInfo.networkId,
                    matchInfo.nodeId);
            }
            else
            {
                Debug.LogError("Join match failed: " + extendedInfo);
            }
        }

        public virtual void StartServer(string relayIP, int relayPort, NetworkID networkID, NodeID nodeID)
        {
            _settings.ManagerType = NetworkManagerTypeEnum.Server;
            Initialize();

            SourceID sourceID = Utility.GetSourceID();
            NetworkTransport.ConnectAsNetworkHost(
                hostID,
                relayIP,
                relayPort,
                networkID,
                sourceID,
                nodeID,
                out error);
        }

        public virtual void ConnectThroughRelay(string relayIP, int relayPort, NetworkID networkID, NodeID nodeID)
        {
            _settings.ManagerType = NetworkManagerTypeEnum.Client;
            Initialize();

            SourceID sourceID = Utility.GetSourceID();
            NetworkTransport.ConnectToNetworkPeer(
                hostID,
                relayIP,
                relayPort,
                0,
                0,
                networkID,
                sourceID,
                nodeID,
                out error);
        }

        public void AddNewMessageHandler(Type type)
        {
            BaseMessageHandler handler = _messageHandlersHolder.AddComponent(type) as BaseMessageHandler;

            _messageHandlersList.Add(handler);
        }

        public void AddNewMessageSender(Type type)
        {
            BaseMessageSender sender = _messageSendersHolder.AddComponent(type) as BaseMessageSender;

            _messageSendersList.Add(sender);
        }

        public void AddNetworkUpdater(BaseNetworkUpdater updater)
        {
            _networkUpdaterList.Add(updater);
        }

        public void RemoveNetworkUpdater(BaseNetworkUpdater updater)
        {
            int index = _networkUpdaterList.IndexOf(updater);
            _networkUpdaterList.RemoveAt(index);
        }
    }

    [Serializable] public sealed class ConnectionEvent : UnityEvent<int> {}
}
