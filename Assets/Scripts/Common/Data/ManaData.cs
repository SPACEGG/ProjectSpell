using UnityEngine;
using UnityEngine.Serialization;

namespace Common.Data
{
    [CreateAssetMenu(fileName = "ManaData", menuName = "Scriptable Objects/Common/Mana/Mana Data")]
    public class ManaData : ScriptableObject
    {
        [FormerlySerializedAs("maxMana")] [Header("Mana Settings")] [SerializeField]
        private float manaPerLevel = 100f;

        [FormerlySerializedAs("manaRegenRate")] [SerializeField]
        private float manaRegenPerSec = 5f;

        [SerializeField] private float manaRegenDelay = 1f;

        public float ManaPerLevel => manaPerLevel;
        public float ManaRegenPerSec => manaRegenPerSec;
        public float ManaRegenDelay => manaRegenDelay;
    }
}