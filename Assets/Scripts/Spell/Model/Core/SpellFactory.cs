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

            // SpellData 기반 VFX 적용
            ApplyVFX(gameObject, data);  // TODO:이펙트 및 머티리얼 GPT프롬프트에 추가

            return behavior;
        }

        // SpellData의 Shape, Size, Element 정보를 활용하여 VFX 적용
        private static void ApplyVFX(GameObject gameObject, SpellData data)
        {
            // 크기 적용
            float size = data.Size > 0 ? data.Size : 2f;
            gameObject.transform.localScale = Vector3.one * size;

            // 메시 적용
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

            var renderer = gameObject.AddComponent<MeshRenderer>();

            // ElementType에 따라 색상 지정
            Color color = Color.red;
            switch (data.Element)
            {
                case ElementType.Fire:
                    color = Color.red;
                    break;
                case ElementType.Ice:
                    color = Color.cyan;
                    break;
                case ElementType.Earth:
                    color = new Color(0.5f, 0.25f, 0f);
                    break;
                case ElementType.Common:
                    color = Color.white;
                    break;
                case ElementType.None:
                default:
                    color = Color.gray;
                    break;
            }

            var mat = new Material(Shader.Find("Universal Render Pipeline/Simple Lit"))
            {
                color = color
            };
            renderer.material = mat;
        }
    }
}