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
        private const string Model = "gpt-4.1-nano";

        private const string SystemPrompt = @"
You are a game assistant. Respond with ONLY a JSON object.

## Available Elements

- fire
- ice
- earth

## example:
{""element"":""fire"", direction"":[x,y,z],""power"":n}";

        private readonly string _apiKey;

        public TextToSpellApi()
        {
            if (!env.TryParseEnvironmentVariable("API_KEY", out _apiKey))
            {
                throw new ArgumentException("API key is not set in the environment variables.");
            }
        }

        public async UniTask<string> TextToSpellAsync(string text)
        {
            var payload = new
            {
                model = Model,
                messages = new object[]
                {
                    new { role = "system", content = SystemPrompt },
                    new { role = "user", content = text }
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
