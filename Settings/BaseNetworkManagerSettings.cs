using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BaseGameLogic.Networking
{
    [Serializable]
    public class BaseNetworkManagerSettings
    {
        [SerializeField]
        private int _port = 8888;
        public int Port { get { return _port; } }

        [SerializeField]
        protected int _connectionsCount = 8;
        public int ConnectionsCount { get { return _connectionsCount; } }

        [SerializeField]
        private int _bufferSize = 1024;
        public int BufferSize
        {
            get { return _bufferSize; }
        }
    }
}