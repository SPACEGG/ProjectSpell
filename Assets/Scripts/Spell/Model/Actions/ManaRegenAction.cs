using Common.Models;
using UnityEngine;

namespace Spell.Model.Actions
{

    public class ManaRegenAction : SpellAction
    {
        public override void Apply(ActionContext context)
        {
            foreach (var target in context.Targets)
            {
                if (target == null)
                {
                    Debug.LogWarning("Mana regen action requires target.");
                    return;
                }

                if (!target.TryGetComponent<IManaProvider>(out var manaProvider))
                {
                    manaProvider = target.GetComponentInParent<IManaProvider>();

                    if (manaProvider == null)
                    {
                        Debug.LogWarning($"Target {target.name} has no mana provider");
                        return;
                    }
                }

                manaProvider.ManaModel.GetMana(context.Value);
            }

        }
    }
}