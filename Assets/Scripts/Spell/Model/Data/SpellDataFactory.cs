using Spell.Model.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Spell.Model.Data
{
    public static class SpellDataFactory
    {
        // 기본값 생성
        public static SpellData Create()
        {
            return new SpellData
            {
                Name = "DefaultSpell",
                Element = ElementType.None,
                Behavior = BehaviorType.Projectile,
                Actions = new List<SpellActionData>(),
                PositionOffset = Vector3.zero,
                Direction = null,
                Count = 1,
                Shape = ShapeType.None,
                Size = 1f,
                HasGravity = false
            };
        }

        // 파싱된 값으로 생성 (GPT 응답 등)
        public static SpellData Create(
            string name,
            ElementType element,
            BehaviorType behavior,
            List<SpellActionData> actions,
            Vector3? positionOffset,
            Vector3? direction,
            int count,
            ShapeType shape,
            float size,
            bool hasGravity)
        {
            return new SpellData
            {
                Name = name,
                Element = element,
                Behavior = behavior,
                Actions = actions ?? new List<SpellActionData>(),
                PositionOffset = positionOffset,
                Direction = direction,
                Count = count,
                Shape = shape,
                Size = size,
                HasGravity = hasGravity
            };
        }
    }
}
