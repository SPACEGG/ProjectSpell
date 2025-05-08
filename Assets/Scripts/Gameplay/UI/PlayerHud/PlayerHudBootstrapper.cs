using Common.Models;
using UnityEngine;

namespace Gameplay.UI.PlayerHud
{
    public class PlayerHudBootstrapper : MonoBehaviour
    {
        [SerializeField] private PlayerHudView view;

        [SerializeField] private PlayerManager playerManager;

        private PlayerHudPresenter _presenter;
        private HealthModel _hpModel;
        private ManaModel _mpModel;

        private void Start()
        {
            _hpModel = playerManager.HealthModel;
            _mpModel = playerManager.ManaModel;

            _presenter = new PlayerHudPresenter(view, _hpModel, _mpModel);
        }

        private void OnDestroy()
        {
            _presenter.Dispose();
        }
    }
}