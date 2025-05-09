using System.Collections.Generic;
using Spell.Model.Enums;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Spell.Model.Data
{
    /// <summary>
    /// Gpt의 응답으로 사용할 클래스이자 Spell를 생성할 때 필요한 데이터 클래스.
    /// </summary>
    public record SpellData
    {
        public string Name;
        public ElementType Element;
        public BehaviorType Behavior;
        public List<SpellActionData> Actions;

        public Vector3? PositionOffset;
        public Vector3? Direction;
        public int Count;

        public ShapeType Shape;
        public Vector3 Size;
        public bool HasGravity;
        public float Speed;
        public float Duration;

        [JsonConverter(typeof(StringEnumConverter))]
        public VfxNameType VfxName { get; set; } // VFX 머티리얼/이펙트 이름 필드 enum 타입으로 변경

        // 생성자에서 기본값 할당
        public SpellData()
        {
            Name = "DefaultSpell";
            Element = ElementType.None;
            Behavior = BehaviorType.Projectile;
            Actions = new List<SpellActionData>();
            PositionOffset = Vector3.zero; // 0으로 기본값 지정
            Direction = Vector3.forward; // 앞방향으로 기본값 지정
            Count = 1;
            Shape = ShapeType.Sphere;
            Size = Vector3.zero;
            HasGravity = false;
            Speed = 0f;
            Duration = 0f;
            VfxName = VfxNameType.None;
        }
    }
}