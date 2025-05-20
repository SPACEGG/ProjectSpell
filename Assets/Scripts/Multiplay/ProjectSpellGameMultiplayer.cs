using System;
using Player;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

namespace Multiplay
{
    public class ProjectSpellGameMultiplayer : NetworkBehaviour
    {
        public static ProjectSpellGameMultiplayer Singleton { get; private set; }

        public static bool _playMultiplayer = true;

        public event EventHandler OnPlayerDataNetworkListChanged;

        private NetworkManager NetworkManager => NetworkManager.Singleton;
        private readonly List<NetworkPlayerInfo> _playerInfos = new();

        private void Awake()
        {
            if (Singleton && Singleton != this)
            {
                Destroy(gameObject);
                return;
            }
            Singleton = this;
        }

        private void PlayerDataNetworkList_OnOnListChanged()
        {
            OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Host

        public void StartHost()
        {
            NetworkManager.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
            NetworkManager.StartHost();
        }

        private void NetworkManager_Server_OnClientConnectedCallback(ulong clientId)
        {
            _playerInfos.Add(new NetworkPlayerInfo()
            {
                ClientId = clientId,
                ColorId = 0, // TODO: 멀티플레이 필레이어 색상 선택하기
            });
            PlayerDataNetworkList_OnOnListChanged();
            SetPlayerNameRpc("Player");
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
            throw new NotImplementedException();
        }

        #endregion

        [Rpc(SendTo.ClientsAndHost)]
        private void SetPlayerNameRpc(string newName, RpcParams rpcParams = default)
        {
            int playerInfoIndex = GetIndexFromClientId(rpcParams.Receive.SenderClientId);

            var playerInfo = _playerInfos[playerInfoIndex];
            playerInfo.Name = newName;
            _playerInfos[playerInfoIndex] = playerInfo;
            PlayerDataNetworkList_OnOnListChanged();
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
    }
}