using System;
using Multiplay;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Multiplay
{
    public class CharacterSelectUi : MonoBehaviour
    {
        [SerializeField] private Button readyButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button startButton;


        private void Awake()
        {
            readyButton.onClick.AddListener(() => { CharacterSelectReady.Singleton.TogglePlayerReady(); });
            cancelButton.onClick.AddListener(() => { CharacterSelectReady.Singleton.TogglePlayerReady(); });
            cancelButton.gameObject.SetActive(false);

            if (NetworkManager.Singleton.IsHost)
            {
                startButton.onClick.AddListener(() =>
                {
                    // TODO: 게임 시작 로직 추가
                    // SceneLoader.LoadNetwork(SceneLoader.SceneType.GameScene);
                });
                startButton.enabled = false;
            }
            else
            {
                startButton.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            CharacterSelectReady.Singleton.OnReadyChanged += CharacterSelectReady_OnOnReadyChanged;
        }

        private void CharacterSelectReady_OnOnReadyChanged(object sender, EventArgs e)
        {
            var isReady = CharacterSelectReady.Singleton.IsPlayerReady(NetworkManager.Singleton.LocalClientId);
            if (isReady)
            {
                readyButton.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(true);
            }
            else
            {
                readyButton.gameObject.SetActive(true);
                cancelButton.gameObject.SetActive(false);
            }

            if (NetworkManager.Singleton.IsHost)
            {
                startButton.interactable = CharacterSelectReady.Singleton.IsAllPlayerReady();
            }
        }

        private void OnDestroy()
        {
            CharacterSelectReady.Singleton.OnReadyChanged -= CharacterSelectReady_OnOnReadyChanged;
        }
    }
}