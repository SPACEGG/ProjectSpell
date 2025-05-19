using Spell.Model.Enums;
using Unity.Netcode;

namespace Spell.Model.Data
{
    public record SpellActionData : INetworkSerializable
    {
        public ActionType Action;
        public TargetType Target;
        public float Value;

        public SpellActionData() { }

        public SpellActionData(ActionType action, TargetType target, float value)
        {
            Action = action;
            Target = target;
            Value = value;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Action);
            serializer.SerializeValue(ref Target);
            serializer.SerializeValue(ref Value);
        }
    }
}