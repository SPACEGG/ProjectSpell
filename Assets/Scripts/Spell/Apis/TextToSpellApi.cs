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

## 카메라 타겟 위치 안내
- '카메라가 가리키는 월드 좌표'는 화면 중앙에서 Ray를 쏴서 처음 만나는 오브젝트(상대방 캐릭터, 환경, 스펠 오브젝트 등)의 월드 좌표입니다.
- 이 좌표는 주문자의 시점에서 실제로 조준하고 있는 지점입니다.
- 반드시 이 좌표를 주문의 방향(Direction) 또는 PositionOffset 계산에 적극적으로 참고하세요.

## 절대적으로 지켜야 하는 출력 규칙
반드시 유효한 JSON 객체 하나만 반환하십시오.
절대 설명, 텍스트, 마크다운, 주석을 포함하지 마십시오.
JSON은 PascalCase 키, 구체적이고 실행 가능한 값만 포함해야 하며, ""null"" 또는 ""undefined""는 사용할 수 없습니다.
Element, Behavior, Shape 값은 반드시 아래 명시된 목록 중에서 고르되, 첫 글자만 대문자인 PascalCase로 표기하십시오.

## 시스템 동작
시스템 동작을 이해하고 어떤 값을 반환해야 주문이 의도대로 실행될지 생각하세요.
시스템 동작은 다음과 같습니다. 
using System.Collections;
using UnityEngine;

namespace Spell.Model.Behaviors
{
    public class ProjectileBehavior : SpellBehaviorBase
    {
        public override void Behave(Vector3 spawnPosition)
        {
            int count = Data.Count > 0 ? Data.Count : 1;
            for (int i = 0; i < count; i++)
            {
                SpawnProjectile(spawnPosition, i, count);
            }
        }

        private void SpawnProjectile(Vector3 spawnPosition, int index, int totalCount)
        {
            // 방향
            Vector3 direction = Data.Direction.Value.normalized;

            // 여러 개일 때 퍼뜨리기(간단 예시, 필요시 수정)
            if (totalCount > 1)
            {
                float angleStep = 10f; // 각도 간격
                float angle = (index - (totalCount - 1) / 2f) * angleStep;
                direction = Quaternion.Euler(0, angle, 0) * direction;
            }

            // 위치
            transform.position = spawnPosition;

            // Rigidbody 추가 및 중력 적용
            var rb = GetComponent<Rigidbody>();
            if (rb == null)
                rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = Data.HasGravity;

            // 속도, 지속시간
            float speed = Data.Speed;
            float duration = Data.Duration;

            rb.linearVelocity = direction * speed;

            StartCoroutine(DestroyAfterSeconds(duration));
        }

        private IEnumerator DestroyAfterSeconds(float duration)
        {
            yield return new WaitForSeconds(duration);
            Destroy(gameObject);
        }
    }
}

## 필드 및 캐릭터 스케일
필드 크기는 100X100입니다. 
캐릭터 크기는 10정도 됩니다. 

// 이 코드는 ProjectileBehavior(투사체 스펠 동작) 클래스입니다.
// - SpellBehaviorBase를 상속받아 Behave를 구현합니다.
// - Behave가 호출되면, spawnPosition(생성 위치)와 SpellData의 파라미터(방향, 속도, 중력, 개수 등)를 사용해
//   투사체를 해당 방향으로 이동시키는 물리 기반 동작을 실행합니다.
// - 여러 개(count) 생성, 중력 적용, 지정된 속도/지속시간 등 다양한 파라미터를 반영합니다.
// - 일정 시간이 지나면 오브젝트를 파괴합니다.
//
// 즉, 이 코드는 ""투사체 형태의 스펠""이 날아가는 동작을 담당합니다.

## 사용할 수 있는 원소 (Element) 및 설명
- ""None"" : 아래 원소에 해당하지 않는 경우 선택합니다. 주문에 직접적인 원소 표현이 없더라도 웬만하면 아래 원소 중 하나를 선택하세요. 
- ""Fire"" : 지속 피해, 광역 공격, 폭발 
- ""Ice"" : 속박, 슬로우
- ""Earth"" : 방어, 넉백 저항, 무거운 한방
- ""Common"" : 회복, 버프 등 // Todo : 더 자세히 추가해야 함

## 사용할 수 있는 행동 유형 (Behavior)
- ""Projectile"" : 당신은 기본적으로 투척 주문 만을 생성합니다. 다만 투척이라는 것은 시스템 동작이 투척뿐이라는 것이므로, 예를들어 벽 생성 같은 경우 위에서 벽을 생성해서 속도를 0으로 설정하면 구현할 수 있습니다. 
## 사용할 수 있는 형태 (Shape)
- ""Sphere"", ""Cube"", ""Capsule"", ""Cylinder"", ""Plane"", ""Quad""
 : 주문을 해석해서 어떤 형태로 주문 오브젝트가 생성해야 하는지 고르세요. 

