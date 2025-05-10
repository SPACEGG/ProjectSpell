using Spell.Model.Actions;
using Spell.Model.Data;
using Spell.Model.Enums;
using UnityEngine;

namespace Spell.Model.Core
{
    public static class ActionContextFactory
    {
        public static SpellContext CreateContext(SpellActionData actionData, GameObject target, SpellData spellData)
        {
            return actionData.Action switch
            {
                ActionType.Damage => CreateDamageContext(target, spellData, actionData),
                _ => null
            };
        }

        private static SpellContext CreateDamageContext(GameObject target, SpellData spellData, SpellActionData actionData)
        {
            return new DamageSpellContext
            {
                Target = target,
                BaseDamage = actionData.Value,
                Element = spellData.Element
            };
        }
    }
}