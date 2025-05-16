using System;
using Common.Data;
using Unity.Netcode;
using UnityEngine;

namespace Common.Models
{
    public class NetworkHealthModel : INetworkSerializable, System.IEquatable<NetworkHealthModel>
    {
        public event Action<float> OnHealthChanged;

        public event Action OnDeath;

        public event Action<float> OnDamageTaken;

        public event Action<float> OnHealed;

        public float MaxHealth;

        public float CurrentHealth;

        public float HealthPercentage => CurrentHealth / MaxHealth;

        public NetworkHealthModel()
        {
        }

        public NetworkHealthModel(HealthData healthData)
        {
            MaxHealth = healthData.MaxHealth;
            CurrentHealth = MaxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (damage <= 0) return;

            CurrentHealth = Mathf.Max(0, CurrentHealth - damage);

            OnHealthChanged?.Invoke(CurrentHealth);
            OnDamageTaken?.Invoke(damage);

            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (amount <= 0) return;

            float previousHealth = CurrentHealth;
            CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);

            float actualHealAmount = CurrentHealth - previousHealth;
            if (actualHealAmount > 0)
            {
                OnHealthChanged?.Invoke(CurrentHealth);
                OnHealed?.Invoke(actualHealAmount);
            }
        }

        private void Die()
        {
            OnDeath?.Invoke();
        }

        public void ResetHealth()
        {
            CurrentHealth = MaxHealth;
            OnHealthChanged?.Invoke(CurrentHealth);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref MaxHealth);
            serializer.SerializeValue(ref CurrentHealth);
        }

        public bool Equals(NetworkHealthModel other)
        {
            if (other == null) return false;

            return MaxHealth.Equals(other.MaxHealth) && CurrentHealth.Equals(other.CurrentHealth);
        }
    }
}