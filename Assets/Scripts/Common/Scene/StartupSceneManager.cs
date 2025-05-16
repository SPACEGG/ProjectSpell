using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common.Scene
{
    public class StartupSceneManager : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;

        private void Start()
        {
            DontDestroyOnLoad(networkManager.gameObject);
            SceneManager.LoadScene("MainScene");
        }
    }
}