namespace Common.Models
{
    public readonly struct Wav
    {
        public byte[] Value { get; }

        public Wav(byte[] value)
        {
            Value = value;
        }
    }
}