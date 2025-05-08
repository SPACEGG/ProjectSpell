using UnityEngine;

namespace Common.Data
{
    [CreateAssetMenu(fileName = "HealthData", menuName = "Scriptable Objects/Common/Health/Health Data")]
    public class HealthData : ScriptableObject
    {
        [Header("Health Settings")] [SerializeField]
        private float maxHealth = 100f;

        public float MaxHealth => maxHealth;
    }
}