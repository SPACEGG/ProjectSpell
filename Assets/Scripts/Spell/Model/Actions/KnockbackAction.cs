using UnityEngine;

namespace Spell.Model.Actions
{

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
                if (target != null && TryGetRigidbody(target, out var targetRigid))
                    targetRigid.AddForce(direction * context.Value, ForceMode.Impulse);
            }
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