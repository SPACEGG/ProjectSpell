using Common.Models;
using Cysharp.Threading.Tasks;
using Spell.Model.Behaviors;
using Spell.Model.Data;
using Unity.Netcode;
using UnityEngine;
using Record;
using Player;

namespace Spell.Model.Core
{
    public class NetworkSpellCaster : NetworkBehaviour
    {
        [Header("Input Settings")]
        [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;
        [SerializeField] private KeyCode recordKey = KeyCode.Mouse1;
        [SerializeField] private KeyCode level1SelectKey = KeyCode.Alpha1;
        [SerializeField] private KeyCode level2SelectKey = KeyCode.Alpha2;
        [SerializeField] private KeyCode level3SelectKey = KeyCode.Alpha3;

        [SerializeField] private float defaultAttackDuration = 2f;
        [SerializeField] private float recordIgnoreDuration = 0.5f;
        [SerializeField] private Transform castOrigin;

        private float _recordStartTime;
        private float _defaultAttackCooldown = 0f;

        private RecordController _recordController;
        private SpellDataController _spellDataController;
        private NetworkHealthManaManager _healthManaManager;
        private NetworkPowerLevelManager _powerLevelManager;
        public Transform CastOrigin => castOrigin != null ? castOrigin : transform;

        #region Unity Events

        public override void OnNetworkSpawn()
        {
            _recordController = new();
            _spellDataController = SpellDataController.Singleton;
            _powerLevelManager = GetComponent<NetworkPowerLevelManager>();
            _healthManaManager = GetComponent<NetworkHealthManaManager>();

            if (_powerLevelManager == null)
            {
                Debug.LogError("PowerLevelManager component not found on the same GameObject!");
            }
        }

        private void Update()
        {
            if (!IsLocalPlayer)
            {
                return;
            }

            DefaultAttackKeyInput();
            RecordKeyInput();
        }

        #endregion

        #region Key Inputs

        private void DefaultAttackKeyInput()
        {
            if (Input.GetKeyDown(attackKey))
            {
                // 쿨타임 체크
                if (Time.time - _defaultAttackCooldown < defaultAttackDuration) return;
                _defaultAttackCooldown = Time.time;

                Debug.Log("Attack key pressed");
                Vector3 cameraTargetPosition = GetCameraTargetPosition();
                Vector3 direction = (cameraTargetPosition - CastOrigin.position).normalized;

                // 위로 살짝 올려 포물선 효과
                direction.y += 0.2f;
                direction = direction.normalized;

                // 디버그 로그 추가
                Debug.Log($"[DefaultAttack] cameraTargetPosition: {cameraTargetPosition}, CastOrigin.position: {CastOrigin.position}, direction: {direction}");

                var spell = SpellDataFactory.Create();
                spell.Direction = direction;

                // 주문 위치 계산 및 로컬 시전
                CastSpellAsOriginator(spell);
                // 서버에 동기화 요청
                RequestCastSpellRpc(spell, NetworkManager.Singleton.LocalClientId);
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
                    _ = CastSpellFromRecording();
                }
            }
        }

        #endregion

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

        private async UniTaskVoid CastSpellFromRecording()
        {
            // 소유 마나보다 더 큰 레벨 마법을 쓰면 레벨 -1으로 고정
            int powerLevel;
            bool isValidPowerLevel = _healthManaManager.ManaModel.UseMana(_powerLevelManager.PowerLevel);
            if (isValidPowerLevel) powerLevel = _powerLevelManager.PowerLevel;
            else powerLevel = -1;

            Vector3 cameraTargetPosition = GetCameraTargetPosition();

            SpellData spelldata = await _spellDataController.BuildSpellDataAsync(
                _recordController.GetRecordingClip(),
                powerLevel,
                cameraTargetPosition,
                CastOrigin.position
            );

            if (spelldata == null)
            {
                Debug.LogError("SpellData creation failed!");
                return;
            }

            int randomSeed = Random.Range(int.MinValue, int.MaxValue);

            // 주문 위치 계산 및 로컬 시전
            CastSpellAsOriginator(spelldata, randomSeed);
            // 서버에 동기화 요청
            RequestCastSpellRpc(spelldata, NetworkManager.Singleton.LocalClientId, randomSeed);
        }

        // 내가 시전한 주문 (로컬 입력 기반)
        public void CastSpellAsOriginator(SpellData spellData, int seed = 0)
        {
            // 캐릭터 앞쪽(예: 1.0f만큼)으로 오프셋
            spellData.PositionOffset = CastOrigin.forward * 1.0f; // 1.0f는 원하는 거리로 조절
            // 시전자 기준으로 스폰 위치 계산
            spellData.SpawnPosition = CastOrigin.position + spellData.PositionOffset;
            CastSpell(spellData, seed);
        }

        // 다른 플레이어가 시전한 주문 (서버를 통해 받은 SpellData 기반)
        public void CastSpellAsReceiver(SpellData spellData, int seed)
        {
            // 캐릭터 앞쪽(예: 1.0f만큼)으로 오프셋
            spellData.PositionOffset = CastOrigin.forward * 1.0f; // 1.0f는 원하는 거리로 조절
            // 이미 계산된 SpawnPosition 사용
            CastSpell(spellData, seed);
        }

        // 클라이언트에서 호출: SpellData를 서버로 전송, originClientId도 함께 전달
        [Rpc(SendTo.Server)]
        public void RequestCastSpellRpc(SpellData spellData, ulong originClientId, int seed = 0)
        {
            if (!IsServer) return;
            SyncSpellDataRpc(spellData, originClientId, seed);
        }

        // 서버에서 호출: 모든 인스턴스에서 SpellData로 주문 생성, 단 originClientId는 제외
        [Rpc(SendTo.Everyone)]
        public void SyncSpellDataRpc(SpellData spellData, ulong originClientId, int seed)
        {
            if (NetworkManager.Singleton.LocalClientId == originClientId)
                return; // 자기 자신이면 중복 시전 방지

            CastSpellAsReceiver(spellData, seed);
        }

        // 실제 스펠 생성 및 적용 (위치 계산 없이 SpawnPosition 사용)
        private void CastSpell(SpellData data, int seed)
        {
            if (data == null)
            {
                Debug.LogWarning("Tried to cast null spell.");
                return;
            }

            var spawnPosition = data.SpawnPosition;

            var spellObject = SpellFactory.CreateSpellGameObject(data, gameObject);
            spellObject.transform.position = spawnPosition;

            var casterColliders = GetComponentsInChildren<Collider>();
            var spellColliders = spellObject.GetComponentsInChildren<Collider>();
            foreach (var casterCol in casterColliders)
            {
                foreach (var spellCol in spellColliders)
                {
                    Physics.IgnoreCollision(spellCol, casterCol, true);
                }
            }

            var behavedSpellObject = spellObject.GetComponent<SpellBehaviorBase>();

            behavedSpellObject.RandomSeed = seed;
            behavedSpellObject.Behave(data);
        }
    }
}