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
                default:  // Todo: 디폴트를 투사체로 할건지, 아니면 실패로 할건지 결정해야됨
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
            // 1. 크기 적용 (Vector3)
            // SpellData의 Size(x, y, z) 값을 오브젝트의 localScale에 적용하여 각 축별로 크기를 조절
            Vector3 size = data.Size;
            if (size == Vector3.zero)
                size = Vector3.one * 0.001f;
            gameObject.transform.localScale = size;

            // 2. 메시 적용 (Shape)
            // SpellData의 Shape에 따라 Unity 기본 도형을 임시로 생성하고, 해당 메시를 복사하여 사용
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
                // 임시 오브젝트에서 메시를 꺼내와서 사용 후 바로 삭제
                mesh = tempPrimitive.GetComponent<MeshFilter>().sharedMesh;
                Object.Destroy(tempPrimitive);
            }

            // 3. MeshFilter, MeshRenderer 컴포넌트 추가 및 메시/머티리얼 적용
            // - MeshFilter: 3D 형태(Shape) 담당
            // - MeshRenderer: 화면에 렌더링(그리기) 담당
            var filter = gameObject.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;

            var renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.material = new Material(Shader.Find("Universal Render Pipeline/Simple Lit"));
            // TODO: 머티리얼, 렌더링 에셋 등은 추후 SpellData에 추가하여 동적으로 적용 가능
        }
    }
}