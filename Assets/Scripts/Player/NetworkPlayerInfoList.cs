using System;
using Unity.Netcode;
using System.Collections.Generic;

namespace Player
{
    public class NetworkPlayerInfoList : NetworkBehaviour
    {
        private readonly List<NetworkPlayerInfo> playerInfos = new();
        
        public event Action<List<NetworkPlayerInfo>> OnListChanged;

        public NetworkPlayerInfo this[int i]
        {
            get => playerInfos[i];
            set
            {
                playerInfos[i] = value;
                OnListChanged?.Invoke(playerInfos);
            }
        }

        public void Add(NetworkPlayerInfo networkPlayerInfo)
        {
            playerInfos.Add(networkPlayerInfo);
            OnListChanged?.Invoke(playerInfos);
        }

        public int GetIndexFromClientId(ulong clientId)
        {
            for (int i = 0; i < playerInfos.Count; i++)
            {
                if (playerInfos[i].ClientId == clientId)
                {
                    return i;
                }
            }
            return -1;
        }

        public int Count => playerInfos.Count;
    }
}