using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BaseGameLogic.Networking
{
    [Serializable]
    public class BroadcastCredentials
    {
        [SerializeField]
        private int _key = 1;
        public int Key { get { return _key; } }

        [SerializeField]
        private int _version = 1;
        public int Version { get { return _version; } }

        [SerializeField]
        private int _subversion = 1;
        public int Subversion { get { return _subversion; } }

        [SerializeField, Tooltip("Time between broadcast messages in ms.")]
        private int _timeout = 3000;
        public int Timeout { get { return _timeout; } }
    }
}