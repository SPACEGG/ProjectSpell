using System;
using System.Collections;
using UnityEngine;

namespace Entity.Prefabs
{
    // ProjectileEntity의 자식 오브젝트로 있는 진짜 발사체
    // 실제 콜라이더와 물리를 가지고 있다
    // 원소타입, 모양타입, 크기를 받아 적절한 모습으로 표현된다
    public class Projectile : MonoBehaviour
    {
        // 충돌 시 이벤트를 외부에서 넣어준다
        public event Action<Projectile, Collision> OnCollision;

        [SerializeField]
        private GameObject effectPrefab;
        private Rigidbody rb;
        private bool isCollide = false;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (!isCollide)
                AlignForwardToVelocity();
        }

        // 발사체의 각도를 날라가는 방향으로 향하게 하기
        private void AlignForwardToVelocity()
        {
            if (rb.linearVelocity.sqrMagnitude > 0.01f)
                transform.forward = rb.linearVelocity.normalized;
        }

        // 충돌 시
        private void OnCollisionEnter(Collision collision)
        {
            isCollide = true;
            OnCollision?.Invoke(this, collision);
        }

        // 효과재생 후 제거
        public void ApplyDestroyEffect(float destroyDecay)
        {
            // 폭발효과는 자식 오브젝트가 아니다 (발사체가 사라져도 남아있음)
            Instantiate(effectPrefab, transform.position, Quaternion.identity);

            // destroy gameObject
            StartCoroutine(DestroyPrefab(destroyDecay));
        }

        private IEnumerator DestroyPrefab(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Destroy(gameObject);
        }
    }
}