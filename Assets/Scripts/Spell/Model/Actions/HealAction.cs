using Common.Models;
using UnityEngine;

namespace Spell.Model.Actions
{

    public class HealAction : SpellAction
    {
        public override void Apply(ActionContext context)
        {
            foreach (var target in context.Targets)
            {
                if (!target.TryGetComponent<INetworkHealthProvider>(out var healthProvider))
                {
                    healthProvider = target.GetComponentInParent<INetworkHealthProvider>();

                    if (healthProvider == null)
                    {
                        Debug.LogWarning($"Target {target.name} has no health provider");
                        return;
                    }
                }

                healthProvider.Heal(context.Value);
            }
        }
    }
}