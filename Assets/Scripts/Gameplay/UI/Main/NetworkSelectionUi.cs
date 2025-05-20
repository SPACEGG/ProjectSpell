using System;
using Multiplay;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Main
{
    public class NetworkSelectionUi : MonoBehaviour
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
            hostButton.onClick.AddListener(() => { Multiplayer.StartHost(); });
            clientButton.onClick.AddListener(() => { Multiplayer.StartClient(); });

            randomNameButton.onClick.AddListener(() => { playerNameInputField.text = randomNameData.GetRandomName(); });
        }

        private void Start()
        {
            playerNameInputField.text = randomNameData.GetRandomName();
        }
    }
}