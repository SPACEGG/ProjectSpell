using System;
using Common.Models;
using UnityEngine;

namespace Gameplay.UI.PlayerHud
{
    public class PlayerHudPresenter
    {
        private readonly PlayerHudView _view;
        private readonly HealthModel _healthModel;
        private readonly ManaModel _manaModel;

        public PlayerHudPresenter(PlayerHudView view, HealthModel healthModel, ManaModel manaModel)
        {
            _view = view;
            _healthModel = healthModel;
            _manaModel = manaModel;

            healthModel.OnHealthChanged += UpdateHp;
            manaModel.OnManaChanged += UpdateMp;

            UpdateHp(healthModel.CurrentHealth);
            UpdateMp(manaModel.CurrentMana);
        }

        private void UpdateHp(float currentHealth)
        {
            Debug.Log("UpdateHp: " + currentHealth);
            _view.SetHp(currentHealth, _healthModel.MaxHealth);
        }

        private void UpdateMp(float currentMana)
        {
            _view.SetMp(currentMana, _manaModel.ManaPerLevel);
        }

        public void Dispose()
        {
            _healthModel.OnHealthChanged -= UpdateHp;
            _manaModel.OnManaChanged -= UpdateMp;
        }
    }
}