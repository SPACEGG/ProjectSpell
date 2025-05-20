using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Main
{
    internal class MainUiView : MonoBehaviour
    {
        [Header("Main Ui")]
        [SerializeField] private Button singlePlayButton;
        [SerializeField] private Button multiPlayButton;

        private void Awake()
        {
            singlePlayButton.onClick.AddListener(() =>
            {
                // TODO: Implement single player mode
            });
            multiPlayButton.onClick.AddListener(() =>
            {
                // TODO: Implement multi player mode
            });
        }
    }
}