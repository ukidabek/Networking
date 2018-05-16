using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace Networking
{
    public abstract class BaseNetworkUpdater : BaseMessageSender
    {
        [SerializeField] protected bool _updateAutomatically = true;

        [SerializeField] protected int _updateRate = 60;

        protected float _updateRateInterval = 0.1f;

        [SerializeField] protected float _updateCounter = 0;

        private void Awake()
        {
            _updateRateInterval = 1f / _updateRate;
            enabled = _updateAutomatically;
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
            if(!_updateAutomatically)
            {
                enabled = _updateAutomatically;
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