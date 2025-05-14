using Common.Models;
using UnityEngine;

namespace Spell.Model.Actions
{

    public class DamageAction : SpellAction
    {
        public override void Apply(ActionContext context)
        {
            foreach (var target in context.Targets)
            {
                if (!target.TryGetComponent<IHealthProvider>(out var healthProvider))
                {
                    healthProvider = target.GetComponentInParent<IHealthProvider>();

                    if (healthProvider == null)
                    {
                        Debug.LogWarning($"Target {target.name} has no health provider");
                        return;
                    }
                }

                healthProvider.HealthModel.TakeDamage(context.Value);
            }
        }
    }
}