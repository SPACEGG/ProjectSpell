using Spell.Model.Behaviors;
using Spell.Model.Data;
using Spell.Model.Enums;
using UnityEngine;

namespace Spell.Model.Core
{
    public static class SpellFactory
    {
        public static GameObject CreateSpellGameObject(SpellData data)
        {
            var gameObject = new GameObject($"[Spell] {data?.Name}");

            SpellBehaviorBase behavior = data?.Behavior switch
            {
                BehaviorType.Projectile => gameObject.AddComponent<ProjectileBehavior>(),
                _ => gameObject.AddComponent<FailureBehavior>()
            };

            gameObject.tag = "Player";
            gameObject.layer = LayerMask.NameToLayer("TemporaryProjectile");

            ApplyVFX(gameObject, data);
            return gameObject;
        }

        public static GameObject CreateProjectile(GameObject baseObject, Vector3 position, Quaternion rotation)
        {
            return Object.Instantiate(baseObject, position, rotation);
        }

        private static void ApplyVFX(GameObject gameObject, SpellData data)
        {
            // 디버그: Shape, Size 값 확인
            Debug.Log($"[SpellFactory] Shape: {data.Shape}, Size: {data.Size}");

            Vector3 size = data.Size;
            if (size == Vector3.zero)
                size = Vector3.one; // 너무 작게 보정하지 말고 1로 보정
            gameObject.transform.localScale = size;

            Mesh mesh = null;
            GameObject tempPrimitive = null;
            switch (data.Shape)
            {
                case ShapeType.Cube:
                    tempPrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    break;
                case ShapeType.Capsule:
                    tempPrimitive = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    break;
                case ShapeType.Cylinder:
                    tempPrimitive = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    break;
                case ShapeType.Sphere:
                default:
                    tempPrimitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    break;
            }

            if (tempPrimitive != null)
            {
                mesh = tempPrimitive.GetComponent<MeshFilter>().sharedMesh;
#if UNITY_EDITOR
                Object.DestroyImmediate(tempPrimitive);
#else
                Object.Destroy(tempPrimitive);
#endif
            }

            var filter = gameObject.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;

            // MeshRenderer 추가 및 활성화 (중복 추가 방지)
            var renderer = gameObject.GetComponent<MeshRenderer>();
            if (renderer == null)
                renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.enabled = true; // 반드시 활성화

            // 디버그: MeshRenderer 활성화 상태 로그
            Debug.Log($"[SpellFactory] MeshRenderer enabled: {renderer.enabled}");

            // === Material 적용 ===
            if (!string.IsNullOrEmpty(data.MaterialName))
            {
                string matPath = $"VFX/Materials/{data.MaterialName}";
                var vfxMat = Resources.Load<Material>(matPath);
                if (vfxMat != null)
                {
                    renderer.material = vfxMat;
                    Debug.Log($"VFX 머티리얼 적용: {matPath}");
                }
                else
                {
                    Debug.LogWarning($"VFX 머티리얼을 찾을 수 없음: {matPath}.mat");
                    renderer.material = new Material(Shader.Find("Universal Render Pipeline/Simple Lit"));
                }
            }
            else
            {
                renderer.material = new Material(Shader.Find("Universal Render Pipeline/Simple Lit"));
            }

            // === Mesh 적용 ===
            if (!string.IsNullOrEmpty(data.MeshName))
            {
                // 실제 사용 가능한 MeshName: Axe1, Axe2, BigRock1, Hammer1, Mace1, Shield1, smallRock1, smallRock2, Spear1, Spear2, tallRock1, tallRock2
                string meshPath = $"VFX/Meshes/{data.MeshName}";
                var vfxMesh = Resources.Load<Mesh>(meshPath);
                if (vfxMesh != null)
                {
                    filter.sharedMesh = vfxMesh;
                    Debug.Log($"VFX 메쉬 적용: {meshPath}");
                }
                else
                {
                    Debug.LogWarning($"VFX 메쉬를 찾을 수 없음: {meshPath}");
                }
            }

            // === Particle 적용 ===
            if (!string.IsNullOrEmpty(data.ParticleName))
            {
                // 실제 사용 가능한 ParticleName: Sparks_red
                string particlePath = $"VFX/Particles/{data.ParticleName}";
                var particlePrefab = Resources.Load<GameObject>(particlePath);
                if (particlePrefab != null)
                {
                    var particleObj = Object.Instantiate(particlePrefab, gameObject.transform);
                    Debug.Log($"VFX 파티클 적용: {particlePath}");
                }
                else
                {
                    Debug.LogWarning($"VFX 파티클을 찾을 수 없음: {particlePath}");
                }
            }

            // === TrailEffect 적용 ===
            if (!string.IsNullOrEmpty(data.TrailName))
            {
                string trailPath = $"VFX/TrailEffects/{data.TrailName}";
                var trailPrefab = Resources.Load<GameObject>(trailPath);
                if (trailPrefab != null)
                {
                    var trailObj = Object.Instantiate(trailPrefab, gameObject.transform);
                    Debug.Log($"VFX 트레일 적용: {trailPath}");
                }
                else
                {
                    Debug.LogWarning($"VFX 트레일을 찾을 수 없음: {trailPath}");
                }
            }

            // === Sound 적용 ===
            if (!string.IsNullOrEmpty(data.SoundName))
            {
                string soundPath = $"VFX/Sound/{data.SoundName}";
                var audioClip = Resources.Load<AudioClip>(soundPath);
                if (audioClip != null)
                {
                    var audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.clip = audioClip;
                    audioSource.playOnAwake = true;
                    audioSource.spatialBlend = 1.0f; // 3D 사운드
                    audioSource.Play();
                    Debug.Log($"VFX 사운드 적용: {soundPath}");
                }
                else
                {
                    Debug.LogWarning($"VFX 사운드를 찾을 수 없음: {soundPath}");
                }
            }

            // Collider 추가 (Shape에 따라)
            switch (data.Shape)
            {
                case ShapeType.Cube:
                    if (gameObject.GetComponent<BoxCollider>() == null)
                        gameObject.AddComponent<BoxCollider>();
                    break;
                case ShapeType.Capsule:
                    if (gameObject.GetComponent<CapsuleCollider>() == null)
                        gameObject.AddComponent<CapsuleCollider>();
                    break;
                case ShapeType.Cylinder:
                    if (gameObject.GetComponent<CapsuleCollider>() == null)
                        gameObject.AddComponent<CapsuleCollider>(); // Unity에 CylinderCollider 없음, Capsule로 대체
                    break;
                case ShapeType.Sphere:
                default:
                    if (gameObject.GetComponent<SphereCollider>() == null)
                        gameObject.AddComponent<SphereCollider>();
                    break;
            }

            // Rigidbody 추가 및 설정 (중복 추가 방지)
            var rb = gameObject.GetComponent<Rigidbody>();
            if (rb == null)
                rb = gameObject.AddComponent<Rigidbody>();

            // 크기에 따라 질량 자동 설정 (x*y*z 비례, 최소값 1)
            float mass = Mathf.Max(1f, data.Size.x * data.Size.y * data.Size.z);

            // ActiveOnCollision이 false면 매우 무겁게(밀리지 않게)
            if (data.ActivateOnCollision == false)
            {
                rb.useGravity = data.HasGravity;
                rb.isKinematic = false;
                rb.mass = 1000f;
                rb.linearDamping = 10f;
                rb.angularDamping = 10f;
            }
            else
            {
                rb.useGravity = data.HasGravity;
                rb.isKinematic = false;
                rb.mass = mass;
                rb.linearDamping = 0f;
                rb.angularDamping = 0.05f;
            }
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }
}