namespace Spell.Model.Actions
{
    public class DamageAction : SpellAction
    {
        public float Damage;

        public DamageAction(float amount)
        {
            Damage = amount;
        }

        public override void Apply(SpellContext context)
        {
            // TODO: Implement damage application logic
        }
    }
}