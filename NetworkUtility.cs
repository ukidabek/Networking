using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;


namespace BaseGameLogic.Networking
{
    public static class NetworkUtility
    {
        private static readonly char[] IP_ADRES_SEPARATORS = { ':' };

        public static string GetIPAdress(string ipAdressString)
        {
            string[] ipAdresParts = ipAdressString.Split(IP_ADRES_SEPARATORS);
            int ipAdresIndex = ipAdresParts.Length - 1;
            return ipAdresParts[ipAdresIndex];
        }

        public static NetworkError GetNetworkError(byte error)
        {
            return (NetworkError)error;
        }

        public static T ConvertBytesToObject<T>(byte[] array, int start = 0, int length = 0)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream(array, start, length > 0 ? length - start : array.Length - start);

            T objectFormBytes = (T)binaryFormatter.Deserialize(memoryStream);

            return objectFormBytes;
        }

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