using System;
using Spell.Model.Core;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    [Obsolete("Use NetworkSpellCaster instead.")]
    public class NetworkDefaultAttackManager : NetworkBehaviour
    {
        [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;
        [SerializeField] private Transform castOrigin;

        private SpellCaster spellCaster;

        public override void OnNetworkSpawn()
        {
            if (!IsLocalPlayer) Destroy(this);

            spellCaster = GetComponent<SpellCaster>();
        }

        private void Update()
        {
            DefaultAttackKeyInput();
        }

        private void DefaultAttackKeyInput()
        {
            if (Input.GetKeyDown(attackKey))
            {
                if (spellCaster == null)
                {
                    Debug.LogError("SpellCaster is null!");
                }
                else
                {
                    var spell = SpellDataFactory.Create();
                    spell.Direction = castOrigin.forward;

                    Debug.Log("CastSpell 호출");
                    spellCaster.CastSpell(spell, gameObject);
                }
            }
        }
    }
}