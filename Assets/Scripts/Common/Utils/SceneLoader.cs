using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Common.Utils
{
    public static class SceneLoader
    {
        public enum SceneType
        {
            MainScene,
            LoadingScene,
            LobbyScene,
            CharacterSelectScene,
        }

        private static SceneType _targetSceneType;

        public static void Load(SceneType targetSceneType)
        {
            _targetSceneType = targetSceneType;

            SceneManager.LoadScene(targetSceneType.ToString());
        }

        public static void LoadNetwork(SceneType targetSceneType)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(targetSceneType.ToString(), LoadSceneMode.Single);
        }

        public static void LoaderCallback()
        {
            SceneManager.LoadScene(_targetSceneType.ToString());
        }
    }
}