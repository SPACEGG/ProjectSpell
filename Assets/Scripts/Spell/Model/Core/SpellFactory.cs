using Spell.Model.Behaviors;
using Spell.Model.Data;
using Spell.Model.Enums;
using UnityEngine;

namespace Spell.Model.Core
{
    public static class SpellFactory
    {
        public static SpellBehaviorBase CreateSpellGameObject(SpellData data)
        {
            var  gameObject= new GameObject($"[Spell] {data?.Name}");

            SpellBehaviorBase behavior;
            // 실패 조건: null, 이름이 Failure/DefaultSpell
            if (data == null || data.Name == "Failure" || data.Name == "DefaultSpell")
            {
                behavior = gameObject.AddComponent<FailureBehavior>();
            }
            else
            {
                switch (data.Behavior)
                {
                    case BehaviorType.Projectile:
                        behavior = gameObject.AddComponent<ProjectileBehavior>();
                        break;
                    default:
                        Debug.LogWarning($"Unknown BehaviorType: {data.Behavior}");
                        behavior = gameObject.AddComponent<FailureBehavior>();
                        break;
                }
            }

            behavior.Data = data;

            ApplyVFX(gameObject, data);

            return behavior;
        }

        // TODO: 에셋 이펙트 및 머티리얼 추가해야됨 -> SpellData및 GPT프롬프트 수정
        private static void ApplyVFX(GameObject gameObject, SpellData data)
        {
            // 1. 크기 적용 (Vector3)
            Vector3 size = data.Size;
            if (size == Vector3.zero)
                size = Vector3.one * 0.001f;
            gameObject.transform.localScale = size;

            // 2. 메시 적용 (Shape)
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
                case ShapeType.Plane:
                    tempPrimitive = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    break;
                case ShapeType.Quad:
                    tempPrimitive = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    break;
                case ShapeType.Sphere:
                default:
                    tempPrimitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    break;
            }
            if (tempPrimitive != null)
            {
                mesh = tempPrimitive.GetComponent<MeshFilter>().sharedMesh;
                Object.Destroy(tempPrimitive);
            }

            var filter = gameObject.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh; // 선택된 메시(Shape)를 적용

            var renderer = gameObject.AddComponent<MeshRenderer>();

            // 3. VFX 머티리얼 적용 (SpellData.VfxName이 None이 아니면)
            if (data.VfxName != VfxNameType.None)
            {
                string vfxNameStr = data.VfxName.ToString();
                // 실제 경로에 맞게 공백 포함
                string resourcePath = $"Magic VFX/Magic VFX - Ice (FREE)/Models/Materials/{vfxNameStr}";
                Debug.Log($"[VFX] Resources.Load 경로: {resourcePath}");
                var vfxMat = Resources.Load<Material>(resourcePath);
                if (vfxMat != null)
                {
                    renderer.material = vfxMat;
                    Debug.Log($"VFX 머티리얼 적용: {resourcePath}");
                }
                else
                {
                    Debug.LogWarning($"VFX 머티리얼을 찾을 수 없음: {resourcePath}.mat");
                    renderer.material = new Material(Shader.Find("Universal Render Pipeline/Simple Lit"));
                }
            }
            else
            {
                renderer.material = new Material(Shader.Find("Universal Render Pipeline/Simple Lit"));
            }
        }
    }
}