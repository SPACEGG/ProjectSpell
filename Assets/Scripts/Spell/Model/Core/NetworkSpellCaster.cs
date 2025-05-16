using System.Collections.Generic;
using Common.Models;
using Cysharp.Threading.Tasks;
using Spell.Model.Data;
using Spell.Model.Enums;
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


        [Rpc(SendTo.Server)]
        public void CastDefaultSpellRpc()
        {
            CastSpell(_defaultSpell);
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
                transform.position,
                CameraUtil.GetCameraForward()
            );

            CastSpell(spellData);
        }

        private void CastSpell(SpellData data)
        {
            if (data == null)
            {
                Debug.LogWarning("Tried to cast null spell.");
                return;
            }

            // SpellData에서 PositionOffset을 가져와서 사용 (null이면 2칸 앞)
            var offset = data.PositionOffset ?? new Vector3(0f, 0f, 2f);
            var spawnPosition = CastOrigin.position + offset;

            // SpellData 정보를 바탕으로 실제 동작/외형이 적용된 스펠 오브젝트 씬에 생성
            var spellBehavior = SpellFactory.CreateSpellGameObject(data);
            spellBehavior.transform.SetPositionAndRotation(spawnPosition, CastOrigin.rotation);
            spellBehavior.Behave(data);
            // 이 이후에 spellBehavior의 Start()가 실행됨
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