using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;


namespace Networking
{
    /// <summary>
    /// Network utilities class
    /// </summary>
    public static class NetworkUtility
    {
        private static readonly char[] IP_ADRES_SEPARATORS = { ':' };

        /// <summary>
        /// Returns IP address form string.
        /// </summary>
        /// <param name="ipAdressString">String contains IP</param>
        /// <returns></returns>
        public static string GetIPAdres(string ipAdressString)
        {
            string[] ipAdresParts = ipAdressString.Split(IP_ADRES_SEPARATORS);
            int ipAdresIndex = ipAdresParts.Length - 1;
            return ipAdresParts[ipAdresIndex];
        }

        /// <summary>
        /// Converts network error byte to network error. 
        /// </summary>
        /// <param name="error">Error byte</param>
        /// <returns>Network error</returns>
        public static NetworkError GetNetworkError(byte error)
        {
            return (NetworkError)error;
        }

        /// <summary>
        /// Converts byte array to object
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="array">Byte array.</param>
        /// <param name="start">Start index</param>
        /// <param name="length">Length of object array</param>
        /// <returns>Object</returns>
        public static T ConvertBytesToObject<T>(byte[] array, int start = 0, int length = 0)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream(array, start, length > 0 ? length - start : array.Length - start);

            T objectFormBytes = (T)binaryFormatter.Deserialize(memoryStream);

            return objectFormBytes;
        }

        /// <summary>
        /// Converts object to byte array
        /// </summary>
        /// <param name="objectToConvert">Object to convert</param>
        /// <returns>Bytes array</returns>
        public static byte[] ConvertObjectToBytes(object objectToConvert)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();

            binaryFormatter.Serialize(memoryStream, objectToConvert);
            byte[] array = memoryStream.ToArray();

            return array;
        }
    }
}