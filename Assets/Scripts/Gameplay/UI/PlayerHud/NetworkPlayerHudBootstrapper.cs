using System;
using Player;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay.UI.PlayerHud
{
    public class NetworkPlayerHudBootstrapper : MonoBehaviour
    {
        public static NetworkPlayerHudBootstrapper Singleton { get; private set; }

        [SerializeField] private PlayerHudView view;

        private NetworkPlayerHudPresenter _presenter;

        private NetworkHealthManaManager _playerHealthManaManager;
        private NetworkPowerLevelManager _playerPowerLevelManager;

        private NetworkManager NetworkManager => NetworkManager.Singleton;

        private void Awake()
        {
            if (Singleton && Singleton != this)
            {
                Destroy(gameObject);
                return;
            }

            Singleton = this;
        }

        private void Start()
        {
            view.Hide();
        }

        public void Initialize(NetworkObject playerObject)
        {
            _playerHealthManaManager = playerObject.GetComponentInChildren<NetworkHealthManaManager>();
            _playerPowerLevelManager = playerObject.GetComponentInChildren<NetworkPowerLevelManager>();
            _presenter = new NetworkPlayerHudPresenter(view, _playerHealthManaManager.HealthModel, _playerHealthManaManager.ManaModel,
                _playerPowerLevelManager);
            view.Show();
        }
    }
}