using Spell.Model.Data;
using Spell.Model.Behaviors; 
using UnityEngine;

namespace Spell.Model.Core
{
    public class SpellCaster : MonoBehaviour
    {
        [SerializeField] private Transform castOrigin;  // 지팡이 끝부분

        public Transform CastOrigin => castOrigin != null ? castOrigin : transform; // 여기서 transform 사용

        public void CastSpell(SpellData data, GameObject caster)
        {
            if (data == null)
            {
                Debug.LogWarning("Tried to cast null spell.");
                return;
            }

            // SpellData에서 PositionOffset을 가져와서 사용 (null이면 Vector3.zero)
            var offset = data.PositionOffset; // Vector3 타입이므로 null 체크 불필요
            var spawnPosition = CastOrigin.position + offset;

            // SpellData 정보를 바탕으로 실제 동작/외형이 적용된 스펠 오브젝트 씬에 생성
            var spellObject = SpellFactory.CreateSpellGameObject(data);

            spellObject.transform.position = spawnPosition;

            // === 시전자와 주문 오브젝트가 충돌하지 않도록 처리 ===
            var casterColliders = GetComponentsInChildren<Collider>();
            var spellColliders = spellObject.GetComponentsInChildren<Collider>();
            foreach (var casterCol in casterColliders)
            {
                foreach (var spellCol in spellColliders)
                {
                    Physics.IgnoreCollision(spellCol, casterCol, true);
                }
            }

            var behavedSpellObject = spellObject.GetComponent<SpellBehaviorBase>();
            behavedSpellObject?.Behave(data);
        }
    }
}