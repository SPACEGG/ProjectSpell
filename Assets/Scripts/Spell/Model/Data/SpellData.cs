using System.Collections.Generic;
using Spell.Model.Enums;
using UnityEngine;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

namespace Spell.Model.Data
{
    /// <summary>
    /// Gpt의 응답으로 사용할 클래스이자 Spell를 생성할 때 필요한 데이터 클래스.
    /// </summary>
    // [CreateAssetMenu(fileName = "SpellData", menuName = "Scriptable Objects/SpellData")]
    public class SpellData
    {
        public string Name { get; init; }
        public ElementType Element { get; init; }
        public BehaviorType Behavior { get; init; }
        public List<SpellActionData> Actions { get; init; }

        public Vector3? PositionOffset { get; init; }
        public Vector3? Direction { get; init; }
        public int Count { get; init; }

        public ShapeType Shape { get; init; }
        public Vector3 Size { get; init; }
        public bool HasGravity { get; init; }
        public float Speed { get; init; }
        public float Duration { get; init; }

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
        }
    }
}
