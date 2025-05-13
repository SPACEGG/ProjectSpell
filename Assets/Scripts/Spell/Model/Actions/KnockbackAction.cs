using Common.Models;
using Spell.Model.Enums;
using UnityEngine;

namespace Spell.Model.Actions
{
    public record KnockbackActionContext : ActionContext
    {
        public GameObject Target { get; init; }
        public GameObject Origin { get; init; }
        public float BaseForce { get; init; }
        public ElementType Element { get; init; }
    }

    public class KnockbackAction : SpellAction
    {
        public override void Apply(ActionContext context)
        {
            if (context is not KnockbackActionContext KnockbackContext)
            {
                Debug.LogError("Invalid context type for KnuckbackAction");
                return;
            }

            if (KnockbackContext.Target == null)
            {
                Debug.LogWarning("No target specified for knuckback action");
                return;
            }

            if (!KnockbackContext.Target.TryGetComponent<Rigidbody>(out var targetRigid))
            {
                targetRigid = KnockbackContext.Target.GetComponentInParent<Rigidbody>();

                if (targetRigid == null)
                {
                    Debug.LogWarning($"Target {KnockbackContext.Target.name} has no Rigidbody");
                    return;
                }
            }

            if (KnockbackContext.Origin == null)
            {
                Debug.LogWarning("No origin specified for knuckback action");
                return;
            }

            if (!KnockbackContext.Origin.TryGetComponent<Rigidbody>(out var originRigid))
            {
                targetRigid = KnockbackContext.Target.GetComponentInParent<Rigidbody>();

                if (targetRigid == null)
                {
                    Debug.LogWarning($"Origin {KnockbackContext.Origin.name} has no Rigidbody");
                    return;
                }
            }

            if (originRigid.linearVelocity.sqrMagnitude < 0.01f)
            {
                Debug.LogWarning($"Origin {KnockbackContext.Origin.name} has no velocity");
                return;
            }

            Vector3 direction = originRigid.linearVelocity.normalized;
            targetRigid.AddForce(direction * KnockbackContext.BaseForce, ForceMode.Impulse);
        }
    }
}