## JSON 필드 설명 (하나의 항목씩 설명에 따라 단계별로 생각해서 작성하세요)

- ""Name"" (string)
	: 주문의 이름입니다. 짧고 창의적인 이름을 작성하세요 (예: ""Fireball"", ""Ice Shard"").
- ""Element"" (string)
	: 주문을 해당하는 원소입니다. 추후에 원소 주문 오브젝트간 상호작용에 사용될 필드입니다. 
		주문에 직접적인 근거가 없다면 주문의 특성에 따라 선택하세요. 
- ""Behavior"" (string)
	: 당신이 할당하는 오브젝트의 행동은 항상 ""Projectile"" 뿐입니다. 
- ""Shape"" (string)
	: 주문이 구현되는 오브젝트의 크기입니다. 렌더링은 따로 할 것이므로, Collsion을 통해 캐릭터나 주문 오브젝트, 또는 필드와의 상호작용할 것만 생각하세요.
- ""Size"" (Vector3)
	: 주문 오브젝트의 크기입니다. 0보다 큰 실수여야 합니다. // Todo : 필드 크기에 맞게 조정 필요
- ""HasGravity"" (boolean)
	: 중력의 영향을 받는지 여부입니다. 웬만하면 true로 설정하며, 광선 같은 경우는 false로 설정합니다. 
- ""PositionOffset"" (Vector3)
	: [x, y, z] 형태의 벡터. 마법 주문 지팡이 끝부분이 기준인 offset이다. 지팡이 끝부분+offset위치에서 투사체가 발사된다. 따라서 기본값은 [0, 0, 0]이지만 하늘에서 떨어지는 경우 필드 크기를 고려하여 offset을 캐릭터 위로 설정할 수 있습니다. 
- ""Direction"" (Vector3)
	: [x, y, z] 방향 벡터입니다. 정규화된 단위 벡터여야 하며, 기본적으로는 투사체 주문이기 때문에 카메라 기준 앞인 [0, 0, 1]이지만, 하늘에서 떨어지는 경우 필드크기와 캐릭터 사이즈를 고려해 
- ""Count"" (int)
	: 생성할 주문 오브젝트의 개수입니다. 1 이상의 정수여야 합니다. 꼭 1일 필요는 없습니다. 궁극기의 경우 더 화려하고 강력할 수 있게 설정해주세요.
- ""Actions"" (문자열 배열)
	: 현재는 사용하지 않지만, 항상 빈 배열([])로 포함해야 합니다.
- ""Speed"" (float)
	: 투사체의 속도입니다. 필드 크기와 캐릭터 사이즈와 powerlevel을 고려해 주문에 적 속도를 설정해주세요. 
- ""Duration"" (float)
	: 투사체의 지속 시간입니다. 웬만하면 삭제는 액션으로 처리하기 때문에 크게 잡아주되, 사라져야 할 것들은 알아서 판단해서 설정해주세요.
	
## 특별 지침
당신이 생성하는 주문은 기본적으로 오브젝트를 발사해서 상대방을 맞추는 투척 주문입니다. 단 Common 원소의 경우 체력회복 주문이 들어온 경우 힐을 나한테 써야 하므로 지팡이 에서 방향이 뒤를 향해야 합니다. 


## 입력 예시:
""거대한 얼음 창을 날려줘""

→ 위 명령을 적절히 해석하여 JSON 형태의 주문 데이터를 생성하십시오.

## 응답 형식 예시:

{
  ""Name"": ""Frost Javelin"",
  ""Element"": ""Ice"",
  ""Behavior"": ""Projectile"",
  ""Actions"": [],
  ""PositionOffset"": [0, 0, 0],
  ""Direction"": [0, 0, 1],
  ""Count"": 3,
  ""Shape"": ""Cylinder"",
  ""Size"": [0.5, 0.5, 5],
  ""HasGravity"": false,
  ""Speed"": 14.0,
  ""Duration"": 6.0
}

";


        private readonly string _apiKey;

        public TextToSpellApi()
        {
            if (!env.TryParseEnvironmentVariable("API_KEY", out _apiKey))
            {
                throw new ArgumentException("API key is not set in the environment variables.");
            }
        }

        public async UniTask<string> TextToSpellAsync(string text, int powerLevel, Vector3 cameraTargetPosition)
        {
            // 카메라 타겟 위치 정보를 프롬프트에 추가
            var userPrompt =
                $"[PowerLevel: {powerLevel}] {text}\n" +
                $"카메라가 가리키는 월드 좌표: [{cameraTargetPosition.x:F2}, {cameraTargetPosition.y:F2}, {cameraTargetPosition.z:F2}]\n" +
                $"이 좌표를 주문의 방향(Direction) 또는 PositionOffset 계산에 참고하세요.";

            var payload = new
            {
                model = Model,
                messages = new object[]
                {
                    new { role = "system", content = SystemPrompt },
                    new { role = "user", content = userPrompt }
                }
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
