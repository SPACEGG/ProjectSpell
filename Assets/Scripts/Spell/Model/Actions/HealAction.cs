using Common.Models;
using Spell.Model.Enums;
using UnityEngine;

namespace Spell.Model.Actions
{
    public record HealActionContext : ActionContext
    {
        public GameObject Target { get; init; }
        public float BaseHeal { get; init; }
        public ElementType Element { get; init; }
    }

    public class HealAction : SpellAction
    {
        public override void Apply(ActionContext context)
        {
            if (context is not HealActionContext HealContext)
            {
                Debug.LogError("Invalid context type for HealAction");
                return;
            }

            if (HealContext.Target == null)
            {
                Debug.LogWarning("No target specified for Heal action");
                return;
            }

            if (!HealContext.Target.CompareTag("Player"))
            {
                return;
            }

            if (!HealContext.Target.TryGetComponent<IHealthProvider>(out var healthProvider))
            {
                healthProvider = HealContext.Target.GetComponentInParent<IHealthProvider>();

                if (healthProvider == null)
                {
                    Debug.LogWarning($"Target {HealContext.Target.name} has no health provider");
                    return;
                }
            }

            healthProvider.HealthModel.Heal(HealContext.BaseHeal);
        }
    }
}