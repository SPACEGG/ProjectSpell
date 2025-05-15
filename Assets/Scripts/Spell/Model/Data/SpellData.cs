using System.Collections.Generic;
using Spell.Model.Enums;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

namespace Spell.Model.Data
{
    /// <summary>
    /// Gpt의 응답으로 사용할 클래스이자 Spell를 생성할 때 필요한 데이터 클래스.
    /// </summary>
    public class SpellData
    {
        // SpellFactory에서 참조하는 속성
        public string Name { get; init; } // 스펠 이름 (GameObject 이름, 실패 체크)
        public ElementType Element { get; init; } // 원소 속성 (아직 동작 미구현)
        public BehaviorType Behavior { get; init; } // 동작 타입 (Projectile 등, 컴포넌트 결정)
        public List<SpellActionData> Actions { get; init; } // 주문 효과(데미지 등, 미구현)
        public ShapeType Shape { get; init; } // 외형(Shape) 결정 (ApplyVFX에서 사용)
        public Vector3 Size { get; init; } // 외형(크기) 결정 (ApplyVFX에서 사용)

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
        public int Count { get; init; } // 오브젝트 개수 (for문 반복)
        public bool HasGravity { get; init; } // 중력 적용 여부 (Rigidbody.useGravity)
        public float Speed { get; init; } // 오브젝트 속도 (Rigidbody.linearVelocity)
        public float Duration { get; init; } // 오브젝트 수명 (DestroyAfterSeconds)
        public float SpreadAngle { get; init; } // 퍼짐 각도 (CalculateDirection)
        public float SpreadRange { get; init; } // 생성 위치 범위 (Spawn 메서드에서 Random.insideUnitSphere)
        public bool ActivateOnCollision { get; init; } // 충돌 시 활성화 여부 (Spawn 메서드에서 조건문)

        // 생성자에서 기본값 할당 제거
        public SpellData() { }
    }
}
