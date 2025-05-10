using Spell.Model.Enums;

namespace Spell.Model.Data
{
    public record SpellActionData
    {
        public ActionType Action;
        public TargetType Target;
        public float Value;

        public SpellActionData(ActionType action, TargetType target, float value)
        {
            Action = action;
            Target = target;
            Value = value;
        }
    }
}