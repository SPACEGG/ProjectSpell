using System;
using Multiplay;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    public class GameEndUi : MonoBehaviour
    {
        public static GameEndUi Singleton { get; private set; }

        [SerializeField] private GameObject gameWinUi;
        [SerializeField] private GameObject gameLoseUi;

        private void Awake()
        {
            Singleton = this;
            gameWinUi.SetActive(false);
            gameLoseUi.SetActive(false);
        }

        private void Start()
        {
            ProjectSpellGameManager.Singleton.OnGameRestart += HandleGameRestart;
        }

        private void OnDestroy()
        {
            if (ProjectSpellGameManager.Singleton != null)
            {
                ProjectSpellGameManager.Singleton.OnGameRestart -= HandleGameRestart;
            }
        }

        private void HandleGameRestart()
        {
            gameWinUi.SetActive(false);
            gameLoseUi.SetActive(false);
        }
    }
}