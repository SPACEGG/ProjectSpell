using Spell.Model.Actions;
using Spell.Model.Data;
using Spell.Model.Enums;

namespace Spell.Model.Core
{
    public static class ActionFactory
    {
        public static SpellAction CreateAction(SpellActionData actionData)
        {
            return actionData.Action switch
            {
                ActionType.Damage => CreateDamageAction(),
                _ => null
            };
        }

        private static SpellAction CreateDamageAction()
        {
            return new DamageAction();
        }
    }
}