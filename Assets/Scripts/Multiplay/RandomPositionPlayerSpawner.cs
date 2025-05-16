using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Multiplay
{
    [RequireComponent(typeof(NetworkManager))]
    public class RandomPositionPlayerSpawner : MonoBehaviour
    {
        private NetworkManager _networkManager;

        private int _roundRobinIndex;

        [SerializeField] private SpawnMethod spawnMethod;

        [SerializeField] private List<Vector3> spawnPositions = new() { Vector3.zero };

        private void Awake()
        {
            var networkManager = gameObject.GetComponent<NetworkManager>();
            networkManager.ConnectionApprovalCallback += ConnectionApprovalWithRandomSpawnPos;
        }

        private void ConnectionApprovalWithRandomSpawnPos(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            response.CreatePlayerObject = true;
            response.Position = GetNextSpawnPosition();
            response.Rotation = Quaternion.identity;
            response.Approved = true;
        }

        private Vector3 GetNextSpawnPosition()
        {
            switch (spawnMethod)
            {
                case SpawnMethod.Random:
                    var index = Random.Range(0, spawnPositions.Count);
                    return spawnPositions[index];
                case SpawnMethod.RoundRobin:
                    _roundRobinIndex = (_roundRobinIndex + 1) % spawnPositions.Count;
                    return spawnPositions[_roundRobinIndex];
                default:
                    throw new NotImplementedException();
            }
        }
    }

    internal enum SpawnMethod
    {
        Random = 0,
        RoundRobin = 1,
    }
}