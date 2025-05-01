using System.IO;
using UnityEngine;

#if UNITY_EDITOR

public static class ApiConfig
{
    private const string Path = "Assets/Resources/config.json";

    public static string OpenAIKey { get; } = GetConfigData().openAIKey;

    private static ConfigData GetConfigData()
    {
        if (File.Exists(Path))
        {
            string json = File.ReadAllText(Path);

            return JsonUtility.FromJson<ConfigData>(json);
        }

        Debug.LogError("config.json 파일을 찾을 수 없습니다.");
        return null;
    }
}

#endif

[System.Serializable]
public class ConfigData
{
    public string openAIKey;
}