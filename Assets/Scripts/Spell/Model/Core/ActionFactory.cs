using System;
using System.Collections.Generic;
using Spell.Model.Actions;
using Spell.Model.Data;
using Spell.Model.Enums;

namespace Spell.Model.Core
{
    public static class ActionFactory
    {
        public static SpellAction CreateAction(SpellActionData actionData)
        {
            return actionConstructors.TryGetValue(actionData.Action, out var constructor)
                ? constructor()
                : null;
        }

        private static readonly Dictionary<ActionType, Func<SpellAction>> actionConstructors = new()
        {
            { ActionType.Damage, () => new DamageAction() },
            { ActionType.Heal, () => new HealAction() },
            { ActionType.Knockback, () => new KnockbackAction() },
            { ActionType.ManaRegen, () => new ManaRegenAction() }
        };
    }
}