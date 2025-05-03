namespace Spell.Model.Actions
{
    public abstract class SpellAction
    {
        public abstract void Apply(SpellContext context);
    }

    public abstract record SpellContext
    {
    }
}