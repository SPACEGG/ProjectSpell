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

        public event EventHandler OnPlayerDied;

        public NetworkVariable<NetworkHealthModel> HealthModel { get; private set; }
        public ManaModel ManaModel { get; private set; }

        private void Awake()
        {
            HealthModel = new NetworkVariable<NetworkHealthModel>(new NetworkHealthModel(healthData));
            ManaModel = new ManaModel(manaData);

            HealthModel.OnValueChanged += (_, current) =>
            {
                if (current.CurrentHealth <= 0)
                {
                    OnPlayerDied?.Invoke(this, EventArgs.Empty);
                }
            };
        }

        public override void OnNetworkSpawn()
        {
            if (IsLocalPlayer)
            {
                HealthModel.Value.OnDeath += HandleDeath;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsLocalPlayer)
            {
                HealthModel.Value.OnDeath -= HandleDeath;
            }
        }

        private void HandleDeath()
        {
            if (IsLocalPlayer)
            {
                GameEndUi.Singleton.ShowGameLoseUi();
            }
        }

        private void FixedUpdate()
        {
            ManaModel.RegenerateMana(Time.deltaTime);
        }
    }
}