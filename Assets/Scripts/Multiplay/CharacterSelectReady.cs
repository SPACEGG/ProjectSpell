using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Multiplay
{
    public class CharacterSelectReady : NetworkBehaviour
    {
        public static CharacterSelectReady Singleton { get; private set; }

        public event EventHandler OnReadyChanged;

        private Dictionary<ulong, bool> _playerReadyDictionary;

        public void Awake()
        {
            if (Singleton && Singleton != this)
            {
                Destroy(gameObject);
                return;
            }
            Singleton = this;

            _playerReadyDictionary = new Dictionary<ulong, bool>();
        }

        public void TogglePlayerReady()
        {
            var clientId = NetworkManager.Singleton.LocalClientId;
            var isReady = IsPlayerReady(clientId);

            Debug.Assert(IsSpawned, "CharacterSelectReady not spawned when TogglePlayerReady is called!");

            TogglePlayerReadyRpc(!isReady);
        }

        [Rpc(SendTo.Server)]
        internal void TogglePlayerReadyRpc(bool isReady, RpcParams rpcParams = default)
        {
            var senderClientId = rpcParams.Receive.SenderClientId;
            _playerReadyDictionary[senderClientId] = isReady;

            PropagateTogglePlayerReadyRpc(senderClientId, isReady);
        }

        [Rpc(SendTo.Everyone)]
        internal void PropagateTogglePlayerReadyRpc(ulong clientId, bool isReady)
        {
            _playerReadyDictionary[clientId] = isReady;

            OnReadyChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool IsPlayerReady(ulong clientId)
        {
            return _playerReadyDictionary.ContainsKey(clientId) && _playerReadyDictionary[clientId];
        }

        public bool IsAllPlayerReady()
        {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (!_playerReadyDictionary.ContainsKey(clientId) || !_playerReadyDictionary[clientId])
                {
                    return false;
                }
            }

            return true;
        }
    }
}