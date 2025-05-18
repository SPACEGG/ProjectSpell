using System;
using Common.Data;
using Common.Models;
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

        public NetworkVariable<NetworkHealthModel> HealthModel { get; private set; }
        public ManaModel ManaModel { get; private set; }

        private void Awake()
        {
            HealthModel = new NetworkVariable<NetworkHealthModel>(new NetworkHealthModel(healthData));
            ManaModel = new ManaModel(manaData);
        }

        private void FixedUpdate()
        {
            ManaModel.RegenerateMana(Time.deltaTime);
        }
    }
}