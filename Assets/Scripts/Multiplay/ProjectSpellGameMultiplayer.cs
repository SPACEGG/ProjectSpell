using System;
using Player;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using Common.Utils;

namespace Multiplay
{
    public class ProjectSpellGameMultiplayer : NetworkBehaviour
    {
        public static ProjectSpellGameMultiplayer Singleton { get; private set; }
        public static bool _playMultiplayer = true;

        [SerializeField] private List<Color> playerColors;
        [SerializeField] private List<Material> playerColorMaterials;

        public event EventHandler OnPlayerInfosChanged;

        private NetworkManager NetworkManager => NetworkManager.Singleton;
        private NetworkList<NetworkPlayerInfo> _playerInfos;

        public string PlayerName { get; set; } = "Player";

        private void Awake()
        {
            if (Singleton && Singleton != this)
            {
                Destroy(gameObject);
                return;
            }

            Singleton = this;

            DontDestroyOnLoad(this);

            _playerInfos = new NetworkList<NetworkPlayerInfo>();
            _playerInfos.OnListChanged += PlayerDataNetworkList_OnOnListChanged;
        }

        private void PlayerDataNetworkList_OnOnListChanged(NetworkListEvent<NetworkPlayerInfo> changeEvent)
        {
            OnPlayerInfosChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Host

        public void StartHost()
        {
            NetworkManager.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
            NetworkManager.StartHost();
            SceneLoader.LoadNetwork(SceneLoader.SceneType.CharacterSelectScene);
        }

        private void NetworkManager_Server_OnClientConnectedCallback(ulong clientId)
        {
            _playerInfos.Add(new NetworkPlayerInfo()
            {
                ClientId = clientId,
                ColorId = GetFirstUnusedColorId(),
            });
            SetPlayerNameRpc(PlayerName);
        }

        #endregion

        #region Client

        public void StartClient()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
            NetworkManager.Singleton.StartClient();
        }

        private void NetworkManager_Client_OnClientConnectedCallback(ulong obj)
        {
            SetPlayerNameRpc(PlayerName);
        }

        #endregion

        [Rpc(SendTo.Server)]
        private void SetPlayerNameRpc(string newName, RpcParams rpcParams = default)
        {
            int playerInfoIndex = GetIndexFromClientId(rpcParams.Receive.SenderClientId);

            var playerInfo = _playerInfos[playerInfoIndex];
            playerInfo.Name = newName;
            _playerInfos[playerInfoIndex] = playerInfo;
        }

        public void ChangePlayerColor(int colorId)
        {
            ChangePlayerColorRpc(colorId);
        }

        [Rpc(SendTo.Server)]
        private void ChangePlayerColorRpc(int colorId, RpcParams rpcParams = default)
        {
            if (!IsColorAvailable(colorId))
            {
                // Color not available
                return;
            }

            int playerInfoIndex = GetIndexFromClientId(rpcParams.Receive.SenderClientId);

            var playerInfo = _playerInfos[playerInfoIndex];

            playerInfo.ColorId = colorId;

            _playerInfos[playerInfoIndex] = playerInfo;
        }

        private bool IsColorAvailable(int colorId)
        {
            foreach (var playerInfo in _playerInfos)
            {
                if (playerInfo.ColorId == colorId)
                {
                    // Already in use
                    return false;
                }
            }

            return true;
        }

        public NetworkPlayerInfo GetPlayerInfo()
        {
            return GetPlayerInfoByClientId(NetworkManager.Singleton.LocalClientId);
        }

        public NetworkPlayerInfo GetPlayerInfoByClientId(ulong clientId)
        {
            foreach (var playerInfo in _playerInfos)
            {
                if (playerInfo.ClientId == clientId)
                {
                    return playerInfo;
                }
            }

            return default;
        }


        public Color GetPlayerColor(int colorId)
        {
            return playerColors[colorId];
        }

        public Material GetPlayerColorMaterial(int colorId)
        {
            return playerColorMaterials[colorId];
        }

        private int GetIndexFromClientId(ulong clientId)
        {
            for (int i = 0; i < _playerInfos.Count; i++)
            {
                if (_playerInfos[i].ClientId == clientId)
                {
                    return i;
                }
            }

            return -1;
        }

        private int GetFirstUnusedColorId()
        {
            for (int i = 0; i < playerColors.Count; i++)
            {
                if (IsColorAvailable(i))
                {
                    return i;
                }
            }

            return -1;
        }

        public bool IsPlayerIndexConnected(int playerIndex)
        {
            return playerIndex < _playerInfos.Count;
        }
    }
}