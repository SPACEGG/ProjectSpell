using System;
using Multiplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Main
{
    public class NetworkSelectionUi : MonoBehaviour
    {
        private ProjectSpellGameMultiplayer Multiplayer => ProjectSpellGameMultiplayer.Singleton;

        [Header("Network Selection Ui")]
        [SerializeField] private Button hostButton;
        [SerializeField] private Button clientButton;
        [SerializeField] private TMP_InputField playerNameInputField;


        private void Awake()
        {
            hostButton.onClick.AddListener(() => { Multiplayer.StartHost(); });
            clientButton.onClick.AddListener(() => { Multiplayer.StartClient(); });
        }
    }
}