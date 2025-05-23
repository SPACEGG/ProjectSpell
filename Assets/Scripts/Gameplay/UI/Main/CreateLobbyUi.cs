using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Gameplay.UI.Main
{
    public class CreateLobbyUi : MonoBehaviour
    {
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button confirmButton;
        [SerializeField] private TMP_InputField lobbyNameInputField;
        [SerializeField] private TMP_InputField playerNameInputField;

        private void Awake()
        {
            cancelButton.onClick.AddListener(() => { Hide(); });

            confirmButton.onClick.AddListener(() =>
            {
                if (string.IsNullOrEmpty(lobbyNameInputField.text))
                {
                    Debug.Log("Lobby name cannot be empty");
                    return;
                }

                ProjectSpellGameLobby.Singleton.CreateLobby(lobbyNameInputField.text).Forget();
                Hide();
            });

            Hide();
        }

        public void Show()
        {
            lobbyNameInputField.text = $"{playerNameInputField.text}'s Lobby";
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}