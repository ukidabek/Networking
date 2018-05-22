using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Networking
{
    /// <summary>
    /// Class containing base network settings.
    /// </summary>
    [Serializable]
    public class BaseNetworkManagerSettings
    {
        // Port used by application
        [SerializeField] private int _port = 8888;
        public int Port { get { return _port; } }

        // Maximum connections that can be handled by application. 
        [SerializeField] protected int _connectionsCount = 8;
        public int ConnectionsCount { get { return _connectionsCount; } }

        // Size of buffer that will store incoming massages
        [SerializeField] private int _bufferSize = 1024;
        public int BufferSize { get { return _bufferSize; } }
    }
}