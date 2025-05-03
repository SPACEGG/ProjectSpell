using Spell.Model.Enums;

namespace Spell.Model.Data
{
    public record SpellActionData
    {
        public ActionType Action;
        public TargetType Target;
    }
}