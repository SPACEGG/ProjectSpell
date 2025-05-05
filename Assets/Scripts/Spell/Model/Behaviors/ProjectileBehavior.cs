using System.Collections;
using UnityEngine;

namespace Spell.Model.Behaviors
{
    public class ProjectileBehavior : SpellBehaviorBase
    {
        /// <summary>
        /// Speed of the projectile
        /// </summary>
        private const float Speed = 0f;  // Todo:기본값 고민

        /// <summary>
        /// Duration of the projectile's flight
        /// </summary>
        private const int Duration = 60;

        public override void Behave(Vector3 spawnPosition)
        {
            // SpellData의 Direction을 사용, 없으면 기본값 [0,0,1]
            Vector3 direction = Data.Direction ?? Vector3.forward;
            direction = direction.normalized;

            float speed = Data.Speed > 0 ? Data.Speed : 30f; // SpellData에서 속도 사용, 없으면 기본값
            float duration = Data.Duration > 0 ? Data.Duration : 3f; // SpellData에서 지속시간 사용, 없으면 기본값

            StartCoroutine(MoveProjectile(direction, speed, duration));
        }

        private IEnumerator MoveProjectile(Vector3 direction, float speed, float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                transform.position += direction * (speed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Destroy the projectile after the duration
            Destroy(gameObject);
        }
    }
}

// 이 코드는 ProjectileBehavior(투사체 스펠 동작) 클래스입니다.
// - SpellBehaviorBase를 상속받아 Activate를 구현합니다.
// - Activate가 호출되면, 목표 위치(targetPosition)와 시전자(caster) 정보를 받아
//   투사체를 해당 방향으로 이동시키는 코루틴(MoveProjectile)을 실행합니다.
// - MoveProjectile 코루틴은 Duration(3초) 동안 지정된 방향으로 Speed(30f) 속도로 이동하다가
//   시간이 끝나면 오브젝트를 파괴합니다.
//
// 즉, 이 코드는 "투사체 형태의 스펠"이 날아가는 동작을 담당합니다.