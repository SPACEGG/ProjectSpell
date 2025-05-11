using Common.Models;
using Player;
using UnityEngine;

namespace Gameplay.UI.PlayerHud
{
    public class PlayerHudBootstrapper : MonoBehaviour
    {
        [SerializeField] private PlayerHudView view;

        [SerializeField] private HealthManaManager playerHealthManaManager;

        private PlayerHudPresenter _presenter;
        private HealthModel _hpModel;
        private ManaModel _mpModel;

        private void Start()
        {
            _hpModel = playerHealthManaManager.HealthModel;
            _mpModel = playerHealthManaManager.ManaModel;

            _presenter = new PlayerHudPresenter(view, _hpModel, _mpModel);

            Debug.Log(_hpModel.CurrentHealth);
        }

        private void OnDestroy()
        {
            _presenter.Dispose();
        }
    }
}