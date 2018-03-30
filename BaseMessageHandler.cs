using UnityEngine;

namespace BaseGameLogic.Networking
{
    public abstract class BaseMessageHandler : MonoBehaviour
    { 
        public abstract int MessageID { get; }

        public abstract void HandleMessage(byte[] message, int messageSize, int connectionID);
    }
}