using System;
using Common.Data;
using Common.Models;
using Gameplay.UI.Multiplay;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class NetworkHealthManaManager : NetworkBehaviour, INetworkHealthProvider, IManaProvider
    {
        [SerializeField]
        private HealthData healthData;
        [SerializeField]
        private ManaData manaData;

        public event EventHandler<OnHealthChangedEventArgs> OnHealthChanged;
        public event EventHandler OnPlayerDied;

        public event Action OnLocalPlayerDied;

        public class OnHealthChangedEventArgs : EventArgs
        {
            public float CurrentHealth;
            public float MaxHealth;
        }

        public NetworkVariable<NewNetworkHealthModel> HealthModel { get; private set; }
        public ManaModel ManaModel { get; private set; }

        private void Awake()
        {
            HealthModel = new NetworkVariable<NewNetworkHealthModel>(new NewNetworkHealthModel(healthData));
            ManaModel = new ManaModel(manaData);

            HealthModel.OnValueChanged += (_, current) =>
            {
                OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs
                {
                    CurrentHealth = current.CurrentHealth,
                    MaxHealth = current.MaxHealth
                });
                if (current.CurrentHealth <= 0)
                {
                    HandleDeath();
                    OnPlayerDied?.Invoke(this, EventArgs.Empty);
                }
            };
        }

        private void HandleDeath()
        {
            if (IsLocalPlayer)
            {
                Debug.Log("HandleDeath() 호출");
                OnLocalPlayerDied?.Invoke();
                GameEndUi.Singleton.ShowGameLoseUi();
            }
        }

        private void FixedUpdate()
        {
            ManaModel.RegenerateMana(Time.deltaTime);
        }

        public void TakeDamage(float damage)
        {
            if (IsServer)
            {
                HealthModel.Value = new NewNetworkHealthModel()
                {
                    MaxHealth = HealthModel.Value.MaxHealth,
                    CurrentHealth = Mathf.Clamp(HealthModel.Value.CurrentHealth - damage, 0, HealthModel.Value.MaxHealth)
                };
            }
        }

        public void Heal(float contextValue)
        {
            if (IsServer)
            {
                HealthModel.Value = new NewNetworkHealthModel()
                {
                    MaxHealth = HealthModel.Value.MaxHealth,
                    CurrentHealth = Mathf.Clamp(HealthModel.Value.CurrentHealth + contextValue, 0, HealthModel.Value.MaxHealth)
                };
            }
        }
    }
}