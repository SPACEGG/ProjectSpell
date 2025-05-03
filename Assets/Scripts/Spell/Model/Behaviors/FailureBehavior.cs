using UnityEngine;

namespace Spell.Model.Behaviors
{
    /// <summary>
    /// 주문 사용 실패시
    /// </summary>
    public class FailureBehavior : SpellBehaviorBase
    {
        public override void Activate(Vector3 targetPosition, Transform caster)
        {
            Debug.Log($"Spell {Data.Name} failed to cast.");
        }
    }
}