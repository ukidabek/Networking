using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Networking
{
    /// <summary>
    /// Helper class defining rate of updates pro network updators.  
    /// </summary>
    [SerializeField]
    public class UpdateRate
    {
        /// <summary>
        /// Frequency of objects
        /// </summary>
        [SerializeField] private int _frequency = 60;

        /// <summary>
        /// Interval of updates in seckends. 
        /// </summary>
        public float Interval { get { return 1f / _frequency; } }
    }
}