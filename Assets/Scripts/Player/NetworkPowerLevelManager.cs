using System;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class NetworkPowerLevelManager : NetworkBehaviour
    {
        [SerializeField] private KeyCode level1SelectKey = KeyCode.Alpha1;
        [SerializeField] private KeyCode level2SelectKey = KeyCode.Alpha2;
        [SerializeField] private KeyCode level3SelectKey = KeyCode.Alpha3;

        public event Action<OnPowerLevelChangedEventArgs> OnPowerLevelChanged;

        private int _powerLevel = 1;

        public int PowerLevel
        {
            get => _powerLevel;
            private set
            {
                if (_powerLevel != value)
                {
                    _powerLevel = value;
                    OnPowerLevelChanged?.Invoke(
                        new OnPowerLevelChangedEventArgs
                        {
                            NewPowerLevel = _powerLevel
                        });
                }
            }
        }

        public override void OnNetworkSpawn()
        {
            if (!IsLocalPlayer) Destroy(this);
        }

        private void Update()
        {
            PowerLevelSelectKeyInput();
        }

        private void PowerLevelSelectKeyInput() // 이름 변경
        {
            if (Input.GetKeyDown(level1SelectKey))
            {
                PowerLevel = 1;
            }
            if (Input.GetKeyDown(level2SelectKey))
            {
                PowerLevel = 2;
            }
            if (Input.GetKeyDown(level3SelectKey))
            {
                PowerLevel = 3;
            }
        }
    }
}