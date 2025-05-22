using System;
using System.Collections.Generic;
using Multiplay;
using Player;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Main
{
    public class LobbyUi : MonoBehaviour
    {
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private Button quickJoinButton;
        [SerializeField] private Transform lobbyContainer;
        [SerializeField] private Transform lobbyTemplate;

        [Header("Player Name")]
        [SerializeField] private Button randomNameButton;
        [SerializeField] private RandomNameData randomNameData;
        [SerializeField] private TMP_InputField playerNameInputField;

        private void Awake()
        {
            createLobbyButton.onClick.AddListener(() =>
            {
                ProjectSpellGameLobby.Singleton.CreateLobby("Test Lobby").Forget();
                Debug.Log("Create Lobby button clicked");
            });

            quickJoinButton.onClick.AddListener(() =>
            {
                ProjectSpellGameLobby.Singleton.QuickJoinLobby().Forget();
                Debug.Log("Quick Join button clicked");
            });

            playerNameInputField.onValueChanged.AddListener(name =>
            {
                if (string.IsNullOrEmpty(name))
                {
                    playerNameInputField.text = randomNameData.GetRandomName();
                }
                else
                {
                    ProjectSpellGameMultiplayer.Singleton.PlayerName = name;
                }
            });
        }

        private void Start()
        {
            playerNameInputField.text = randomNameData.GetRandomName();

            ProjectSpellGameLobby.Singleton.OnLobbyListChanged += ProjectSpellGameLobby_OnOnLobbyListChanged;
            UpdateLobbyList(new List<Lobby>());

            lobbyTemplate.gameObject.SetActive(false);
        }

        private void ProjectSpellGameLobby_OnOnLobbyListChanged(object sender, ProjectSpellGameLobby.OnLobbyListChangedEventArgs e)
        {
            UpdateLobbyList(e.Lobbies);
        }

        private void UpdateLobbyList(List<Lobby> lobbies)
        {
            foreach (Transform child in lobbyContainer)
            {
                if (child == lobbyTemplate) continue;
                Destroy(child.gameObject);
            }

            foreach (var lobby in lobbies)
            {
                var lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
                lobbyTransform.gameObject.SetActive(true);
                lobbyTransform.GetComponent<LobbyListItem>().SetLobby(lobby);
            }
        }

        private void OnDestroy()
        {
            ProjectSpellGameLobby.Singleton.OnLobbyListChanged -= ProjectSpellGameLobby_OnOnLobbyListChanged;
        }
    }
}