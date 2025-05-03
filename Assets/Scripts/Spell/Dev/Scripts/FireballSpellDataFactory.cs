using System.Collections.Generic;
using Spell.Model.Data;
using Spell.Model.Enums;

namespace Spell.Dev.Premades
{
    public static class FireballSpellDataFactory
    {
        public static SpellData Create()
        {
            return new SpellData
            {
                Name = "Fireball",
                Element = ElementType.Fire,
                Behavior = BehaviorType.Projectile,

                Actions = new List<SpellActionData>
                {
                    new SpellActionData
                    {
                        Action = ActionType.Damage,
                        Target = TargetType.Enemy,
                    },
                }
            };
        }
    }
}