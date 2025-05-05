using System;
using System.Collections;
using UnityEngine;

namespace Entity.Prefabs
{
    // 엔티티가 효과 발동시 생성되는 효과 오브젝트
    // 3초후 스스로 사라진다
    public class Effect : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(DestroyPrefab(3f));
        }

        private IEnumerator DestroyPrefab(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Destroy(gameObject);
        }
    }
}