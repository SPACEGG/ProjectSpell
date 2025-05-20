using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Main
{
    public class NetworkSelectionUi : MonoBehaviour
    {
        private NetworkManager _networkManager;

        [Header("Network Selection Ui")]
        [SerializeField] private Button hostButton;
        [SerializeField] private Button clientButton;

        private void Awake()
        {
            _networkManager = NetworkManager.Singleton;

            hostButton.onClick.AddListener(OnHostButtonClicked);
            clientButton.onClick.AddListener(OnClientButtonClicked);
        }

        private void OnHostButtonClicked()
        {
            if (_networkManager.IsHost || _networkManager.IsServer)
            {
                _networkManager.Shutdown();
            }

            _networkManager.StartHost();
        }

        private void OnClientButtonClicked()
        {
            if (_networkManager.IsHost || _networkManager.IsServer)
            {
                _networkManager.Shutdown();
            }

            _networkManager.StartClient();
        }
    }
}