using Spell.Dev.Premades;
using Spell.Model.Core;
using UnityEngine;

namespace Spell.Dev.Scripts
{
    public class DevSpellCasterManager : MonoBehaviour
    {
        [SerializeField] private SpellCaster spellCaster;

        private void Start()
        {
            var fireballSpellData = FireballSpellDataFactory.Create();
            spellCaster.CastSpell(fireballSpellData, Vector3.forward);
        }
    }
}