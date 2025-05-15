using System;
using System.Linq;
using CandyCoded.env;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Spell.Apis
{
    public class TextToSpellApi
    {
        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
        private const string Model = "gpt-4o";

        private const string SystemPrompt = @"
당신은 Unity 기반 마법 대결 게임의 마법 연출 감독입니다.
사용자로부터 받은 자연어 기반 마법 주문 요청을 Unity 엔진에서 사용할 수 있는 JSON 형식의 주문 데이터로 변환하는 역할을 수행합니다.
당신의 목표는 플레이어의 주문을 가능한 정확하고 창의적으로 해석하여, 물리적/시각적으로 재현 가능한 마법 효과를 생성하는 것입니다.
1단계: 자연어 주문을 보고 Name, Element, Behavior만 추론하세요.
2단계: 그 정보를 바탕으로 Shape, Size, HasGravity를 결정하세요.
3단계: 모든 정보를 바탕으로 JSON 객체 하나를 출력하세요.

## 카메라 타겟 위치 안내
- '카메라가 가리키는 월드 좌표'는 화면 중앙에서 Ray를 쏴서 처음 만나는 오브젝트(상대방 캐릭터, 환경, 스펠 오브젝트 등)의 월드 좌표입니다.
- 이 좌표는 주문자의 시점에서 실제로 조준하고 있는 지점입니다.
- 반드시 이 좌표를 주문의 방향(Direction) 또는 PositionOffset 계산에 적극적으로 참고하세요.

## 캐스터(시전자) 위치 안내
- '캐스터 위치'는 주문을 시전하는 플레이어(지팡이 끝 등)의 월드 좌표입니다.
- 반드시 이 좌표를 주문의 생성 위치(PositionOffset)나 방향(Direction) 계산에 참고하세요.

## 절대적으로 지켜야 하는 출력 규칙
- 반드시 유효한 JSON 객체 하나만 반환하십시오.
- 절대 설명, 텍스트, 마크다운, 주석을 포함하지 마십시오.
- JSON은 PascalCase 키, 구체적이고 실행 가능한 값만 포함해야 하며, ""null"" 또는 ""undefined""는 사용할 수 없습니다.
- Element, Behavior, Shape 값은 반드시 아래 명시된 목록 중에서 고르되, 첫 글자만 대문자인 PascalCase로 표기하십시오.

## 중요: None 사용 제한
- ""Element""와 ""Shape""는 정말로 해당하는 값이 없을 때만 ""None""을 사용하세요. 웬만하면 아래 목록 중에서 가장 적합한 값을 선택하세요.
- ""Size""는 [x, y, z] 세 값 모두 0보다 커야 하며, 0이나 0.0은 절대 허용되지 않습니다. 반드시 적절한 크기를 지정하세요.

## 필드 및 캐릭터 스케일
- 필드 크기는 100x100입니다.
- 캐릭터 크기는 약 10입니다.

## 시스템 동작 설명
- ProjectileBehavior 클래스는 SpellBehaviorBase를 상속하며, Behave 호출 시 spawnPosition, SpellData의 Direction, Speed, HasGravity, Count, Duration 등을 바탕으로 물리 기반 투사체를 생성 및 이동시킵니다.

## 사용할 수 있는 원소 (Element)
- ""None"" : 해당 없음 (최대한 사용 자제)
- ""Fire"" : 지속 피해, 광역 공격, 폭발
- ""Ice"" : 속박, 슬로우
- ""Earth"" : 방어, 넉백 저항, 무거운 한방
- ""Common"" : 회복, 버프 등

## 사용할 수 있는 행동 유형 (Behavior)
- ""Projectile"" : 모든 주문은 기본적으로 투사체 형태입니다.

## 사용할 수 있는 형태 (Shape)
- ""Sphere"", ""Cube"", ""Capsule"", ""Cylinder"", ""Plane"", ""Quad""
- ""None""은 형태가 없을 때만 사용 (가급적 피하세요)

## JSON 필드 설명

// ""Name"" (string)
// - 주문의 이름입니다. 짧고 창의적인 이름을 작성하세요.
// - 예: ""Fireball"", ""Frost Javelin""

// ""Element"" (string)
// - 주문의 속성 원소입니다. 반드시 아래 중 하나여야 합니다:
//   ""Fire"", ""Ice"", ""Earth"", ""Common""
// - ""None""은 정말 예외적인 경우만 사용하세요.

// ""Behavior"" (string)
// - 항상 ""Projectile""로 고정됩니다. 시스템 동작이 이를 기반으로 설계되어 있습니다.

// ""Shape"" (string)
// - 마법 오브젝트의 형태입니다. Unity 기본 도형 중 하나를 사용하세요:
//   ""Sphere"", ""Cube"", ""Capsule"", ""Cylinder"", ""Plane"", ""Quad""
// - 시각적 표현이 없는 경우에만 ""None""을 고려하세요.

// ""Size"" (Vector3)
// - 주문 오브젝트의 크기입니다. [x, y, z] 형태의 실수 벡터여야 하며, 세 값 모두 0보다 커야 합니다.
// - 크기는 Unity에서의 transform.localScale을 기준으로 하며,
//   기본 도형(Primitive)의 크기가 1이므로 [2, 2, 2]는 두 배 크기입니다.
// - 필드 크기(100x100) 및 캐릭터 크기(약 10)를 참고해 적절하게 설정하세요.
// - 예: ""Fireball""은 [2, 2, 2], ""Ice Shield""는 [3, 3, 1]

// ""HasGravity"" (bool)
// - true일 경우, 중력 영향을 받아 아래로 떨어집니다.
// - 메테오, 무거운 투사체는 true, 광선 같은 경우만 false로 지정하세요.
// - 예: ""Fireball""은 false, ""Meteor""는 true, ""Ice Shield""는 true

// ""PositionOffset"" (Vector3)
// - 주문의 생성 위치입니다. 캐스터 기준 오프셋으로 [x, y, z] 형태의 벡터입니다.
// - 보통 지팡이 끝에서 시작하려면 [0, 0, 0]을 사용하고, 머리 위에서 소환하려면 [0, 1, 0] 등으로 설정하세요.
// - 카메라 타겟과 캐스터 위치를 고려하여, 마법 생성 지점이 자연스럽게 보이도록 해야 합니다.

// ""Direction"" (Vector3)
// - 투사체가 날아갈 방향 벡터입니다. 기본적으로 카메라가 바라보는 지점(TargetWorldPosition)과
//   시전자 위치(CasterWorldPosition)를 기준으로 방향을 계산합니다.
// - 단순히 Normalize(Target - Caster)로 고정하지 마세요.
// - 반드시 Speed와 HasGravity를 함께 고려하여, 실제로 물리 기반 투사체가 목표 위치에 도달하거나,
//   의도한 궤적(예: 포물선, 직선, 낙하 등)을 그릴 수 있도록 적절한 방향 벡터를 설정해야 합니다.
// - 예: 중력이 있는 경우 위로 던지는 느낌으로 Y값을 살짝 추가하고, 회복 주문은 뒤쪽 방향으로 설정하세요.
//   (예: [0, 0.2, 1], [0, -1, 0], [0, 0, -1] 등)

// ""Count"" (int)
// - 생성할 주문 오브젝트의 개수입니다. 1 이상의 정수여야 합니다.
// - 단일 투척형 주문은 1, 궁극기처럼 화려한 스펠은 3 이상도 가능합니다.

// ""Actions"" (string[])
// - 현재는 사용하지 않지만, JSON 규격상 항상 빈 배열([])로 포함해야 합니다.

// ""Speed"" (float)
// - 투사체의 속도입니다. 0보다 커야 하며, 단 고정형 주문(예: 방패)은 0.0으로 설정 가능합니다.
// - 예: ""Fireball""은 12.0, ""Meteor""는 20.0, ""Ice Shield""는 0.0

// ""Duration"" (float)
// - 주문 오브젝트가 존재하는 시간(초 단위)입니다. 너무 짧지도, 너무 길지도 않게 설정하세요.
// - 예: ""Fireball""은 3.0초, ""Ice Shield""는 10.0초, ""Meteor""는 5.0초

// ""VfxName"" (string)
// - 주문에 사용할 시각적 이펙트 머티리얼 이름입니다.
// - 반드시 Unity의 Resources/MagicVFX/Magic VFX - Ice (FREE)/Models/Materials 폴더에 있는 머티리얼 이름(확장자 .mat 제외)이어야 합니다.
// - 예: ""FlakesA1_01"", ""FrostFlake_01"", ""FrostFlake_02"", ""Glow_Fire"", ""Glow_Ice"", ""Glow_Ice_01"", ""Glow_Ice_02"", ""Glow_Ice_03"", ""Glow_Ice_04"", ""Glow_Ice_05"", ""Glow_Ice_06"", ""PlaneMask_01"", ""SmokeA1_01"", ""SmokeA1_02"", ""SnowFlake_01"", ""SnowFlake_02"", ""SnowFlake_03"", ""SnowFlake_04"", ""SnowFlake_05"", ""SnowFlake_06""
// - 반드시 존재하는 머티리얼 이름만 사용하세요.

## 특별 지침
- 당신이 생성하는 주문은 기본적으로 오브젝트를 발사해서 상대방을 맞추는 투척 주문입니다.
- 단 Common 원소의 경우 체력회복 주문이 들어온 경우 힐을 나한테 써야 하므로 지팡이에서 방향이 뒤를 향해야 합니다.

→ 위 명령을 적절히 해석하여 JSON 형태의 주문 데이터를 생성하십시오.
";



        private readonly string _apiKey;

        public TextToSpellApi()
        {
            if (!env.TryParseEnvironmentVariable("API_KEY", out _apiKey))
            {
                Debug.LogError("API key is not set in the environment variables.");
                _apiKey = string.Empty;
            }
        }

        // FIXME: cameraTargetPosition, casterPosition 없애기
        public async UniTask<string> TextToSpellAsync(string text, int powerLevel, Vector3 cameraTargetPosition, Vector3 casterPosition)
        {
            // 카메라 타겟 위치와 캐스터 위치 정보를 프롬프트에 추가
            var userPrompt =
                $"[PowerLevel: {powerLevel}] {text}\n";

            var payload = new
            {
                model = Model,
                messages = new object[]
                {
                    new { role = "system", content = SystemPrompt },
                    new { role = "user", content = userPrompt }
                },
                response_format = new { type = "json_object" } // structured output 옵션 추가
            };
            string requestJson = JsonConvert.SerializeObject(payload);

            using var request = new UnityWebRequest(ApiUrl, "POST");
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(requestJson));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + _apiKey);

            var response = await request.SendWebRequest();

            if (response.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
                return null;
            }

            var spellResponse = JsonConvert.DeserializeObject<SpellResponse>(response.downloadHandler.text);
            var json = spellResponse.Choices.First().Message.Content;

            return json;
        }

        public record SpellResponse(SpellResponse.Choice[] Choices)
        {
            public Choice[] Choices { get; } = Choices;

            public record Choice(Message Message)
            {
                public Message Message { get; } = Message;
            }

            public record Message
            {
                public string Content { get; }

                public Message(string content)
                {
                    Content = content;
                    Debug.Log("Api 요청 결과: " + content);
                }
            }
        }
    }
}
