using Unity.Netcode;

namespace Common.Models
{
    public struct Wav : INetworkSerializable
    {
        public byte[] Value { get; }

        public Wav(byte[] value)
        {
            Value = value;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsWriter)
            {
                serializer.GetFastBufferWriter().WriteValueSafe(Value);
            }
            else
            {
                serializer.GetFastBufferReader().ReadValueSafe(out byte[] value);
                this = new Wav(value);
            }
        }
    }
}