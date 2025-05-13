using Common.Models;
using Spell.Model.Enums;
using UnityEngine;

namespace Spell.Model.Actions
{
    public record KnuckbackSpellContext : SpellContext
    {
        public GameObject Target { get; init; }
        public GameObject Origin { get; init; }
        public float BaseForce { get; init; }
        public ElementType Element { get; init; }
    }

    public class KnuckbackAction : SpellAction
    {
        public override void Apply(SpellContext context)
        {
            if (context is not KnuckbackSpellContext KnuckbackContext)
            {
                Debug.LogError("Invalid context type for KnuckbackAction");
                return;
            }

            if (KnuckbackContext.Target == null)
            {
                Debug.LogWarning("No target specified for knuckback action");
                return;
            }

            if (!KnuckbackContext.Target.TryGetComponent<Rigidbody>(out var targetRigid))
            {
                targetRigid = KnuckbackContext.Target.GetComponentInParent<Rigidbody>();

                if (targetRigid == null)
                {
                    Debug.LogWarning($"Target {KnuckbackContext.Target.name} has no Rigidbody");
                    return;
                }
            }

            if (KnuckbackContext.Origin == null)
            {
                Debug.LogWarning("No origin specified for knuckback action");
                return;
            }

            if (!KnuckbackContext.Origin.TryGetComponent<Rigidbody>(out var originRigid))
            {
                targetRigid = KnuckbackContext.Target.GetComponentInParent<Rigidbody>();

                if (targetRigid == null)
                {
                    Debug.LogWarning($"Origin {KnuckbackContext.Target.name} has no Rigidbody");
                    return;
                }
            }

            Vector3 direction = originRigid.linearVelocity.normalized;
            targetRigid.AddForce(direction * KnuckbackContext.BaseForce, ForceMode.Impulse);
        }
    }
}