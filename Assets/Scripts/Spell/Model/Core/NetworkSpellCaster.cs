using Common.Models;
using Cysharp.Threading.Tasks;
using Spell.Model.Behaviors;
using Spell.Model.Data;
using Unity.Netcode;
using UnityEngine;

namespace Spell.Model.Core
{
    public class NetworkSpellCaster : NetworkBehaviour
    {
        [SerializeField] private Transform castOrigin; // 지팡이 끝부분

        private readonly SpellDataController _spellDataController = SpellDataController.Singleton;

        public Transform CastOrigin => castOrigin ? castOrigin : Camera.main?.transform; // 여기서 transform 사용

        private SpellData _defaultSpell;

        public override void OnNetworkSpawn()
        {
            _defaultSpell = SpellDataFactory.Create();
        }

        // 내가 시전한 주문 (로컬 입력 기반)
        public void CastSpellAsOriginator(SpellData spellData)
        {
            // 시전자 기준으로 스폰 위치 계산
            spellData.SpawnPosition = CastOrigin.position + spellData.PositionOffset;
            CastSpell(spellData);
        }

        // 다른 플레이어가 시전한 주문 (서버를 통해 받은 SpellData 기반)
        public void CastSpellAsReceiver(SpellData spellData)
        {
            // 이미 계산된 SpawnPosition 사용
            CastSpell(spellData);
        }

        private async UniTask BuildAndCastSpell(Wav recording)
        {
            var spellData = await _spellDataController.BuildSpellDataAsyncByWav(
                recording,
                1,
                Camera.main ? Camera.main.transform.position : Vector3.zero,
                transform.position
            );

            CastSpellAsOriginator(spellData);
        }

        // 클라이언트에서 호출: SpellData를 서버로 전송, originClientId도 함께 전달
        [Rpc(SendTo.Server)]
        public void RequestCastSpellRpc(SpellData spellData, ulong originClientId)
        {
            if (!IsServer) return;
            SyncSpellDataRpc(spellData, originClientId);
        }

        // 서버에서 호출: 모든 인스턴스에서 SpellData로 주문 생성, 단 originClientId는 제외
        [Rpc(SendTo.Everyone)]
        public void SyncSpellDataRpc(SpellData spellData, ulong originClientId)
        {
            if (NetworkManager.Singleton.LocalClientId == originClientId)
                return; // 자기 자신이면 중복 시전 방지

            CastSpellAsReceiver(spellData);
        }

        // 실제 스펠 생성 및 적용 (위치 계산 없이 SpawnPosition 사용)
        private void CastSpell(SpellData data)
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
            behavedSpellObject?.Behave(data);
        }
    }
}