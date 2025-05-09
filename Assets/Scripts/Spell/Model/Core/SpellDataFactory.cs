using Spell.Model.Enums;
using System.Collections.Generic;
using UnityEngine;
using Spell.Model.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Spell.Model.Core
{
    public static class SpellDataFactory
    {
        // 시스템 오류 방지용 최소값만 채움. 실제 주문 실패 시에는 FailureBehavior로 분기할 것!
        public static SpellData Create()
        {
            return new SpellData();
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
            Vector3 size, 
            bool hasGravity,
            float speed,        
            float duration,
            string vfxName = null // VfxName 파라미터 추가 (기본값 null)
        )
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
                HasGravity = hasGravity,
                Speed = speed,       
                Duration = duration,
                VfxName = vfxName // 할당
            };
        }

        // JSON 문자열을 SpellData로 파싱
        // UnityEngine.JsonUtility는 UnityEngine.Vector3만 지원
        public static SpellData FromJson(string json)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = { new StringEnumConverter(), new Vector3Converter() }
            };
            return JsonConvert.DeserializeObject<SpellData>(json, settings);
        }

        // JSON 문자열을 SpellData로 파싱 (예외 처리 포함)
        public static SpellData SafeFromJson(string json)
        {
            try
            {
                var spellData = FromJson(json);
                Debug.Log($"Parsed SpellData: {spellData}");
                return spellData;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to parse SpellData: {ex.Message}\nJSON: {json}");
                return null;
            }
        }
    }
}
