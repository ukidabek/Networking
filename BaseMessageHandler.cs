using UnityEngine;

namespace Networking
{
    /// <summary>
    /// Base class used to handle incoming messages. 
    /// </summary>
    public abstract class BaseMessageHandler : MonoBehaviour
    { 
        /// <summary>
        /// ID of message that handler handle.
        /// </summary>
        public abstract int MessageID { get; }

        /// <summary>
        /// Coll to handle a message.
        /// </summary>
        /// <param name="message">Message bytes</param>
        /// <param name="messageSize">Message size</param>
        /// <param name="connectionID">Connection id that message originated from</param>
        public abstract void HandleMessage(byte[] message, int messageSize, int connectionID);
    }
}