using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace Networking
{
    /// <summary>
    /// Base abstract class used to create and send message.
    /// </summary>
    public abstract class BaseMessageSender : MonoBehaviour
    {
        /// <summary>
        /// Network manager instance.
        /// </summary>
        protected BaseNetworkManager NetworkManagerInstance { get { return BaseNetworkManager.Instance; } }

        /// <summary>
        /// Defines how message will be handler. 
        /// </summary>
        [SerializeField] private SendType SendType = SendType.SendReliable;

        /// <summary>
        /// Id that will be used to identify a message.   
        /// </summary>
        public abstract int MessageID { get; }

        /// <summary>
        /// Generate message bytes.
        /// </summary>
        /// <returns>Byte array of message</returns>
        protected abstract byte[] GetMessageBytes();

        /// <summary>
        /// Generate message bytes.
        /// </summary>
        /// <returns>Byte array of message</returns>
        protected abstract byte[] GetMessageBytes(int connectionID);

        protected virtual byte[] CombineMessageWithId(int id, byte[] message)
        {
            List<byte> combinedMessage = new List<byte>();
            combinedMessage.Add((byte)id);
            combinedMessage.AddRange(message);

            return combinedMessage.ToArray();
        }

        /// <summary>
        /// Send message to all connections. 
        /// </summary>
        public void SendMessage()
        {
            byte[] message = CombineMessageWithId(MessageID, GetMessageBytes());

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

        /// <summary>
        /// Send message to specified connection.
        /// </summary>
        /// <param name="connectionID">Destination connection ID</param>
        public void SendMessage(int connectionID)
        {
            byte[] message = CombineMessageWithId(MessageID, GetMessageBytes(connectionID));

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