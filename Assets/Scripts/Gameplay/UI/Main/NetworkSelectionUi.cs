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
        [SerializeField] private Button backButton;

        [Header("Main Ui")]
        [SerializeField] private MainUiView mainUiView;

        private void Awake()
        {
            _networkManager = NetworkManager.Singleton;
            Hide();
        }

        private void Start()
        {
            hostButton.onClick.AddListener(OnHostButtonClicked);
            clientButton.onClick.AddListener(OnClientButtonClicked);
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        private void OnHostButtonClicked()
        {
            if (_networkManager.IsHost || _networkManager.IsServer)
            {
                _networkManager.Shutdown();
            }

            _networkManager.StartHost();
            Hide();
        }

        private void OnClientButtonClicked()
        {
            if (_networkManager.IsHost || _networkManager.IsServer)
            {
                _networkManager.Shutdown();
            }

            _networkManager.StartClient();
            Hide();
        }

        private void OnBackButtonClicked()
        {
            mainUiView.Show();
            Hide();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}