using System.Collections.Generic;
using Spell.Model.Enums;
using UnityEngine;

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
        public Vector3 Size; // float Size -> Vector3 Size로 변경
        public bool HasGravity;
        public float Speed; // 추가: 투사체 속도 등
        public float Duration; // 추가: 투사체 지속 시간(초)
    }
}