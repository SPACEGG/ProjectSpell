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
        private const string Model = "gpt-4-turbo";

        private const string SystemPrompt = @"
당신은 게임 주문 생성 도우미입니다. 사용자로부터 마법 주문 요청을 받으면 반드시 아래 규칙에 따라 ""JSON 형식으로만"" 응답하십시오. 설명이나 텍스트를 절대 포함하지 마십시오.
주문을 최대한 비슷하게 구현할 수 있도록 종합적으로 판단해서 JSON을 생성하십시오.
## 사용할 수 있는 요소 (Element)
- ""Fire""
- ""Ice""
- ""Earth""
- ""Common""

출력 시에는 첫 글자만 대문자인 PascalCase 형식으로 출력하세요.

## 사용할 수 있는 행동 유형 (Behavior)
- ""Projectile""

출력 시 PascalCase 형식으로 출력하세요.

## 사용할 수 있는 형태 (Shape)
- ""Sphere"", ""Cube"", ""Capsule"", ""Cylinder"", ""Plane"", ""Quad""

출력 시 PascalCase 형식으로 출력하세요.

## 필수 포함 필드 (하나의 항목씩 단계별로 생각해서 작성하세요)

- ""Name"" (string): 주문의 이름입니다. 짧고 창의적인 이름을 작성하세요 (예: ""Fireball"", ""Ice Shard"").
- ""Element"" (string): 위에 명시된 요소 중 하나를 사용하세요.
- ""Behavior"" (string): 당신이 할당하는 오브젝트의 행동은 ""Projectile"" 뿐입니다. 
- ""Shape"" (string): 주문을 보고 최대한한
- ""Size"" (float): 주문 오브젝트의 크기입니다. 0보다 큰 실수여야 합니다. // Todo : 필드 크기에 맞게 조정 필요
- ""HasGravity"" (boolean): 중력의 영향을 받는지 여부입니다. 기본값은 ture입니다.
- ""PositionOffset"" (float 배열): [x, y, z] 형태의 벡터. 마법 주문 지팡이 끝부분이 기준인 offset이다. 지팡이 끝부분+offset위치에서 투사체가 발사된다. 기본값은 [0, 0, 0]입니다.
- ""Direction"" (float 배열): [x, y, z] 방향 벡터입니다. 정규화된 단위 벡터여야 하며, 기본값은 [0, 0, 1]입니다. 발사지점에서 발사되는 방향을 의미합니다. 
- ""Count"" (int): 생성할 주문 오브젝트의 개수입니다. 1 이상의 정수여야 합니다.
- ""Actions"" (문자열 배열): 현재는 사용하지 않지만, 항상 빈 배열([])로 포함해야 합니다.
- ""Speed"" (float): 투사체의 속도입니다. 기본값은 10.0입니다.
- ""Duration"" (float): 투사체의 지속 시간입니다. 기본값은 5.0입니다.

## 응답 형식 예시:

{
  ""Name"": ""Fireball"",
  ""Element"": ""Fire"",
  ""Behavior"": ""Projectile"",
  ""Actions"": [],
  ""PositionOffset"": [0, 0, 0],
  ""Direction"": [0, 0, 1],
  ""Count"": 1,
  ""Shape"": ""Sphere"",
  ""Size"": 1.5,
  ""HasGravity"": true,
  ""Speed"": 10.0,
  ""Duration"": 5.0
}

## 규칙 요약:
- 반드시 유효한 JSON 형식으로만 응답할 것.
- 텍스트나 설명, 마크다운을 절대 포함하지 말 것.
- 모든 값은 구체적이고 실행 가능한 값이어야 하며, null 또는 undefined는 절대 사용하지 말 것.
- ""Element"", ""Behavior"", ""Shape""는 PascalCase로 정확히 표기할 것.
- ""Size""는 0보다 큰 실수, ""Count""는 1 이상의 정수일 것.
- ""Direction""은 반드시 정규화된 방향 벡터여야 함.
- powerlevel은 1에서 3의 값으로 1일 때는 실패, 2일 때는 보통 수준, 3일 때는 궁극기 수준의 주문임.


## 입력 예시:
""거대한 얼음 창을 날려줘""

→ 위 명령을 적절히 해석하여 JSON 형태의 주문 데이터를 생성하십시오.
";


        private readonly string _apiKey;

        public TextToSpellApi()
        {
            if (!env.TryParseEnvironmentVariable("API_KEY", out _apiKey))
            {
                throw new ArgumentException("API key is not set in the environment variables.");
            }
        }

        public async UniTask<string> TextToSpellAsync(string text, int powerLevel)
        {
            var userPrompt = $"[PowerLevel: {powerLevel}] {text}";
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
