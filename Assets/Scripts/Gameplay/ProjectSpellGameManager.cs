using System;
using System.Collections.Generic;
using Entity.Prefabs;
using Multiplay;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        private NetworkVariable<GameState> _state = new();

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
            var multiplayer = ProjectSpellGameMultiplayer.Singleton;
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                var playerInfo = multiplayer.GetPlayerInfoByClientId(clientId);
                var playerTransform = Instantiate(playerPrefab);

                playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
                playerTransform.GetComponentInChildren<WizardBodyVisual>()
                    .SetPlayerColor(multiplayer.GetPlayerColorMaterial(playerInfo.ColorId));
                playerTransform.position = GetNextSpawnPoint().position;
            }
        }

        private Transform GetNextSpawnPoint()
        {
            var spawnPoint = spawnPoints[0];
            spawnPoints.RemoveAt(0);

            return spawnPoint;
        }
    }
}