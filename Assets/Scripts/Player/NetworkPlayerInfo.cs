using System;
using Unity.Collections;
using Unity.Netcode;

namespace Player
{
    public struct NetworkPlayerInfo : INetworkSerializable, IEquatable<NetworkPlayerInfo>
    {
        public ulong ClientId;
        public int ColorId;
        public FixedString64Bytes Name;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref ColorId);
            serializer.SerializeValue(ref Name);
        }

        public bool Equals(NetworkPlayerInfo other)
        {
            return ClientId == other.ClientId &&
                   ColorId == other.ColorId &&
                   Name == other.Name;
        }
    }
}