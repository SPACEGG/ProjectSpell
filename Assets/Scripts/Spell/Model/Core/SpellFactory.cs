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
            var gameObject = new GameObject($"[Spell] {data.Name}");

            SpellBehaviorBase behavior;
            switch (data.Behavior)
            {
                case BehaviorType.Projectile:
                    behavior = gameObject.AddComponent<ProjectileBehavior>();
                    break;
                default:
                    Debug.LogWarning($"Unknown BehaviorType: {data.Behavior}");
                    Object.Destroy(gameObject);
                    behavior = gameObject.AddComponent<FailureBehavior>();
                    break;
            }

            behavior.Data = data;

            ApplyVFX(gameObject, data);  

            return behavior;
        }

        // TODO: 에셋 이펙트 및 머티리얼 추가해야됨 -> SpellData및 GPT프롬프트 수정
        private static void ApplyVFX(GameObject gameObject, SpellData data)
        {
            // 크기 적용 (Vector3)
            Vector3 size = data.Size;
            if (size == Vector3.zero)
                size = Vector3.one * 0.001f;
            gameObject.transform.localScale = size;

            // 메시 적용 (shape만 사용)
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
            filter.sharedMesh = mesh;

            var renderer = gameObject.AddComponent<MeshRenderer>();  // TODO: 머티리얼 렌더링 등 에셋 추가가
            renderer.material = new Material(Shader.Find("Universal Render Pipeline/Simple Lit"));
        }
    }
}