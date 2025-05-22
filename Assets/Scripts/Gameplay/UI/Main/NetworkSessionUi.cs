using System;
using Multiplay;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Main
{
    public class NetworkSessionUi : MonoBehaviour
    {
        private ProjectSpellGameMultiplayer Multiplayer => ProjectSpellGameMultiplayer.Singleton;

        [Header("Network Selection")]
        [SerializeField] private Button hostButton;
        [SerializeField] private Button clientButton;

        [Header("Player Name")]
        [SerializeField] private Button randomNameButton;
        [SerializeField] private RandomNameData randomNameData;
        [SerializeField] private TMP_InputField playerNameInputField;


        private void Awake()
        {
            hostButton.onClick.AddListener(() =>
            {
                Multiplayer.StartHost();
                Debug.Log("Host button clicked");
            });
            clientButton.onClick.AddListener(() =>
            {
                Multiplayer.StartClient();
                Debug.Log("Client button clicked");
            });

            randomNameButton.onClick.AddListener(() => { playerNameInputField.text = randomNameData.GetRandomName(); });
            playerNameInputField.onValueChanged.AddListener(name =>
            {
                if (string.IsNullOrEmpty(name))
                {
                    playerNameInputField.text = randomNameData.GetRandomName();
                }
                else
                {
                    Multiplayer.PlayerName = name;
                }
            });
        }

        private void Start()
        {
            playerNameInputField.text = randomNameData.GetRandomName();
        }
    }
}