using Common.Models;
using Spell.Model.Enums;
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

                ElementType targetElement = ElementType.Common;
                if (!target.TryGetComponent(out IElementProvider targetElementProvider))
                    targetElementProvider = target.GetComponentInParent<IElementProvider>();

                if (targetElementProvider != null)
                    targetElement = targetElementProvider.Element;

                healthProvider.HealthModel.TakeDamage(context.Value * GetAffinityMultiplier(context.OriginElement, targetElement));

            }
        }

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
    }
}