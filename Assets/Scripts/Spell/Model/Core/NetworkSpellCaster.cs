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

        // 더 이상 RPC 필요 없음. 각자 입력에서 직접 호출
        public void CastSpellLocally(SpellData spellData)
        {
            CastSpell(spellData, gameObject);
        }

        private async UniTask BuildAndCastSpell(Wav recording)
        {
            var spellData = await _spellDataController.BuildSpellDataAsyncByWav(
                recording,
                1,
                Camera.main ? Camera.main.transform.position : Vector3.zero,
                transform.position
            );

            CastSpellLocally(spellData);
        }

        // 클라이언트에서 호출: SpellData를 서버로 전송, originClientId도 함께 전달
        [Rpc(SendTo.Server)]
        public void RequestCastSpellRpc(SpellData spellData, ulong originClientId)
        {
            // 서버에서만 실행
            if (!IsServer) return;

            // 모든 인스턴스(서버+클라)에 SpellData와 originClientId 브로드캐스트
            SyncSpellDataRpc(spellData, originClientId);
        }

        // 서버에서 호출: 모든 인스턴스에서 SpellData로 주문 생성, 단 originClientId는 제외
        [Rpc(SendTo.Everyone)]
        public void SyncSpellDataRpc(SpellData spellData, ulong originClientId)
        {
            if (NetworkManager.Singleton.LocalClientId == originClientId)
                return; // 자기 자신이면 중복 시전 방지

            CastSpell(spellData, gameObject);
        }

        // 실제 스펠 생성 및 적용 (서버에서만 실행)
        private void CastSpell(SpellData data, GameObject caster)
        {
            if (data == null)
            {
                Debug.LogWarning("Tried to cast null spell.");
                return;
            }

            // SpellData에서 PositionOffset을 가져와서 사용 (null이면 2칸 앞)
            var offset = data.PositionOffset;
            var spawnPosition = CastOrigin.position + offset;

            // SpellData 정보를 바탕으로 실제 동작/외형이 적용된 스펠 오브젝트 씬에 생성
            var spellObject = SpellFactory.CreateSpellGameObject(data, caster);

            spellObject.transform.position = spawnPosition;

            // === 시전자와 주문 오브젝트가 충돌하지 않도록 처리 ===
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