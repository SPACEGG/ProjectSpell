namespace Spell.Model.Actions
{
    public abstract class SpellAction
    {
        public abstract void Apply(ActionContext context);
    }

    public abstract record ActionContext
    {
    }
}