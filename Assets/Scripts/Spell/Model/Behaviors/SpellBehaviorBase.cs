using Spell.Model.Data;
using UnityEngine;

namespace Spell.Model.Behaviors
{
    public abstract class SpellBehaviorBase : MonoBehaviour
    {
        public SpellData Data;
        public abstract void Activate(Vector3 spawnPosition, Transform caster);
    }
}