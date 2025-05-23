using System;
using Common.Data;
using Unity.Netcode;

namespace Common.Models
{
    public struct NewNetworkHealthModel : INetworkSerializable, IEquatable<NewNetworkHealthModel>
    {
        public float MaxHealth;

        public float CurrentHealth;

        public float HealthPercentage => CurrentHealth / MaxHealth;

        public NewNetworkHealthModel(HealthData healthData)
        {
            MaxHealth = healthData.MaxHealth;
            CurrentHealth = MaxHealth;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref MaxHealth);
            serializer.SerializeValue(ref CurrentHealth);
        }

        public bool Equals(NewNetworkHealthModel other)
        {
            return MaxHealth.Equals(other.MaxHealth) && CurrentHealth.Equals(other.CurrentHealth);
        }
    }
}