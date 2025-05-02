using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;

public static class VoiceToWhisper
{
    public static IEnumerator SendToWhisper(AudioClip clip, Action<string> callback)
    {
        string openAIKey = ApiConfig.OpenAIKey;
        byte[] wavBytes = WavUtility.FromAudioClip(clip);

        var form = new WWWForm();
        form.AddBinaryData("file", wavBytes, "voice.wav", "audio/wav");
        form.AddField("model", "whisper-1");

        using var www = UnityWebRequest.Post(
            "https://api.openai.com/v1/audio/transcriptions", form);
        www.SetRequestHeader("Authorization", "Bearer " + openAIKey);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Whisper Error: " + www.error);
            yield break;
        }

        var resp = JsonConvert.DeserializeObject<WhisperResponse>(www.downloadHandler.text);
        callback(resp.text);

    }

    // Whisper 응답용 데이터 클래스
    class WhisperResponse { public string text; }
}
