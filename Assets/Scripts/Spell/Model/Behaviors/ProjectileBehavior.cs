using System.Collections;
using System.Collections.Generic;
using Entity.Prefabs;
using Spell.Model.Actions;
using Spell.Model.Core;
using Spell.Model.Data;
using Spell.Model.Enums;
using UnityEngine;

namespace Spell.Model.Behaviors
{
    public class ProjectileBehavior : SpellBehaviorBase
    {
        private PrefabVisualData projectileVisualData;
        private GameObject projectilePrefab; // 실제 프리펩
        private SpellData _spellData; // 스펠 데이터

        private ElementType elementType; // 원소 타입
        private ShapeType shapeType; // 형태
        private Vector3 scale = Vector3.one; // 크기
        private float speed = 1f; // 속도
        private int count = 1; // 개수
        private float spreadAngle = 10f; // 발사 퍼지는 각도
        private float spreadRange = 0f; // 생성 범위
        private bool hasGravity = true; // 중력 여부
        private bool activateOnCollision = true; // 충돌 시 활성화 (false라면 수명 끝날때 활성화)
        private float lifetime = 5f; // 수명 (끝나면 사라짐)
        private Vector3? direction; // 발사 방향 (로컬좌표계)

        private Vector3 worldDirection; // 발사 방향 (월드좌표계)

        public override void Behave(SpellData spellData)
        {
            _spellData = spellData;
            elementType = spellData.Element;
            shapeType = spellData.Shape;
            direction = spellData.Direction ?? Vector3.forward;
            count = spellData.Count;
            scale = spellData.Size;
            speed = spellData.Speed;
            hasGravity = spellData.HasGravity;
        }

        private void Start()
        {
            projectileVisualData = Resources.Load<PrefabVisualData>("Prefabs/ProjectileVisuals/ProjectileVisualData");

            worldDirection = transform.TransformDirection(direction.Value);
            SpawnProjectile();

            StartCoroutine(DestroyEntity(lifetime));
        }

        // 발사체 소환
        private void SpawnProjectile()
        {
            projectilePrefab = GetPrefab(elementType, shapeType, scale);

            for (int i = 0; i < count; i++)
            {
                // 발사체별 퍼진 transform 정하고 생성
                Vector3 spreadPosition = transform.TransformPoint(Random.insideUnitSphere * (spreadRange * 0.5f));
                Vector3 spreadDirection = GetSpreadDirection(worldDirection, spreadAngle);
                GameObject projectile = Instantiate(
                    projectilePrefab,
                    spreadPosition,
                    Quaternion.LookRotation(spreadDirection),
                    transform
                );

                // 충돌 끄기
                projectile.layer = LayerMask.NameToLayer("TemporaryProjectile");

                // 크기 조절
                projectile.transform.localScale = scale;

                // 날리기
                ThrowProjectile(projectile);

                // OnCollision
                projectile.GetComponent<Projectile>().OnCollision += (proj, col) =>
                {
                    if (activateOnCollision)
                    {
                        ApplyActions(col.gameObject, proj.gameObject);
                        proj.ApplyDestroyEffect(0.1f);
                    }
                };

                // 충돌 켜기
                StartCoroutine(EnableCollision(projectile, 1f));
            }
        }

        private void ApplyActions(GameObject collided, GameObject origin)
        {
            if (_spellData?.Actions == null) return;

            foreach (var actionData in _spellData.Actions)
            {
                var action = ActionFactory.CreateAction(actionData);
                if (action == null) continue;
                ActionContext context = new(actionData, collided, origin, _spellData);
                action.Apply(context);
            }
        }

        // 스스로를 제거
        private IEnumerator DestroyEntity(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (activateOnCollision)
            {
                Destroy(gameObject);
            }
            else
            {
                // TODO: Action.Apply

                // 터지고사라지기
                foreach (Projectile proj in GetComponentsInChildren<Projectile>())
                {
                    proj.ApplyDestroyEffect(0.1f);
                }
            }
        }

        // 발사체 프리팹 가져오기
        private GameObject GetPrefab(ElementType elementType, ShapeType shapeType, Vector3 scale)
        {
            SizeType sizeType = GetSizeType(scale.magnitude);
            List<GameObject> list = projectileVisualData.GetPrefabList(elementType, shapeType, sizeType);
            int index = Random.Range(0, list.Count);
            return list[index];
        }

        private SizeType GetSizeType(float scale)
        {
            if (scale < 1f) return SizeType.Small;
            if (scale < 3f) return SizeType.Medium;
            return SizeType.Big;
        }

        // 발사체 날리기
        private void ThrowProjectile(GameObject projectile)
        {
            Rigidbody rigid = projectile.GetComponent<Rigidbody>();
            rigid.useGravity = hasGravity;
            rigid.AddForce(projectile.transform.forward * speed, ForceMode.Impulse);
        }

        // 발사체 각도 퍼트리기
        private Vector3 GetSpreadDirection(Vector3 direction, float spreadAngle)
        {
            direction.Normalize();

            // direction을 기준 회전 좌표계
            Quaternion baseRotation = Quaternion.LookRotation(direction);

            // spreadAngle 내에서 무작위 방향
            float angle = Random.Range(0f, spreadAngle) * Mathf.Deg2Rad;
            float azimuth = Random.Range(0f, 360f) * Mathf.Deg2Rad;

            // 구면 좌표계 방향 생성
            Vector3 spreadDir = new(
                Mathf.Sin(angle) * Mathf.Cos(azimuth),
                Mathf.Sin(angle) * Mathf.Sin(azimuth),
                Mathf.Cos(angle)
            );

            // direction 기준 회전방향
            return baseRotation * spreadDir;
        }

        // 일정 시간 후 발사체끼리 충돌 생기게 하기 (안그러면 처음에 바로 충돌함)
        private IEnumerator EnableCollision(GameObject projectile, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (projectile != null)
                projectile.layer = LayerMask.NameToLayer("Default");
        }
    }
}