using System.Collections;
using System.Collections.Generic;
using Entity.Prefabs;
using Spell.Model.Enums;
using UnityEngine;

namespace Entity
{
    public class ProjectileEntity : Entity
    {
        private PrefabVisualData projectileVisualData;
        private GameObject projectilePrefab;        // 실제 프리펩

        private ElementType elementType;            // 원소 타입
        private ShapeType shapeType;                // 형태
        private float scale = 1f;                   // 크기
        private float speed = 1f;                   // 속도
        private int count = 1;                      // 개수
        private float spreadAngle = 10f;            // 발사 퍼지는 각도
        private float spreadRange = 0f;             // 생성 범위
        private bool hasGravity = true;             // 중력 여부
        private bool activateOnCollision = true;    // 충돌 시 활성화 (false라면 수명 끝날때 활성화)
        private float lifetime = 5f;                // 수명 (끝나면 사라짐)
        private Vector3 offset = Vector3.zero;      // 생성 위치 오프셋
        private Vector3? direction;                 // 발사 방향

        #region Builder

        public ProjectileEntity SetElementType(ElementType type)
        {
            elementType = type;
            return this;
        }
        public ProjectileEntity SetShapeType(ShapeType type)
        {
            shapeType = type;
            return this;
        }
        public ProjectileEntity SetScale(float scale)
        {
            this.scale = scale;
            return this;
        }
        public ProjectileEntity SetSpeed(float speed)
        {
            this.speed = speed;
            return this;
        }
        public ProjectileEntity SetCount(int count)
        {
            this.count = count;
            return this;
        }
        public ProjectileEntity SetSpreadAngle(float angle)
        {
            spreadAngle = angle;
            return this;
        }
        public ProjectileEntity SetSpreadRange(float range)
        {
            spreadRange = range;
            return this;
        }
        public ProjectileEntity SetGravity(bool hasGravity)
        {
            this.hasGravity = hasGravity;
            return this;
        }
        public ProjectileEntity SetActivateOnCollision(bool active)
        {
            activateOnCollision = active;
            return this;
        }
        public ProjectileEntity SetLifetime(float seconds)
        {
            lifetime = seconds;
            return this;
        }
        public ProjectileEntity SetOffset(Vector3 offset)
        {
            this.offset = offset;
            return this;
        }
        public ProjectileEntity SetDirection(Vector3 direction)
        {
            this.direction = direction;
            return this;
        }

        #endregion

        private void Awake()
        {
            projectileVisualData = Resources.Load<PrefabVisualData>("Prefabs/ProjectileVisuals/ProjectileVisualData");
        }

        private void Start()
        {
            direction ??= transform.forward;
            direction = direction.Value.normalized;

            Spawn();

            StartCoroutine(DestroyEntity(lifetime));
        }

        IEnumerator DestroyEntity(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (activateOnCollision)
            {
                // 그냥사라지기
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

        // 발사체 소환
        public void Spawn()
        {
            projectilePrefab = GetPrefab(elementType, shapeType, scale);

            for (int i = 0; i < count; i++)
            {
                // 발사체별 퍼진 transform 정하고 생성
                Vector3 spreadPosition = transform.TransformPoint(offset + Random.insideUnitSphere * (spreadRange * 0.5f));
                Vector3 spreadDirection = GetSpreadDirection(direction.Value, spreadAngle);
                GameObject projectile = Instantiate(
                    projectilePrefab,
                    spreadPosition,
                    Quaternion.LookRotation(spreadDirection),
                    transform
                );

                // 충돌 끄기
                projectile.layer = LayerMask.NameToLayer("TemporaryProjectile");

                // 크기 조절
                projectile.transform.localScale = Vector3.one * scale;

                // 날리기
                ThrowProjectile(projectile);

                // OnCollision
                projectile.GetComponent<Projectile>().OnCollision += (proj, col) =>
                {
                    if (activateOnCollision)
                    {
                        // TODO: Action.Apply

                        // 터지고사라지기
                        proj.ApplyDestroyEffect(0.1f);
                    }
                };

                // 충돌 켜기
                StartCoroutine(EnableCollision(projectile, 1f));
            }
        }

        // 발사체 프리팹 가져오기
        private GameObject GetPrefab(ElementType elementType, ShapeType shapeType, float scale)
        {
            SizeType sizeType = GetSizeType(scale);
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