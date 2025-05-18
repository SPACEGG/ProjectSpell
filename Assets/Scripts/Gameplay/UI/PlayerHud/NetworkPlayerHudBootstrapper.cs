using System;
using Player;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay.UI.PlayerHud
{
    public class NetworkPlayerHudBootstrapper : MonoBehaviour
    {
        [SerializeField] private PlayerHudView view;

        private NetworkPlayerHudPresenter _presenter;

        private NetworkHealthManaManager _playerHealthManaManager;
        private PowerLevelManager _playerPowerLevelManager;

        private NetworkManager _networkManager;

        private void Start()
        {
            view.Hide();

            _networkManager ??= NetworkManager.Singleton;
            _networkManager.OnClientConnectedCallback += NetworkManager_OnClientConnected;
        }

        private void NetworkManager_OnClientConnected(ulong obj)
        {
            if (obj != _networkManager.LocalClientId) return;

            var playerObject = _networkManager.SpawnManager.GetPlayerNetworkObject(obj);

            _playerHealthManaManager = playerObject.GetComponentInChildren<NetworkHealthManaManager>();
            _playerPowerLevelManager = playerObject.GetComponentInChildren<PowerLevelManager>();
            _presenter = new NetworkPlayerHudPresenter(view, _playerHealthManaManager.HealthModel, _playerHealthManaManager.ManaModel,
                _playerPowerLevelManager);
            view.Show();
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();
        }
    }
}