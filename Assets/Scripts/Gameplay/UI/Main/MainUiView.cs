using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Main
{
    internal class MainUiView : MonoBehaviour
    {
        [Header("Main Ui")]
        [SerializeField] private Button singlePlayButton;
        [SerializeField] private Button multiPlayButton;

        [Header("Network Selection Ui")]
        [SerializeField] private NetworkSelectionUi networkSelectionUi;

        private void Start()
        {
            singlePlayButton.onClick.AddListener(OnSinglePlayButtonClicked);
            multiPlayButton.onClick.AddListener(OnMultiPlayButtonClicked);
        }

        private void OnSinglePlayButtonClicked()
        {
            // TODO: Implement single player mode
        }

        private void OnMultiPlayButtonClicked()
        {
            networkSelectionUi.Show();
            Hide();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}