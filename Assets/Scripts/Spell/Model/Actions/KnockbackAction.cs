using Common.Models;
using Spell.Model.Enums;
using UnityEngine;

namespace Spell.Model.Actions
{

    // 넉백 액션
    // target에게 origin의 방향, value만큼의 힘을 가한다
    // [원소별 상성]
    // target=Earth: value*0.5, origin=Earth: value*2
    // target=Ice: value*1.5, origin=Ice: value*0.6667
    public class KnockbackAction : SpellAction
    {
        public override void Apply(ActionContext context)
        {
            if (context.Origin == null)
            {
                Debug.LogWarning("Knockback action requires Origin.");
                return;
            }

            if (!TryGetRigidbody(context.Origin, out var originRigid))
            {
                Debug.LogWarning($"Origin {context.Origin.name} has no Rigidbody.");
                return;
            }

            if (originRigid.linearVelocity.sqrMagnitude < 0.01f)
            {
                Debug.LogWarning($"Origin {context.Origin.name} has no velocity.");
                return;
            }

            Vector3 direction = originRigid.linearVelocity.normalized;

            foreach (var target in context.Targets)
            {
                if (target == null) continue;

                if (target.TryGetComponent(out IElementProvider targetElementProvider) && TryGetRigidbody(target, out var targetRigid))
                {
                    float knockbackPower = context.Value * GetAffinityMultiplier(context.OriginElement, targetElementProvider.Element);
                    targetRigid.AddForce(direction * knockbackPower, ForceMode.Impulse);
                }

            }
        }

        // origin vs target
        private float GetAffinityMultiplier(ElementType originElement, ElementType targetElement)
        {
            float multiplier = 1f;
            if (originElement == ElementType.Common || targetElement == ElementType.Common) return multiplier;

            if (originElement == ElementType.Ice) multiplier *= 0.8f;
            if (targetElement == ElementType.Ice) multiplier *= 1.25f;
            if (originElement == ElementType.Earth) multiplier *= 2f;
            if (targetElement == ElementType.Earth) multiplier *= 0.5f;

            return multiplier;
        }

        private bool TryGetRigidbody(GameObject obj, out Rigidbody rigidbody)
        {
            if (obj.TryGetComponent(out rigidbody))
                return true;

            rigidbody = obj.GetComponentInParent<Rigidbody>();
            return rigidbody != null;
        }
    }
}