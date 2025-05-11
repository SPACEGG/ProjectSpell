using Common.Data;
using Common.Models;
using UnityEngine;

namespace Player
{
    public class HealthManaManager : MonoBehaviour, IHealthProvider, IManaProvider
    {
        [SerializeField]
        private HealthData healthData;
        [SerializeField]
        private ManaData manaData;

        public HealthModel HealthModel { get; private set; }
        public ManaModel ManaModel { get; private set; }

        private void Awake()
        {
            HealthModel = new HealthModel(healthData);
            ManaModel = new ManaModel(manaData);
        }
    }
}