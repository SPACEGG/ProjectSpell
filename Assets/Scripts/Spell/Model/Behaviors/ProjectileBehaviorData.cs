using UnityEngine;

namespace Spell.Model.Behaviors
{
    [CreateAssetMenu(fileName = "ProjectileBehaviorData", menuName = "Scriptable Objects/Spell/ProjectileBehaviorData")]
    public class ProjectileBehaviorData : ScriptableObject
    {
        public float Speed = 10f;
    }
}