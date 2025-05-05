using Spell.Model.Data;
using UnityEngine;

namespace Spell.Model.Behaviors
{
    public abstract class SpellBehaviorBase : MonoBehaviour
    {
        public SpellData Data;
        
        // Activate는 추상 메서드이므로 자식 클래스서 정의
        public abstract void Behave(Vector3 spawnPosition);
    }
}