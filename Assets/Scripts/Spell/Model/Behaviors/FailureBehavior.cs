using JetBrains.Annotations;
using Spell.Model.Data;
using UnityEngine;

namespace Spell.Model.Behaviors
{
    /// <summary>
    /// 주문 사용 실패시
    /// </summary>
    public class FailureBehavior : SpellBehaviorBase
    {
        public override void Behave([CanBeNull] SpellData spellData)
        {
            Destroy(this, 1f);
        }
    }
}