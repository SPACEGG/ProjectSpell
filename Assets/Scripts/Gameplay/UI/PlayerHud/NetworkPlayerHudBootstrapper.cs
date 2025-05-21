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
        private NetworkPowerLevelManager _playerPowerLevelManager;

        private NetworkManager NetworkManager => NetworkManager.Singleton;

        private void Start()
        {
            view.Hide();

            ProjectSpellGameManager.Singleton.OnPlayerSpawn += ProjectSpellGameManager_OnPlayerSpawn;
        }

        private void ProjectSpellGameManager_OnPlayerSpawn(object sender, ProjectSpellGameManager.OnPlayerSpawnEventArgs args)
        {
            if (args.ClientId != NetworkManager.LocalClientId) return;

            var playerObject = NetworkManager.SpawnManager.GetPlayerNetworkObject(args.ClientId);

            _playerHealthManaManager = playerObject.GetComponentInChildren<NetworkHealthManaManager>();
            _playerPowerLevelManager = playerObject.GetComponentInChildren<NetworkPowerLevelManager>();
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