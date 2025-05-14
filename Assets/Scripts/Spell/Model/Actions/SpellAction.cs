using System.Collections.Generic;
using Common.Models;
using Spell.Model.Behaviors;
using Spell.Model.Data;
using Spell.Model.Enums;
using UnityEngine;

namespace Spell.Model.Actions
{
    public abstract class SpellAction
    {
        public abstract void Apply(ActionContext context);
    }

    public record ActionContext
    {
        public List<GameObject> Targets { get; init; }  // action 적용대상
        public GameObject Origin { get; init; }         // action을 실행한 오브젝트
        public float Value { get; init; }               // action 값
        public ElementType OriginElement { get; init; } // origin의 원소타입

        public ActionContext(SpellActionData actionData, GameObject activator, GameObject origin)
        {
            GameObject caster = origin.GetComponent<SpellBehaviorBase>().Caster;

            Targets = actionData.Target switch
            {
                TargetType.Activator => new List<GameObject>() { activator },
                TargetType.Caster => new List<GameObject>() { caster },
                // TargetType.Global => TODO: BattleManager.GetAllPlayers() 같은거 필요함,
                _ => null
            };
            Origin = origin;
            Value = actionData.Value;
            OriginElement = origin.GetComponent<IElementProvider>().Element;
        }
    }
}