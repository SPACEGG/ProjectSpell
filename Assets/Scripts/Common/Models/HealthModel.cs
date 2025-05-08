using System;
using UnityEngine;

namespace Common.Models
{
    public record HealthModel
    {
        public event Action<float> OnHealthChanged;

        public event Action OnDeath;

        public event Action<float> OnDamageTaken;

        public event Action<float> OnHealed;

        public float MaxHealth { get; private set; }

        public float CurrentHealth { get; private set; }

        public float HealthPercentage => CurrentHealth / MaxHealth;

        public HealthModel(float maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
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
    }
}