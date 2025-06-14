using System;
using Entity.Prefabs;
using Multiplay;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public class CharacterSelectPlayer : MonoBehaviour
    {
        [SerializeField] private int playerIndex;
        [SerializeField] private GameObject readyGameObject;
        [SerializeField] private TextMeshPro playerNameText;
        [SerializeField] private WizardBodyVisual playerVisual;
        [SerializeField] private Animator animator;

        private void Awake()
        {
            readyGameObject.SetActive(false);
            animator.Play($"Idle{Random.Range(1, 4)}");
        }

        private void Start()
        {
            ProjectSpellGameMultiplayer.Singleton.OnPlayerInfosChanged += ProjectSpellGameMultiplayer_OnPlayerInfosChanged;
            CharacterSelectReady.Singleton.OnReadyChanged += CharacterSelectReady_OnReadyChanged;

            UpdatePlayer();
        }

        private void ProjectSpellGameMultiplayer_OnPlayerInfosChanged(object sender, EventArgs e)
        {
            UpdatePlayer();
        }

        private void CharacterSelectReady_OnReadyChanged(object sender, EventArgs e)
        {
            UpdatePlayer();
        }

        private void UpdatePlayer()
        {
            if (ProjectSpellGameMultiplayer.Singleton.IsPlayerIndexConnected(playerIndex))
            {
                Show();

                var playerInfo = ProjectSpellGameMultiplayer.Singleton.GetPlayerInfoByClientId((ulong)playerIndex);

                readyGameObject.SetActive(CharacterSelectReady.Singleton.IsPlayerReady(playerInfo.ClientId));

                playerNameText.text = playerInfo.Name.ToString();

                playerVisual.SetPlayerColor(ProjectSpellGameMultiplayer.Singleton.GetPlayerColorMaterial(playerInfo.ColorId));
            }
            else
            {
                Hide();
            }
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            ProjectSpellGameMultiplayer.Singleton.OnPlayerInfosChanged -= ProjectSpellGameMultiplayer_OnPlayerInfosChanged;
            CharacterSelectReady.Singleton.OnReadyChanged -= CharacterSelectReady_OnReadyChanged;
        }
    }
}