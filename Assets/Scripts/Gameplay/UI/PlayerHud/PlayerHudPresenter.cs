using System;
using Common.Models;
using UnityEngine;

namespace Gameplay.UI.PlayerHud
{
    public class PlayerHudPresenter
    {
        private readonly PlayerHudView _view;
        private readonly HealthModel _healthModel;

        public PlayerHudPresenter(PlayerHudView view, HealthModel healthModel)
        {
            _view = view;
            _healthModel = healthModel;

            healthModel.OnHealthChanged += UpdateHp;

            UpdateHp(healthModel.CurrentHealth);
        }

        private void UpdateHp(float currentHealth)
        {
            Debug.Log("UpdateHp: " + currentHealth);
            _view.SetHp(currentHealth, _healthModel.MaxHealth);
        }

        public void Dispose()
        {
            _healthModel.OnHealthChanged -= UpdateHp;
        }
    }
}