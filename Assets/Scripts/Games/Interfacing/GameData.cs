using JetBrains.Annotations;
using Unity.Netcode;


namespace Lobby
{
    /// <summary>
    /// Handles networking of Game Data in the lobby over JSON-strings
    /// </summary>
    public class NetworkGameData : NetworkBehaviour
    {
        private NetworkVariable<Netstring> dataString = new NetworkVariable<Netstring>(new Netstring()
        {
            st = ""
        });

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Returns dataString
        /// </summary>
        /// <returns></returns>
        public NetworkVariable<Netstring> GetDataString()
        {
            return dataString;
        }

    }


    /// <summary>
    /// String that can be sent over a network
    /// </summary>
    public struct Netstring : INetworkSerializable, System.IEquatable<Netstring>
    {
        /// <summary>
        /// String Value
        /// </summary>
        public string st;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out st);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(st);
            }
        }

        /// <summary>
        /// Check if two Netstrings are equal
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals([CanBeNull] Netstring other)
        {
            if (string.Equals(a: other.st, b: st, System.StringComparison.CurrentCultureIgnoreCase)) return true;
            return false;
        }
    }


}

