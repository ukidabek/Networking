using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking.Match;

namespace Networking
{
    /// <summary>
    /// Settings used to handle match.
    /// </summary>
    [Serializable]
    public class MatchSettings
    {
        /// <summary>
        /// Name of the match
        /// </summary>
        [SerializeField] private string matchName = "NewRoom";
        public string MatchName
        {
            get { return matchName; }
            set { matchName = value; }
        }


        /// <summary>
        /// Maximum players that can join to match.
        /// </summary>
        [SerializeField] private uint matchSize = 8;
        public uint MatchSize
        {
            get { return matchSize; }
            set { matchSize = value; }
        }
        
        /// <summary>
        /// Defines if match is private or not.
        /// </summary>
        [SerializeField] private bool matchAdresatice = true;
        public bool MatchAdresatice
        {
            get { return matchAdresatice; }
            set { matchAdresatice = value; }
        }


        /// <summary>
        /// Match creation status.  
        /// </summary>
        [SerializeField] private bool matchCreated;
        public bool MatchCreated
        {
            get { return matchCreated; } 
            set { matchCreated = value; }
        }

        /// <summary>
        /// Join match status.
        /// </summary>
        [SerializeField] private bool matchJoined;
        public bool MatchJoined
        {
            get { return matchJoined; } 
            set { matchJoined = value; }
        }

        /// <summary>
        /// Network match instance.
        /// </summary>
        [SerializeField] private NetworkMatch networkMatch;
        public NetworkMatch NetworkMatch
        {
            get { return networkMatch; }
            set { networkMatch = value; }
        }
    }
}