using Spell.Model.Data;
using UnityEngine;

namespace Spell.Model.Core
{
    public class SpellCaster : MonoBehaviour
    {
        [SerializeField] private Transform castOrigin;  // 지팡이 끝부분

        public Transform CastOrigin => castOrigin != null ? castOrigin : transform; // 여기서 transform 사용

        public void CastSpell(SpellData data, Vector3 cameraTargetPosition)
        {
            if (data == null)
            {
                Debug.LogWarning("Tried to cast null spell.");
                return;
            }

            // SpellData에서 PositionOffset을 가져와서 사용 (null이면 Vector3.zero)
            var offset = data.PositionOffset ?? Vector3.zero;
            var spawnPosition = CastOrigin.position + offset;
            
            // SpellData 정보를 바탕으로 실제 동작/외형이 적용된 스펠 오브젝트 씬에 생성
            var spell = SpellFactory.CreateSpellGameObject(data);

            spell.transform.position = spawnPosition;
            spell.Behave(spawnPosition);

            // 예시: 주문 생성 시 cameraTargetPosition을 TextToSpellApi에 넘겨주는 코드가 필요하다면
            // await textToSpellApi.TextToSpellAsync(주문텍스트, powerLevel, cameraTargetPosition);
        }
    }
}