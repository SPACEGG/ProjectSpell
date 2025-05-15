using Spell.Model.Actions;
using Spell.Model.Data;
using Spell.Model.Enums;
using UnityEngine;

namespace Spell.Model.Core
{
    // TODO: 제안: 하위 클래스를 안쓰고 그냥 ActionContext으로 해도 상관없을거 같다
    // 어차피 CreateContext가 받아오는 매개변수 정보는 동일하기 때문
    // 그렇게 바꾸면 이 팩토리 자체가 없어도 상관없음 (여기 코드 너무 중복이 많음)
    public static class ActionContextFactory
    {
        // target: 날린/맞은 플레이어, origin: 발사체 오브젝트
        public static ActionContext CreateContext(SpellActionData actionData, GameObject target, GameObject origin, SpellData spellData)
        {
            return actionData.Action switch
            {
                ActionType.Damage => CreateDamageContext(target, spellData, actionData),
                ActionType.Heal => CreateHealContext(target, spellData, actionData),
                ActionType.Knockback => CreateKnuckbackContext(target, spellData, actionData, origin),
                ActionType.ManaRegen => CreateManaRegenContext(target, spellData, actionData),
                _ => null
            };
        }

        private static ActionContext CreateDamageContext(GameObject target, SpellData spellData, SpellActionData actionData)
        {
            return new DamageActionContext
            {
                Target = target,
                BaseDamage = actionData.Value,
                Element = spellData.Element
            };
        }

        private static ActionContext CreateHealContext(GameObject target, SpellData spellData, SpellActionData actionData)
        {
            return new HealActionContext
            {
                Target = target,
                BaseHeal = actionData.Value,
                Element = spellData.Element
            };
        }

        private static ActionContext CreateKnuckbackContext(GameObject target, SpellData spellData, SpellActionData actionData, GameObject origin)
        {
            return new KnockbackActionContext
            {
                Target = target,
                Origin = origin,
                BaseForce = actionData.Value,
                Element = spellData.Element
            };
        }

        private static ActionContext CreateManaRegenContext(GameObject target, SpellData spellData, SpellActionData actionData)
        {
            return new ManaRegenActionContext
            {
                Target = target,
                BaseManaRegen = actionData.Value,
                Element = spellData.Element
            };
        }
    }
}