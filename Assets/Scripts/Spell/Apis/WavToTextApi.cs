using System;
using CandyCoded.env;
using Common.Models;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Spell.Apis
{
    public class WavToTextApi
    {
        private const string ApiUrl = "https://api.openai.com/v1/audio/transcriptions";
        private const string Model = "whisper-1";

        private readonly string _apiKey;

        public WavToTextApi()
        {
            if (!env.TryParseEnvironmentVariable("API_KEY", out _apiKey))
            {
                Debug.LogError("API key is not set in the environment variables.");
                _apiKey = string.Empty;
            }
        }

        public async UniTask<string> WavToTextAsync(Wav wav)
        {
            var form = new WWWForm();
            form.AddBinaryData("file", wav.Value, "voice.wav", "audio/wav");
            form.AddField("model", Model);

            using var www = UnityWebRequest.Post(ApiUrl, form);
            www.SetRequestHeader("Authorization", "Bearer " + _apiKey);

            var response = await www.SendWebRequest();

            if (response.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + www.error);
                return null;
            }

            var whisperResponse = JsonConvert.DeserializeObject<WhisperResponse>(response.downloadHandler.text);
            var responseText = whisperResponse.Text;

            return responseText;
        }

        public record WhisperResponse
        {
            public string Text { get; }

            public WhisperResponse(string text)
            {
                Text = text;
                Debug.Log("인식된 텍스트: " + text);
            }
        }
    }
}
