using Spell.Model.Data;
using UnityEngine;

namespace Spell.Model.Behaviors
{
    /// <summary>
    /// 주문 사용 실패시
    /// </summary>
    public class FailureBehavior : SpellBehaviorBase
    {
        public override void Behave(SpellData spellData)
        {
            Debug.LogWarning("FailureBehavior: 스펠 동작 실패! (정의되지 않은 BehaviorType)");
            // 필요시 실패 이펙트 등 추가
        }
    }
}