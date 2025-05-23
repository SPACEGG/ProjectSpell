using System;
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
            Array materialNames = Enum.GetValues(typeof(MaterialNameType));
            Array meshNames = Enum.GetValues(typeof(MeshNameType));
            int randomIndex = UnityEngine.Random.Range(0, materialNames.Length);
            string materialName = ((MaterialNameType)materialNames.GetValue(randomIndex)).ToString();
            randomIndex = UnityEngine.Random.Range(0, meshNames.Length);
            string meshName = ((MeshNameType)meshNames.GetValue(randomIndex)).ToString();


            return new SpellData
            {
                Name = "DefaultSpell",
                Element = ElementType.Common,
                Behavior = BehaviorType.Projectile,
                Actions = new List<SpellActionData>()
                {
                    new(ActionType.Damage, TargetType.Activator, 20f),
                    new(ActionType.ManaRegen, TargetType.Caster, 20f)
                },
                PositionOffset = Vector3.zero,
                Direction = Vector3.forward,
                Count = 1,
                Shape = ShapeType.Sphere,
                Size = Vector3.one * 0.8f,
                HasGravity = true,
                Speed = 30f,
                Duration = 5f,
                SpreadAngle = 0f,
                SpreadRange = 0f,
                ActivateOnCollision = true,
                // ---- VFX 속성 랜덤 세팅 ----
                MaterialName = materialName,
                MeshName = meshName,
                ParticleName = "None",
                TrailName = "None",
                SoundName = "None"
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
            int? count,
            ShapeType? shape,
            Vector3? size,
            bool? hasGravity,
            float? speed,
            float? duration,
            float? spreadAngle,
            float? spreadRange,
            bool? activateOnCollision,
            string materialName,
            string meshName,
            string particleName,
            string trailName
        )
        {
            // 모든 속성에 대해 디폴트 처리
            if (string.IsNullOrEmpty(name))
                name = "DefaultSpell";
            if (actions == null)
                actions = new List<SpellActionData>();
            if (positionOffset == null)
                positionOffset = Vector3.zero;
            if (direction == null)
                direction = Vector3.forward;
            if (count == null || count <= 0)
                count = 1;
            if (shape == null)
                shape = ShapeType.Sphere;
            if (size == null || size == Vector3.zero)
                size = Vector3.one;
            if (hasGravity == null)
                hasGravity = false;
            if (speed == null || speed <= 0f)
                speed = 10f;
            if (duration == null || duration <= 0f)
                duration = 5f;
            if (spreadAngle == null)
                spreadAngle = 0f;
            if (spreadRange == null)
                spreadRange = 0f;
            if (activateOnCollision == null)
                activateOnCollision = true;
            if (element == 0) // enum의 기본값이 None일 경우
                element = ElementType.Earth;
            if (string.IsNullOrEmpty(materialName))
                materialName = "None";
            if (string.IsNullOrEmpty(meshName))
                meshName = "None";
            if (string.IsNullOrEmpty(particleName))
                particleName = "None";
            if (string.IsNullOrEmpty(trailName))
                trailName = "None";

            return new SpellData
            {
                Name = name,
                Element = element,
                Behavior = behavior,
                Actions = actions,
                PositionOffset = positionOffset.Value, // 명시적 .Value 사용
                Direction = direction.Value,           // 명시적 .Value 사용
                Count = count.Value,
                Shape = shape.Value,
                Size = size.Value,
                HasGravity = hasGravity.Value,
                Speed = speed.Value,
                Duration = duration.Value,
                SpreadAngle = spreadAngle.Value,
                SpreadRange = spreadRange.Value,
                ActivateOnCollision = activateOnCollision.Value,
                MaterialName = materialName,
                MeshName = meshName,
                ParticleName = particleName,
                TrailName = trailName
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
                // ...기존 VfxName 변환 코드는 필요 없음 (JsonConverter로 자동 변환됨)
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
