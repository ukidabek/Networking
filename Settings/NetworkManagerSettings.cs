﻿using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Networking
{
    [Serializable]
    public class NetworkManagerSettings : BaseNetworkManagerSettings
    {
        [SerializeField]
        protected NetworkManagerTypeEnum _pearType = NetworkManagerTypeEnum.Server;
        public NetworkManagerTypeEnum ManagerType
        {
            get { return _pearType; }
            set { _pearType = value; }
        }
    }
}