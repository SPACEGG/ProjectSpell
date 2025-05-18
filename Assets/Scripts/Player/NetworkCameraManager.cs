using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class NetworkCameraManager : NetworkBehaviour
    {
        [SerializeField] private GameObject globalCamera;
        [SerializeField] private GameObject mainCamera;
        [SerializeField] private GameObject playerFollowCamera;

        private void Awake()
        {
            globalCamera = globalCamera != null ? globalCamera : GameObject.FindGameObjectWithTag("GlobalCamera");
            mainCamera = mainCamera != null ? mainCamera : transform.Find("MainCamera").gameObject;
            playerFollowCamera = transform.Find("PlayerFollowCamera").gameObject;
        }

        public override void OnNetworkSpawn()
        {
            if (globalCamera) globalCamera.SetActive(false);

            if (!IsLocalPlayer) DisableOtherCamera();
        }

        private void DisableOtherCamera()
        {
            mainCamera.SetActive(false);
            playerFollowCamera.SetActive(false);
            Destroy(this);
        }
    }
}