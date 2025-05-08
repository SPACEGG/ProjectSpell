using System;
using Common.Data;
using UnityEngine;

namespace Common.Components
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private HealthData healthData;

        public event Action<float> OnHealthChanged;
        public event Action OnDeath;
        public event Action<float> OnDamageTaken;
        public event Action<float> OnHealed;

        public float CurrentHealth { get; private set; }
        public float MaxHealth => healthData.MaxHealth;
        public float HealthPercentage => CurrentHealth / healthData.MaxHealth;

        private void Awake()
        {
            if (healthData == null)
            {
                Debug.LogError($"HealthData is not assigned to {gameObject.name}'s Health component!");
                return;
            }

            CurrentHealth = healthData.MaxHealth;
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
            CurrentHealth = Mathf.Min(healthData.MaxHealth, CurrentHealth + amount);

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
            CurrentHealth = healthData.MaxHealth;
            OnHealthChanged?.Invoke(CurrentHealth);
        }
    }
}