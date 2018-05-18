using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace Networking
{
    public abstract class BaseNetworkUpdater : BaseMessageSender
    {
        [SerializeField] protected virtual bool UpdateAutomatically { get { return true; } }

        [SerializeField] protected int _updateRate = 60;

        protected float _updateRateInterval = 0.1f;

        [SerializeField] protected float _updateCounter = 0;

        private void Awake()
        {
            _updateRateInterval = 1f / _updateRate;
            enabled = UpdateAutomatically;
        }

        protected virtual void Start()
        {
            if(NetworkManagerInstance == null)
            {
                enabled = false;
            }
            else
            {
                NetworkManagerInstance.AddNetworkUpdater(this);
            }

        }

        protected virtual void OnDestroy()
        {
            if (NetworkManagerInstance != null)
            {
                NetworkManagerInstance.RemoveNetworkUpdater(this);
            }
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
                _updateCounter = _updateRateInterval;
            }
            else
                _updateCounter -= Time.deltaTime;
        }
    }
}