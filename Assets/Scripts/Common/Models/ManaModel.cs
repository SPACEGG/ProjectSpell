using System;
using Common.Data;
using UnityEngine;

namespace Common.Models
{
    public record ManaModel
    {
        public event Action<float> OnManaChanged;

        public float ManaPerLevel { get; set; }
        public float MaxMana { get; private set; }
        public float CurrentMana { get; private set; }

        public float ManaRegenPerSec { get; private set; }
        public float ManaRegenDelay { get; private set; }

        private float _manaRegenTimer = 0f;


        public ManaModel(ManaData manaData)
        {
            ManaPerLevel = manaData.ManaPerLevel;
            MaxMana = ManaPerLevel * 3;
            CurrentMana = 0;

            ManaRegenPerSec = manaData.ManaRegenPerSec;
            ManaRegenDelay = manaData.ManaRegenDelay;
        }

        public void GetMana(float amount)
        {
            if (amount <= 0) return;
            CurrentMana = Math.Min(MaxMana, CurrentMana + amount);
            OnManaChanged?.Invoke(CurrentMana);
        }

        public bool UseMana(int level)
        {
            if (level <= 0) return false;
            bool isValidLevel = CurrentMana - level * ManaPerLevel >= 0;

            CurrentMana = Math.Max(0, CurrentMana - level * ManaPerLevel);
            OnManaChanged?.Invoke(CurrentMana);

            return isValidLevel;
        }

        public void RegenerateMana(float deltaTime)
        {
            if (CurrentMana >= MaxMana) return;

            _manaRegenTimer += deltaTime;

            if (_manaRegenTimer >= ManaRegenDelay)
            {
                CurrentMana = Math.Min(MaxMana, CurrentMana + ManaRegenPerSec * ManaRegenDelay);
                _manaRegenTimer = 0f;
                OnManaChanged?.Invoke(CurrentMana);
            }
        }
    }
}