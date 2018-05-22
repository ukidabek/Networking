using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Networking
{
    /// <summary>
    /// Network manager settings.
    /// </summary>
    [Serializable]
    public class NetworkManagerSettings : BaseNetworkManagerSettings
    {
        /// <summary>
        /// Define if manager work as a server or client.
        /// </summary>
        [SerializeField] protected NetworkManagerTypeEnum _pearType = NetworkManagerTypeEnum.Server;
        public NetworkManagerTypeEnum ManagerType
        {
            get { return _pearType; }
            set { _pearType = value; }
        }
    }
}