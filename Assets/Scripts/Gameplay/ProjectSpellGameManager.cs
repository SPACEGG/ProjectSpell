using System;
using System.Collections.Generic;
using System.Linq;
using Entity.Prefabs;
using Multiplay;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Gameplay.UI.Multiplay;

namespace Gameplay
{

    internal enum GameState
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    public class ProjectSpellGameManager : NetworkBehaviour
    {
        public static ProjectSpellGameManager Singleton { get; private set; }

        [SerializeField] private Transform playerPrefab;
        [SerializeField] private List<Transform> spawnPoints;

        public event EventHandler OnStateChanged;
        public event Action OnLocalPlayerDied;

        public event EventHandler OnPlayerWin;
        public event Action OnLocalPlayerWin;

        public class OnPlayerSpawnEventArgs : EventArgs
        {
            public ulong ClientId { get; set; }
        }


        private NetworkVariable<GameState> _state = new();

        private int totalPlayers = 0;
        private Dictionary<ulong, bool> SurvivedPlayers = new();

        private void Awake()
        {
            if (Singleton && Singleton != this)
            {
                Destroy(gameObject);
                return;
            }

            Singleton = this;


        }

        protected override void OnNetworkPostSpawn()
        {
            _state.OnValueChanged += State_OnValueChanged;

            if (IsServer)
            {
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
            }
        }

        private void State_OnValueChanged(GameState previousValue, GameState newValue)
        {
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted,
            List<ulong> clientsTimedOut)
        {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                var playerTransform = Instantiate(playerPrefab);

                var networkObject = playerTransform.GetComponent<NetworkObject>();
                networkObject.SpawnAsPlayerObject(clientId, true);

                SurvivedPlayers[clientId] = true;
            }

            totalPlayers = NetworkManager.Singleton.ConnectedClientsIds.Count;
        }

        public Vector3 GetPlayerSpawnPosition(ulong ownerClientId)
        {
            var playerIndex = NetworkManager.Singleton.ConnectedClientsIds.ToList().IndexOf(ownerClientId);

            return spawnPoints[playerIndex % spawnPoints.Count].position;
        }

        public void CheckPlayerWin(ulong clientId)
        {
            if (!IsServer) return;

            if (SurvivedPlayers.ContainsKey(clientId) && SurvivedPlayers[clientId])
            {
                SurvivedPlayers[clientId] = false;
                totalPlayers--;


                if (totalPlayers == 1)
                {
                    ulong winnerId = SurvivedPlayers.First(pair => pair.Value).Key;

                    Debug.Log("PlayerWin 호출");
                    OnPlayerWin?.Invoke(this, EventArgs.Empty);
                    WinnerClientRpc(winnerId);
                }
            }
        }
        [ClientRpc]
        private void WinnerClientRpc(ulong winnerId)
        {
            if (winnerId == NetworkManager.Singleton.LocalClientId)
            {
                OnLocalPlayerWin?.Invoke();
                GameEndUi.Singleton.ShowGameWinUi();
            }
        }
    }
}