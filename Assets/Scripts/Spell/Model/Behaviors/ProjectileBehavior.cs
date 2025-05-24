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

        // public
        public List<SpellActionData> ActionList { get; set; }
        public bool ActivateOnCollision { get; set; }

        private Rigidbody _rb;
        private bool _alignForward = true;

        public override void Behave(SpellData spellData)
        {
            Random.InitState(RandomSeed);

            ActionList = spellData.Actions;
            ActivateOnCollision = spellData.ActivateOnCollision;

            // 팩토리에서 이미 위치를 결정하므로, transform.position만 사용
            Vector3 spawnPosition = transform.position;

            int count = spellData.Count > 0 ? spellData.Count : 1;
            // 원본 오브젝트 비활성화 후 삭제 (복제체만 활성)
            gameObject.SetActive(false);
            Invoke(nameof(DestroyThis), spellData.Duration);

            for (int i = 0; i < count; i++)
            {
                SpawnProjectile(spawnPosition, i, count, spellData);
            }
            // 원본 오브젝트는 파괴하지 않고 비활성화만 함
        }

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (_alignForward)
                AlignForwardToVelocity();
        }

        // 발사체의 각도를 날아가는 방향으로 향하게 하기
        private void AlignForwardToVelocity()
        {
            // Y축이 길쭉한(세로) 프리팹을 날아가는 방향으로 맞추려면
            // transform.up을 velocity 방향으로 맞춰야 함
            if (_rb != null && _rb.linearVelocity.sqrMagnitude > 0.01f)
                transform.up = _rb.linearVelocity.normalized;
        }

        private void OnCollisionEnter(Collision collision)
        {
            _alignForward = false;
            if (ActivateOnCollision && collision.gameObject.tag == "Player")
            {
                var effectPrefab = Resources.Load<GameObject>("VFX/Particle/Hit1");
                if (effectPrefab != null)
                {
                    Instantiate(effectPrefab, transform);
                }

                var audioClip = Resources.Load<AudioClip>("VFX/Sound/_Quazic");
                if (audioClip != null)
                {
                    AudioSource.PlayClipAtPoint(audioClip, transform.position, 1.0f);
                }
                MultiplayManager.Singleton.ApplyActions(ActionList, collision.gameObject, gameObject);
                StartCoroutine(DestroyAfterSeconds(gameObject, 0.1f));
            }
        }

        private void SpawnProjectile(Vector3 spawnPosition, int index, int totalCount, SpellData spellData)
        {
            // 여러 개 생성 시 항상 퍼져서 생성 (원형 분산)
            Vector3 offset = Vector3.zero;
            if (totalCount > 1)
            {
                float radius = Mathf.Max(spellData.Size.x, spellData.Size.z) * 0.6f + 1.0f;
                float angle = 360f * index / Mathf.Max(1, totalCount);
                float rad = angle * Mathf.Deg2Rad;
                offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * radius;
            }

            GameObject projectile = Instantiate(gameObject, spawnPosition + offset, Quaternion.identity);
            projectile.SetActive(true);
            var projectileB = projectile.GetComponent<ProjectileBehavior>();
            projectileB.ActionList = spellData.Actions;
            projectileB.ActivateOnCollision = spellData.ActivateOnCollision;
            projectileB.Caster = Caster;

            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            Vector3 direction = CalculateDirection(index, totalCount, spellData);
            rb.linearVelocity = direction.normalized * spellData.Speed;

            // Destroy 코루틴은 복제체에서만 실행
            projectileB.StartCoroutine(DestroyAfterSeconds(projectile, spellData.Duration));
            projectileB.StartCoroutine(SetToDefaultLayer(projectile, 0.2f));
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

        private void DestroyThis()
        {
            Destroy(gameObject);
        }

        private IEnumerator DestroyAfterSeconds(GameObject obj, float duration)
        {
            yield return new WaitForSeconds(duration);

            if (obj) Destroy(obj);
        }

        private IEnumerator SetToDefaultLayer(GameObject obj, float duration)
        {
            yield return new WaitForSeconds(duration);

            obj.layer = LayerMask.NameToLayer("Default");
        }
    }
}