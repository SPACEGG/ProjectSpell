using Spell.Model.Data;
using UnityEngine;

namespace Spell.Model.Behaviors
{
    public abstract class SpellBehaviorBase : MonoBehaviour
    {
        public GameObject Caster { get; set; }
        public int RandomSeed { get; set; }

        // Activate는 추상 메서드이므로 자식 클래스서 정의
        public abstract void Behave(SpellData spellData);
    }
}