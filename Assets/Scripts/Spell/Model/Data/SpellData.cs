using System.Collections.Generic;
using Spell.Model.Enums;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Unity.Netcode; 

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

namespace Spell.Model.Data
{
    public class SpellData : INetworkSerializable
    {
        // SpellFactory에서 참조하는 속성
        public string Name { get; set; } // 스펠 이름 (GameObject 이름, 실패 체크)
        public ElementType Element { get; set; } // 원소 속성 (아직 동작 미구현)
        public BehaviorType Behavior { get; set; } // 동작 타입 (Projectile 등, 컴포넌트 결정)
        public List<SpellActionData> Actions { get; set; } // 주문 효과(데미지 등, 미구현)
        public ShapeType Shape { get; set; } // 외형(Shape) 결정 (ApplyVFX에서 사용)
        public Vector3 Size { get; set; } // 외형(크기) 결정 (ApplyVFX에서 사용)

        // VFX 관련 속성
        public string MaterialName { get; set; } // Materials 폴더용
        public string MeshName { get; set; }     // Meshes 폴더용
        public string ParticleName { get; set; } // Particles 폴더용
        public string TrailName { get; set; }    // TrailEffects 폴더용

        // Spell 동작(Behavior)에서 참조하는 속성
        [JsonConverter(typeof(Vector3Converter))]
        public Vector3 PositionOffset { get; set; } // Vector3로 변경
        [JsonConverter(typeof(Vector3Converter))]
        public Vector3 Direction { get; set; } // Vector3로 변경
        public int Count { get; set; } // 오브젝트 개수 (for문 반복)
        public bool HasGravity { get; set; } // 중력 적용 여부 (Rigidbody.useGravity)
        public float Speed { get; set; } // 오브젝트 속도 (Rigidbody.linearVelocity)
        public float Duration { get; set; } // 오브젝트 수명 (DestroyAfterSeconds)
        public float SpreadAngle { get; set; } // 퍼짐 각도 (CalculateDirection)
        public float SpreadRange { get; set; } // 생성 위치 범위 (Spawn 메서드에서 Random.insideUnitSphere)
        public bool ActivateOnCollision { get; set; } // 충돌 시 활성화 여부 (Spawn 메서드에서 조건문)
        [JsonIgnore]
        public Vector3 SpawnPosition { get; set; } // 네트워크 동기화용 실제 스폰 위치

        public SpellData() { }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            // 문자열, enum 등 기본 필드 직렬화
            var name = Name;
            serializer.SerializeValue(ref name);
            if (!serializer.IsWriter) Name = name;

            var element = Element;
            serializer.SerializeValue(ref element);
            if (!serializer.IsWriter) Element = element;

            var behavior = Behavior;
            serializer.SerializeValue(ref behavior);
            if (!serializer.IsWriter) Behavior = behavior;

            // SpellActionData 리스트 직렬화
            if (serializer.IsWriter)
            {
                int count = Actions?.Count ?? 0;
                serializer.SerializeValue(ref count);
                for (int i = 0; i < count; i++)
                {
                    var action = Actions[i];
                    action.NetworkSerialize(serializer);
                }
            }
            else
            {
                int count = 0;
                serializer.SerializeValue(ref count);
                Actions = new List<SpellActionData>(count);
                for (int i = 0; i < count; i++)
                {
                    var action = new SpellActionData();
                    action.NetworkSerialize(serializer);
                    Actions.Add(action);
                }
            }

            var shape = Shape;
            serializer.SerializeValue(ref shape);
            if (!serializer.IsWriter) Shape = shape;

            var size = Size;
            serializer.SerializeValue(ref size);
            if (!serializer.IsWriter) Size = size;

            var materialName = MaterialName;
            serializer.SerializeValue(ref materialName);
            if (!serializer.IsWriter) MaterialName = materialName;

            var meshName = MeshName;
            serializer.SerializeValue(ref meshName);
            if (!serializer.IsWriter) MeshName = meshName;

            var particleName = ParticleName;
            serializer.SerializeValue(ref particleName);
            if (!serializer.IsWriter) ParticleName = particleName;

            var trailName = TrailName;
            serializer.SerializeValue(ref trailName);
            if (!serializer.IsWriter) TrailName = trailName;

            var positionOffset = PositionOffset;
            serializer.SerializeValue(ref positionOffset);
            if (!serializer.IsWriter) PositionOffset = positionOffset;

            var direction = Direction;
            serializer.SerializeValue(ref direction);
            if (!serializer.IsWriter) Direction = direction;

            var countValue = Count;
            serializer.SerializeValue(ref countValue);
            if (!serializer.IsWriter) Count = countValue;

            var hasGravity = HasGravity;
            serializer.SerializeValue(ref hasGravity);
            if (!serializer.IsWriter) HasGravity = hasGravity;

            var speed = Speed;
            serializer.SerializeValue(ref speed);
            if (!serializer.IsWriter) Speed = speed;

            var duration = Duration;
            serializer.SerializeValue(ref duration);
            if (!serializer.IsWriter) Duration = duration;

            var spreadAngle = SpreadAngle;
            serializer.SerializeValue(ref spreadAngle);
            if (!serializer.IsWriter) SpreadAngle = spreadAngle;

            var spreadRange = SpreadRange;
            serializer.SerializeValue(ref spreadRange);
            if (!serializer.IsWriter) SpreadRange = spreadRange;

            var activateOnCollision = ActivateOnCollision;
            serializer.SerializeValue(ref activateOnCollision);
            if (!serializer.IsWriter) ActivateOnCollision = activateOnCollision;

            var spawnPosition = SpawnPosition;
            serializer.SerializeValue(ref spawnPosition);
            if (!serializer.IsWriter) SpawnPosition = spawnPosition;
        }
    }
}
