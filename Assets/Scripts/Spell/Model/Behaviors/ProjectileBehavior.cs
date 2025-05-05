using System.Collections;
using UnityEngine;

namespace Spell.Model.Behaviors
{
    public class ProjectileBehavior : SpellBehaviorBase
    {
        public override void Behave(Vector3 spawnPosition)
        {
            int count = Data.Count > 0 ? Data.Count : 1;
            for (int i = 0; i < count; i++)
            {
                SpawnProjectile(spawnPosition, i, count);
            }
        }

        private void SpawnProjectile(Vector3 spawnPosition, int index, int totalCount)
        {
            // 방향
            Vector3 direction = Data.Direction ?? Vector3.forward;
            direction = direction.normalized;

            // 여러 개일 때 퍼뜨리기(간단 예시, 필요시 수정)
            if (totalCount > 1)
            {
                float angleStep = 10f; // 각도 간격
                float angle = (index - (totalCount - 1) / 2f) * angleStep;
                direction = Quaternion.Euler(0, angle, 0) * direction;
            }

            // 위치
            transform.position = spawnPosition;

            // Rigidbody 추가 및 중력 적용
            var rb = GetComponent<Rigidbody>();
            if (rb == null)
                rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = Data.HasGravity;

            // 속도, 지속시간
            float speed = Data.Speed > 0 ? Data.Speed : 30f;
            float duration = Data.Duration > 0 ? Data.Duration : 3f;

            rb.linearVelocity = direction * speed;

            StartCoroutine(DestroyAfterSeconds(duration));
        }

        private IEnumerator DestroyAfterSeconds(float duration)
        {
            yield return new WaitForSeconds(duration);
            Destroy(gameObject);
        }
    }
}

// 이 코드는 ProjectileBehavior(투사체 스펠 동작) 클래스입니다.
// - SpellBehaviorBase를 상속받아 Behave를 구현합니다.
// - Behave가 호출되면, spawnPosition(생성 위치)와 SpellData의 파라미터(방향, 속도, 중력, 개수 등)를 사용해
//   투사체를 해당 방향으로 이동시키는 물리 기반 동작을 실행합니다.
// - 여러 개(count) 생성, 중력 적용, 지정된 속도/지속시간 등 다양한 파라미터를 반영합니다.
// - 일정 시간이 지나면 오브젝트를 파괴합니다.
//
// 즉, 이 코드는 "투사체 형태의 스펠"이 날아가는 동작을 담당합니다.