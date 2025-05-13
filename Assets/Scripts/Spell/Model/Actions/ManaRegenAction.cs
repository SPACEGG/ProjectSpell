using Common.Models;
using Spell.Model.Enums;
using UnityEngine;

namespace Spell.Model.Actions
{
    public record ManaRegenActionContext : ActionContext
    {
        public GameObject Target { get; init; }
        public float BaseManaRegen { get; init; }
        public ElementType Element { get; init; }
    }

    public class ManaRegenAction : SpellAction
    {
        public override void Apply(ActionContext context)
        {
            if (context is not ManaRegenActionContext ManaContext)
            {
                Debug.LogError("Invalid context type for ManaRegenAction");
                return;
            }

            if (ManaContext.Target == null)
            {
                Debug.LogWarning("No target specified for mana regenerate action");
                return;
            }

            if (!ManaContext.Target.CompareTag("Player"))
            {
                return;
            }

            if (!ManaContext.Target.TryGetComponent<IManaProvider>(out var manaProvider))
            {
                manaProvider = ManaContext.Target.GetComponentInParent<IManaProvider>();

                if (manaProvider == null)
                {
                    Debug.LogWarning($"Target {ManaContext.Target.name} has no mana provider");
                    return;
                }
            }

            manaProvider.ManaModel.GetMana(ManaContext.BaseManaRegen);
        }
    }
}