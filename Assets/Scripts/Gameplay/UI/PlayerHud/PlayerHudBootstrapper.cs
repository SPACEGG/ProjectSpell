using Common.Data;
using Common.Models;
using UnityEngine;

namespace Gameplay.UI.PlayerHud
{
    public class PlayerHudBootstrapper : MonoBehaviour
    {
        [SerializeField] private PlayerHudView view;
        [SerializeField] private HealthData healthData;

        private PlayerHudPresenter _presenter;
        private HealthModel _hpModel;

        private void Start()
        {
            _hpModel = new HealthModel(healthData.MaxHealth);

            _presenter = new PlayerHudPresenter(view, _hpModel);
        }

        private void OnDestroy()
        {
            _presenter.Dispose();
        }
    }
}