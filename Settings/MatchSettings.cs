using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking.Match;

namespace BaseGameLogic.Networking
{
    [Serializable]
    public class MatchSettings
    {
        [SerializeField]
        private string matchName = "NewRoom";
        public string MatchName
        {
            get { return matchName; }
            set { matchName = value; }
        }

        [SerializeField]
        private uint matchSize = 8;
        public uint MatchSize
        {
            get { return matchSize; }
            set { matchSize = value; }
        }
        
        [SerializeField]
        private bool matchAdresatice = true;
        public bool MatchAdresatice
        {
            get { return matchAdresatice; }
            set { matchAdresatice = value; }
        }


        [SerializeField]
        private bool matchCreated;
        public bool MatchCreated
        {
            get { return matchCreated; } 
            set { matchCreated = value; }
        }

        [SerializeField]
        private bool matchJoined;
        public bool MatchJoined
        {
            get { return matchJoined; } 
            set { matchJoined = value; }
        }

        [SerializeField]
        private NetworkMatch networkMatch;
        public NetworkMatch NetworkMatch
        {
            get { return networkMatch; }
            set { networkMatch = value; }
        }

    }
}