using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace BaseGameLogic.Networking
{
    internal enum SendType
    {
        SendReliable,
        SendUnreiable,
        UpdateReliable,
        UpdateUnreiable
    }

    public abstract class BaseMessageSender : MonoBehaviour
    {
        protected BaseNetworkManager NetworkManagerInstance
        {
            get { return BaseNetworkManager.Instance; }
        }

        [SerializeField]
        private SendType SendType = SendType.SendReliable;

        public abstract int MessageID { get; }

        protected abstract byte[] GetMessageBytes();
        protected abstract byte[] GetMessageBytes(int connectionID);

        public void SendMessage()
        {
            byte[] message = GetMessageBytes();
            switch (SendType)
            {
                case SendType.SendReliable:
                    NetworkManagerInstance.SendToAllReliable(message);
                    break;
                case SendType.SendUnreiable:
                    break;
                case SendType.UpdateReliable:
                    break;
                case SendType.UpdateUnreiable:
                    NetworkManagerInstance.UpdateForAllUnreiable(message);
                    break;
            }
        }

        public void SendMessage(int connectionID)
        {
            byte[] message = GetMessageBytes(connectionID);
            switch (SendType)
            {
                case SendType.SendReliable:
                    NetworkManagerInstance.SendReliable(message, connectionID);
                    break;
                case SendType.SendUnreiable:
                    break;
                case SendType.UpdateReliable:
                    break;
                case SendType.UpdateUnreiable:
                    NetworkManagerInstance.UpdateUnreiable(message, connectionID);
                    break;
            }
        }
    }
}