using System;
using Common.Utils;
using Cysharp.Threading.Tasks;
using Record;
using Spell.Model.Core;
using Spell.Model.Data;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    [Obsolete("This class is deprecated. Use NetworkDefaultAttackManager instead.")]
    [RequireComponent(typeof(NetworkSpellCaster), typeof(NetworkHealthManaManager), typeof(PowerLevelManager))]
    public class NetworkInputManager : NetworkBehaviour
    {
        [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;
        [SerializeField] private KeyCode recordKey = KeyCode.Mouse1;
        [SerializeField] private KeyCode level1SelectKey = KeyCode.Alpha1;
        [SerializeField] private KeyCode level2SelectKey = KeyCode.Alpha2;
        [SerializeField] private KeyCode level3SelectKey = KeyCode.Alpha3;
        [SerializeField] private float recordIgnoreDuration = 0.5f;

        private float _recordStartTime;

        private RecordController _recordController;
        private SpellDataController _spellController;
        private NetworkSpellCaster _spellCaster;
        private NetworkHealthManaManager _healthManaManager;
        private PowerLevelManager _powerLevelManager;

        private void Awake()
        {
            _recordController = new();
            _spellController = SpellDataController.Singleton;
            _powerLevelManager = GetComponent<PowerLevelManager>();
            _healthManaManager = GetComponent<NetworkHealthManaManager>();

            if (_powerLevelManager == null)
            {
                Debug.LogError("PowerLevelManager component not found on the same GameObject!");
            }
        }

        public override void OnNetworkSpawn()
        {
            _spellCaster = GetComponent<NetworkSpellCaster>();
        }

        private void Update()
        {
            if (!IsLocalPlayer) return;

            DefaultAttackKeyInput();
            RecordKeyInput();
            PowerLevelSelectKeyInput();
        }

        private void DefaultAttackKeyInput()
        {
            if (Input.GetKeyDown(attackKey))
            {
                Debug.Log("Attack key pressed");
                if (_spellCaster == null)
                {
                    Debug.LogError("NetworkSpellCaster is null!");
                    return;
                }

                Vector3 cameraTargetPosition = GetCameraTargetPosition();
                Vector3 direction = (cameraTargetPosition - transform.position).normalized;

                var spell = SpellDataFactory.Create();
                spell.Direction = direction;

                _spellCaster.CastDefaultSpellRpc();
            }
        }

        private void PowerLevelSelectKeyInput()
        {
            if (Input.GetKeyDown(level1SelectKey))
            {
                _powerLevelManager.SetPowerLevel(1);
            }
            else if (Input.GetKeyDown(level2SelectKey))
            {
                _powerLevelManager.SetPowerLevel(2);
            }
            else if (Input.GetKeyDown(level3SelectKey))
            {
                _powerLevelManager.SetPowerLevel(3);
            }
        }

        private void RecordKeyInput()
        {
            if (Input.GetKeyDown(recordKey))
            {
                Debug.Log("Record key pressed");
                _recordController.StartRecording();
                _recordStartTime = Time.time;
            }

            if (Input.GetKeyUp(recordKey))
            {
                Debug.Log("Record key released");
                _recordController.StopRecording();

                if (Time.time - _recordStartTime >= recordIgnoreDuration)
                {
                    _ = CastSpell();
                }
            }
        }

        private Vector3 GetCameraTargetPosition()
        {
            if (Camera.main == null)
            {
                Debug.LogWarning("Camera.main is null!");
                return Vector3.zero;
            }

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            int layerMask = ~LayerMask.GetMask("Player");

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask))
            {
                return hit.point;
            }

            return ray.GetPoint(100f);
        }

        private async UniTaskVoid CastSpell()
        {
            // FIXME: 마나 소모가 가능한 경우에만 사용하도록 수정 필요
            _healthManaManager.ManaModel.UseMana(_powerLevelManager.CurrentPowerLevel);

            Vector3 cameraTargetPosition = GetCameraTargetPosition();

            SpellData spelldata = await _spellController.BuildSpellDataAsync(
                _recordController.GetRecordingClip(),
                _powerLevelManager.CurrentPowerLevel,
                cameraTargetPosition,
                transform.position
            );

            if (spelldata == null)
            {
                Debug.LogError("SpellData creation failed!");
                return;
            }

            // Fixme: 스펠 사용 로직 수정 필요
            // _spellCaster.CastUltimateSpellRpc();
        }
    }
}