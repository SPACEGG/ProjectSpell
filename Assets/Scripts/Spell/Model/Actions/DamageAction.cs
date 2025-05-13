using Common.Models;
using Spell.Model.Enums;
using UnityEngine;

namespace Spell.Model.Actions
{
    public record DamageActionContext : ActionContext
    {
        public GameObject Target { get; init; }
        public float BaseDamage { get; init; }
        public ElementType Element { get; init; }
    }

    public class DamageAction : SpellAction
    {
        public override void Apply(ActionContext context)
        {
            if (context is not DamageActionContext damageContext)
            {
                Debug.LogError("Invalid context type for DamageAction");
                return;
            }

            if (damageContext.Target is null)
            {
                Debug.LogWarning("No target specified for damage action");
                return;
            }

            if (!damageContext.Target.CompareTag("Player"))
            {
                return;
            }

            var healthProvider = damageContext.Target.GetComponent<IHealthProvider>();
            if (healthProvider == null)
            {
                healthProvider = damageContext.Target.GetComponentInParent<IHealthProvider>();

                if (healthProvider == null)
                {
                    Debug.LogWarning($"Target {damageContext.Target.name} has no health provider");
                    return;
                }
            }

            healthProvider.HealthModel.TakeDamage(damageContext.BaseDamage);
        }
    }
}