using Common.Models;
using Player;
using Unity.Netcode;

namespace Gameplay.UI.PlayerHud
{
    public class NetworkPlayerHudPresenter
    {
        private readonly PlayerHudView _view;
        private readonly NetworkVariable<NetworkHealthModel> _healthModel;
        private readonly ManaModel _manaModel;

        public NetworkPlayerHudPresenter(PlayerHudView view, NetworkVariable<NetworkHealthModel> healthModel, ManaModel manaModel, NetworkPowerLevelManager powerLevelManager)
        {
            _view = view;
            _healthModel = healthModel;
            _manaModel = manaModel;

            healthModel.OnValueChanged += UpdateHp;
            manaModel.OnManaChanged += UpdateMp;
            powerLevelManager.OnPowerLevelChanged += UpdatePowerLevel;

            UpdateHp(healthModel.Value, healthModel.Value);
            UpdateMp(manaModel.CurrentMana);
        }

        private void UpdateHp(NetworkHealthModel _, NetworkHealthModel newValue)
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
            _healthModel.OnValueChanged -= UpdateHp;
            _manaModel.OnManaChanged -= UpdateMp;
        }
    }
}