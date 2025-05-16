using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Multiplay
{
    public class NetworkHud : MonoBehaviour
    {
        [SerializeField] private Button hostButton;
        [SerializeField] private Button clientButton;

        private NetworkManager _networkManager;

        private void Start()
        {
            _networkManager ??= NetworkManager.Singleton;
            _networkManager.OnConnectionEvent += HandleConnectionEvent;

            hostButton.onClick.AddListener(OnHostButtonClicked);
            clientButton.onClick.AddListener(OnClientButtonClicked);
        }

        private void OnHostButtonClicked()
        {
            if (_networkManager.IsHost || _networkManager.IsServer)
            {
                Debug.Log("Already hosting or server is running.");
                return;
            }

            _networkManager.StartHost();
            Debug.Log("Host started.");
        }

        private void OnClientButtonClicked()
        {
            if (_networkManager.IsClient || _networkManager.IsHost)
            {
                Debug.Log("Already connected as client or host.");
                return;
            }

            _networkManager.StartClient();
            Debug.Log("Client started.");
        }

        private void HandleConnectionEvent(NetworkManager arg1, ConnectionEventData arg2)
        {
            if (arg2.EventType == ConnectionEvent.ClientConnected && arg2.ClientId == _networkManager.LocalClientId)
            {
                Hide();
            }
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}