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
        public GameObject Target { get; init; }
        public GameObject Origin { get; init; }
        public float Value { get; init; }
        public ElementType OriginElement { get; init; }
    }
}