using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Record;
using Spell.Model.Core;
using Spell.Model.Data;
using Spell.Model.Enums;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class NetworkInputManager : NetworkBehaviour
    {
        [SerializeField]
        private KeyCode attackKey = KeyCode.Mouse0;
        [SerializeField]
        private KeyCode recordKey = KeyCode.Mouse1;
        [SerializeField]
        private float recordIgnoreDuration = 0.5f;

        private SpellData _defaultSpell;
        private float _recordStartTime;

        private RecordController _recordController;
        private SpellDataController _spellController;
        private NetworkSpellCaster _spellCaster;

        private void Awake()
        {
            _recordController = new();
            _spellController = new();
        }

        public override void OnNetworkSpawn()
        {
            _spellCaster = GetComponent<NetworkSpellCaster>();

            _defaultSpell = SpellDataFactory.Create(
                "Default Spell",
                ElementType.Earth,
                BehaviorType.Projectile,
                new List<SpellActionData>() { new(ActionType.Damage, TargetType.Enemy, 10) },
                Vector3.zero,
                Vector3.forward,
                1,
                ShapeType.Sphere,
                Vector3.one,
                true,
                10f,
                5f
            );
        }

        private void Update()
        {
            if (!IsLocalPlayer) return;

            if (Input.GetKeyDown(attackKey))
            {
                _spellCaster.CastSpellRpc(_defaultSpell);
            }

            if (Input.GetKeyDown(recordKey))
            {
                _recordController.StartRecording();
                // uiController.ShowRecordIcon();
                _recordStartTime = Time.time;
            }

            if (Input.GetKeyUp(recordKey))
            {
                _recordController.StopRecording();
                // uiController.HideRecordIcon();

                if (Time.time - _recordStartTime >= recordIgnoreDuration)
                {
                    // UseSpellServerRpc().Forget();
                }
            }
        }

        // TODO: 서버에서 스펠을 사용하는 로직 추가
        // Spell 사용
        // [ServerRpc]
        // private async UniTaskVoid UseSpellServerRpc()
        // {
        //     var spellData = await _spellController.BuildSpellDataAsync(
        //         _recordController.GetRecordingClip(),
        //         1,
        //         Camera.main ? Camera.main.transform.position : Vector3.zero,
        //         transform.position
        //     );
        //
        //     // TODO: 마나 소모
        //     // healthManaManager.ManaModel.UseMana(200f);
        //     _spellCaster.CastSpell(spellData);
        // }
    }
}