using System;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    [Obsolete("This class is deprecated. Use NetworkCameraManager instead.")]
    public class NetworkPowerLevelManager : NetworkBehaviour
    {
        [SerializeField] private KeyCode level1SelectKey = KeyCode.Alpha1;
        [SerializeField] private KeyCode level2SelectKey = KeyCode.Alpha2;
        [SerializeField] private KeyCode level3SelectKey = KeyCode.Alpha3;

        public int PowerLevel { get; private set; } = 1;

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
                // TODO: 레벨1 선택 ui
                PowerLevel = 1;
            }
            if (Input.GetKeyDown(level2SelectKey))
            {
                // TODO: 레벨2 선택 ui
                PowerLevel = 2;
            }
            if (Input.GetKeyDown(level3SelectKey))
            {
                // TODO: 레벨3 선택 ui
                PowerLevel = 3;
            }
        }
    }
}