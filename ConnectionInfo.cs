using UnityEngine;
using UnityEngine.Networking.Types;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BaseGameLogic.Networking
{
    [Serializable]
    public class ConnectionInfo
    {
        [SerializeField]
        private int _port = 0;
        public int Port
        {
            get { return _port; }
        }

        [SerializeField]
        private string _ipAdres = string.Empty;
        public string IPAdres
        {
            get { return _ipAdres; }
        }

        [SerializeField]
        private int _connectionID = -1;
        public int ConnectionID
        {
            get { return _connectionID; }
            set { _connectionID = value; }
        }

        public ConnectionInfo(int connectionID, string ipAdress, int port)
        {
            _connectionID = connectionID;
            _ipAdres = NetworkUtility.GetIPAdress(ipAdress);
            _port = port;
        } 
    }
}