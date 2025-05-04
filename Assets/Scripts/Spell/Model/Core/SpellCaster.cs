using Spell.Model.Data;
using UnityEngine;

namespace Spell.Model.Core
{
    public class SpellCaster : MonoBehaviour
    {
        [SerializeField] private Transform castOrigin;

        private Transform CastOrigin => castOrigin != null ? castOrigin : transform;

        public void CastSpell(SpellData data, Vector3 spawnPosition)
        {
            if (data == null)
            {
                Debug.LogWarning("Tried to cast null spell.");
                return;
            }

            var spell = SpellFactory.CreateSpellGameObject(data);

            spell.transform.position = CastOrigin.position;
            spell.Activate(spawnPosition, transform);
        }
    }
}