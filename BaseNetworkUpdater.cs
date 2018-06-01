using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Networking
{
    public abstract class BaseNetworkUpdater : BaseMessageSender
    {
        [SerializeField] protected virtual bool UpdateAutomatically { get { return true; } }

        [Serializable]
        protected class UpdateRate
        {
            public int Rate = 60;
            public float UpdateRateInterval = 0.1f;

            public void CalculateInterval()
            {
                UpdateRateInterval = 1f / Rate;
            }
        }

        [SerializeField] protected UpdateRate updateRate = new UpdateRate();

        [SerializeField] protected float _updateCounter = 0;

        protected override void Awake()
        {
            updateRate.CalculateInterval();
            enabled = UpdateAutomatically;
        }

        protected virtual void Start()
        {
            if(NetworkManagerInstance == null)
                enabled = false;
            else
                NetworkManagerInstance.AddNetworkUpdater(this);
        }

        protected virtual void OnDestroy()
        {
            if (NetworkManagerInstance != null)
                NetworkManagerInstance.RemoveNetworkUpdater(this);
        }

        protected virtual void Update()
        {
            if(!UpdateAutomatically)
            {
                enabled = UpdateAutomatically;
                return;
            }

            if (_updateCounter <= 0)
            {
                SendMessage();
                _updateCounter = updateRate.UpdateRateInterval;
            }
            else
                _updateCounter -= Time.deltaTime;
        }
    }
}