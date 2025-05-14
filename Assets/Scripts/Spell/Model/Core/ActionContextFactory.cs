using System;
using Spell.Model.Actions;
using Spell.Model.Data;
using Spell.Model.Enums;
using UnityEngine;

namespace Spell.Model.Core
{
    [Obsolete("그냥 ActionContext 직접 생성해서 쓰세요.")]
    public static class ActionContextFactory
    {
        // target: 날린/맞은 플레이어, origin: 발사체 오브젝트
        public static ActionContext CreateContext(SpellActionData actionData, GameObject target, GameObject origin, SpellData spellData)
        {
            return actionData.Action switch
            {
                // ActionType.Damage => CreateDamageContext(target, spellData, actionData),
                // ActionType.Heal => CreateHealContext(target, spellData, actionData),
                // ActionType.Knockback => CreateKnuckbackContext(target, spellData, actionData, origin),
                // ActionType.ManaRegen => CreateManaRegenContext(target, spellData, actionData),
                _ => null
            };
        }

        // private static ActionContext CreateDamageContext(GameObject target, SpellData spellData, SpellActionData actionData)
        // {
        //     return new DamageActionContext
        //     {
        //         Target = target,
        //         BaseDamage = actionData.Value,
        //         Element = spellData.Element
        //     };
        // }

        // private static ActionContext CreateHealContext(GameObject target, SpellData spellData, SpellActionData actionData)
        // {
        //     return new HealActionContext
        //     {
        //         Target = target,
        //         BaseHeal = actionData.Value,
        //         Element = spellData.Element
        //     };
        // }

        // private static ActionContext CreateKnuckbackContext(GameObject target, SpellData spellData, SpellActionData actionData, GameObject origin)
        // {
        //     return new KnockbackActionContext
        //     {
        //         Target = target,
        //         Origin = origin,
        //         BaseForce = actionData.Value,
        //         Element = spellData.Element
        //     };
        // }

        // private static ActionContext CreateManaRegenContext(GameObject target, SpellData spellData, SpellActionData actionData)
        // {
        //     return new ManaRegenActionContext
        //     {
        //         Target = target,
        //         BaseManaRegen = actionData.Value,
        //         Element = spellData.Element
        //     };
        // }
    }
}