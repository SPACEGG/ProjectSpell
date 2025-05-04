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

            // Apply VFX to the game object
            ApplyVFX(gameObject);

            return behavior;
        }

        private static void ApplyVFX(GameObject gameObject)
        {
            gameObject.transform.localScale = Vector3.one * 2f;

            var filter = gameObject.AddComponent<MeshFilter>();
            var renderer = gameObject.AddComponent<MeshRenderer>();

            // Grab built-in sphere mesh
            var tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            if (tempSphere != null)
            {
                filter.sharedMesh = tempSphere.GetComponent<MeshFilter>().sharedMesh;
                Object.Destroy(tempSphere); // Cleanup temporary object
            }

            // Set material (메모리 누수 방지: 필요시 Material.Destroy 사용)
            var mat = new Material(Shader.Find("Universal Render Pipeline/Simple Lit"))
            {
                color = Color.red
            };
            renderer.material = mat;
        }
    }
}