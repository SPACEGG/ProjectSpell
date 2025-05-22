using System;
using Player;
using UnityEngine;

namespace Gameplay.UI.Multiplay
{
    public class GameEndUi : MonoBehaviour
    {
        public static GameEndUi Singleton { get; private set; }

        [SerializeField] private GameObject gameWinUi;
        [SerializeField] private GameObject youDieUi;

        private void Awake()
        {
            if (Singleton && Singleton != this)
            {
                Destroy(gameObject);
                return;
            }

            Singleton = this;
            DontDestroyOnLoad(this);
        }

        public void Initialize(NetworkHealthManaManager healthManaManager)
        {
            healthManaManager.OnPlayerDied += HealthManaManager_OnOnPlayerDied;
        }

        private void HealthManaManager_OnOnPlayerDied(object sender, EventArgs e)
        {
            ShowGameLoseUi();
        }

        private void Start()
        {
            Hide();
        }

        public void ShowGameWinUi()
        {
            gameWinUi.SetActive(true);
            youDieUi.SetActive(false);
            gameObject.SetActive(true);
        }

        public void ShowGameLoseUi()
        {
            gameWinUi.SetActive(false);
            youDieUi.SetActive(true);
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}