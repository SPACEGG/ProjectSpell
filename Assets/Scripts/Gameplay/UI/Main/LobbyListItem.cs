using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Main
{
    public class LobbyListItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lobbyNameText;

        private Lobby _lobby;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                ProjectSpellGameLobby.Singleton.JoinLobby(_lobby.Id).Forget();
                Debug.Log($"Join Lobby button clicked for lobby: {_lobby.Name}");
            });
        }

        public void SetLobby(Lobby lobby)
        {
            _lobby = lobby;
            lobbyNameText.text = lobby.Name;
        }
    }
}