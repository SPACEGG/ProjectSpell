using Common.Models;
using Player;

namespace Gameplay.UI.PlayerHud
{
    public class NetworkPlayerHudPresenter
    {
        private readonly PlayerHudView _view;
        private readonly NetworkHealthManaManager _healthManaManager;
        private readonly ManaModel _manaModel;

        public NetworkPlayerHudPresenter(
            PlayerHudView view,
            NetworkHealthManaManager healthManaManager,
            NetworkPowerLevelManager powerLevelManager
        )
        {
            _view = view;
            _healthManaManager = healthManaManager;
            _manaModel = healthManaManager.ManaModel;

            _healthManaManager.OnHealthChanged += HealthManaManager_OnOnHealthChanged;
            _healthManaManager.ManaModel.OnManaChanged += UpdateMp;
            powerLevelManager.OnPowerLevelChanged += UpdatePowerLevel;

            UpdateHp(_healthManaManager.HealthModel.Value);
            UpdateMp(_healthManaManager.ManaModel.CurrentMana);
        }

        private void HealthManaManager_OnOnHealthChanged(object sender, NetworkHealthManaManager.OnHealthChangedEventArgs e)
        {
            _view.SetHp(e.CurrentHealth, e.MaxHealth);
        }

        private void UpdateHp(NewNetworkHealthModel newValue)
        {
            _view.SetHp(newValue.CurrentHealth, newValue.MaxHealth);
        }

        private void UpdateMp(float currentMana)
        {
            _view.SetMp(currentMana, _manaModel.ManaPerLevel);
        }

        private void UpdatePowerLevel(OnPowerLevelChangedEventArgs args)
        {
            _view.SetPowerLevel(args.NewPowerLevel);
        }

        public void Dispose()
        {
            _healthManaManager.OnHealthChanged += HealthManaManager_OnOnHealthChanged;
            _manaModel.OnManaChanged -= UpdateMp;
        }
    }
}