using UnityEngine;
using System;

namespace Player
{
    [Obsolete("This class is deprecated. Use NetworkPowerLevelManager instead.")]
    public class PowerLevelManager : MonoBehaviour
    {
        private int _currentPowerLevel = 1;

        public event Action<OnPowerLevelChangedEventArgs> OnPowerLevelChanged;

        public int CurrentPowerLevel
        {
            get => _currentPowerLevel;
            private set
            {
                if (_currentPowerLevel != value)
                {
                    _currentPowerLevel = value;
                    OnPowerLevelChanged?.Invoke(
                        new OnPowerLevelChangedEventArgs
                        {
                            NewPowerLevel = _currentPowerLevel
                        });
                }
            }
        }

        public void SetPowerLevel(int level)
        {
            CurrentPowerLevel = level;
        }
    }

    public struct OnPowerLevelChangedEventArgs
    {
        public int NewPowerLevel;
    }
}