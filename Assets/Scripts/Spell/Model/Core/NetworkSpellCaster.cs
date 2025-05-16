using Common.Models;
using Cysharp.Threading.Tasks;
using Spell.Model.Behaviors;
using Spell.Model.Data;
using Unity.Netcode;
using UnityEngine;

namespace Spell.Model.Core
{
    // FIXME: 변경사항 많음
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


        [Rpc(SendTo.Server)]
        public void CastDefaultSpellRpc()
        {
            CastSpell(_defaultSpell, gameObject);
        }

        [Rpc(SendTo.Server)]
        public void CastUltimateSpellRpc(CastUltimateSpellRpcArgs args)
        {
            _ = BuildAndCastSpell(args.Recording);
        }

        private async UniTask BuildAndCastSpell(Wav recording)
        {
            var spellData = await _spellDataController.BuildSpellDataAsyncByWav(
                recording,
                1,
                Camera.main ? Camera.main.transform.position : Vector3.zero,
                transform.position
            );

            CastSpell(spellData, gameObject);
        }

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

    public struct CastUltimateSpellRpcArgs : INetworkSerializable
    {
        public Wav Recording;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Recording);
        }
    }
}