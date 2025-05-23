using System.Collections;
using UnityEngine;
using Spell.Model.Data;
using Common.Models;
using Spell.Model.Enums;
using System.Collections.Generic;
using Multiplay;

namespace Spell.Model.Behaviors
{
    public class ProjectileBehavior : SpellBehaviorBase, IHealthProvider, IElementProvider
    {
        public ElementType Element { get; init; }

        public HealthModel HealthModel { get; init; }

        private List<SpellActionData> actionList;
        private bool activateOnCollision;

        public override void Behave(SpellData spellData)
        {
            Random.InitState(RandomSeed);

            actionList = spellData.Actions;
            activateOnCollision = spellData.ActivateOnCollision;

            // 팩토리에서 이미 위치를 결정하므로, transform.position만 사용
            Vector3 spawnPosition = transform.position;

            int count = spellData.Count > 0 ? spellData.Count : 1;
            // 원본 오브젝트 비활성화 (복제체만 활성)
            gameObject.SetActive(false);

            for (int i = 0; i < count; i++)
            {
                SpawnProjectile(spawnPosition, i, count, spellData);
            }
            // 원본 오브젝트는 파괴하지 않고 비활성화만 함
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (activateOnCollision)
            {
                MultiplayManager.Singleton.ApplyActions(actionList, collision.gameObject, gameObject);
                StartCoroutine(DestroyAfterSeconds(gameObject, 0.1f));
            }
        }

        private void SpawnProjectile(Vector3 spawnPosition, int index, int totalCount, SpellData spellData)
        {
            // 랜덤 오프셋 제거: 항상 동일한 위치에 생성
            // Vector3 randomOffset = Random.insideUnitSphere * spellData.SpreadRange;
            // GameObject projectile = Instantiate(gameObject, spawnPosition + randomOffset, Quaternion.identity);
            GameObject projectile = Instantiate(gameObject, spawnPosition, Quaternion.identity);
            projectile.SetActive(true);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            Vector3 direction = CalculateDirection(index, totalCount, spellData);
            rb.linearVelocity = direction.normalized * spellData.Speed;

            // Destroy 코루틴은 복제체에서만 실행
            projectile.GetComponent<ProjectileBehavior>().StartCoroutine(DestroyAfterSeconds(projectile, spellData.Duration));
        }

        private Vector3 CalculateDirection(int index, int totalCount, SpellData spellData)
        {
            // SpellData.Direction을 기준으로 SpreadAngle 적용
            Vector3 baseDirection = spellData.Direction.normalized;
            if (totalCount > 1 && spellData.SpreadAngle != 0f)
            {
                float angleStep = spellData.SpreadAngle;
                float angle = (index - (totalCount - 1) / 2f) * angleStep;
                // Y축(상하) 기준으로 회전 (수평 퍼짐)
                baseDirection = Quaternion.Euler(0, angle, 0) * baseDirection;
            }
            return baseDirection;
        }

        private IEnumerator DestroyAfterSeconds(GameObject obj, float duration)
        {
            yield return new WaitForSeconds(duration);

            if (obj) Destroy(obj);
        }
    }
}