using Spell.Model.Data;
using UnityEngine;

namespace Spell.Model.Core
{
    public class SpellCaster : MonoBehaviour
    {
        [SerializeField] private Transform castOrigin;  // 지팡이 끝부분

        public Transform CastOrigin => castOrigin != null ? castOrigin : Camera.main.transform; // 여기서 transform 사용

        public void CastSpell(SpellData data, GameObject caster)
        {
            if (data == null)
            {
                Debug.LogWarning("Tried to cast null spell.");
                return;
            }

            // SpellData에서 PositionOffset을 가져와서 사용 (null이면 2칸 앞)
            var offset = data.PositionOffset ?? new Vector3(0f, 0f, 2f);
            var spawnPosition = CastOrigin.position + offset;

            // SpellData 정보를 바탕으로 실제 동작/외형이 적용된 스펠 오브젝트 씬에 생성
            var spellBehavior = SpellFactory.CreateSpellGameObject(data, caster);
            spellBehavior.transform.SetPositionAndRotation(spawnPosition, CastOrigin.rotation);
            spellBehavior.Behave(data);
            // 이 이후에 spellBehavior의 Start()가 실행됨
        }
    }
}