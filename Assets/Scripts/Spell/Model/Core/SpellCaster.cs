using Spell.Model.Data;
using UnityEngine;

namespace Spell.Model.Core
{
    public class SpellCaster : MonoBehaviour
    {
        [SerializeField] private Transform castOrigin;

        public Transform CastOrigin => castOrigin != null ? castOrigin : transform;

        public void CastSpell(SpellData data)
        {
            if (data == null)
            {
                Debug.LogWarning("Tried to cast null spell.");
                return;
            }

            // SpellData에서 PositionOffset을 가져와서 사용 (null이면 Vector3.zero)
            var offset = data.PositionOffset ?? Vector3.zero;
            var spawnPosition = CastOrigin.position + offset;
            
            var spell = SpellFactory.CreateSpellGameObject(data);

            spell.transform.position = spawnPosition;
            spell.Activate(spawnPosition, transform);
        }
    }
}