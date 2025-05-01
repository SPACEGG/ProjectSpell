using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;

public static class TextToSpell
{
    public static IEnumerator SendToGPT(string text, Action<Spell> callback)
    {
        string openAIKey = ApiConfig.OpenAIKey;
        string chatURL = "https://api.openai.com/v1/chat/completions";
        string systemPrompt = @"
You are a game assistant. Available spells:
- Fireball → prefab 'FireballPrefab'
- IceShard → prefab 'IceShardPrefab'
- RockThrow → prefab 'RockPrefab'
- 만약 비슷한 프리펩이 없으면 그냥 NULL 값 출력

Respond with ONLY a JSON object:
{""spell"":""..."",""prefab"":""..."",""direction"":[x,y,z],""power"":n}";

        var payload = new
        {
            model = "gpt-3.5-turbo",
            messages = new object[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user",   content = text }
            }
        };
        string json = JsonConvert.SerializeObject(payload);

        using var www = new UnityWebRequest(chatURL, "POST")
        {
            uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + openAIKey);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("ChatGPT Error: " + www.error);
            yield break;
        }

        // 4) JSON 디코드
        var chatResp = JsonConvert.DeserializeObject<ChatResponse>(www.downloadHandler.text);
        string content = chatResp.choices[0].message.content;
        // Debug.Log("GPT 반환 JSON: " + content);

        // 5) JSON → SpellData
        Spell spell = JsonConvert.DeserializeObject<Spell>(content);

        // 6) 스펠 실행
        callback(spell);
    }

    class ChatResponse { public Choice[] choices; }
    class Choice { public Message message; }
    class Message { public string content; }
}
