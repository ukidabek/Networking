using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace Networking
{
    public abstract class BaseNetworkUpdater : BaseMessageSender
    {
        [SerializeField]
        private int _updateRate = 60;

        private float _updateRateInterval = 0.1f;

        [SerializeField]
        private float _updateCounter = 0;

        private void Awake()
        {
            _updateRateInterval = 1 / _updateRate;
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
            if (_updateCounter <= 0)
            {
                SendMessage();
                _updateCounter = _updateRate;
            }
            else
            {
                _updateCounter -= Time.deltaTime;
            }
        }
    }
}