using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace BaseGameLogic.Networking
{
    public abstract class BaseNetworkUpdater : BaseMessageSender
    {
        [SerializeField, Range(0f, 1f)]
        private float _updateRate = 0.1f;

        [SerializeField]
        private float _updateCounter = 0;

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