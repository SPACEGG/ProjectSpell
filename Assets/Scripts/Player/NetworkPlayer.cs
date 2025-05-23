using System.Collections;
using Entity.Prefabs;
using Gameplay;
using Gameplay.UI.Multiplay;
using Gameplay.UI.PlayerHud;
using Multiplay;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(NetworkHealthManaManager))]
    public class NetworkPlayer : NetworkBehaviour
    {
        [SerializeField] private WizardBodyVisual wizardBodyVisual;
        [SerializeField] private Transform playerArmature;

        public static NetworkPlayer LocalInstance { get; set; }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                LocalInstance = this;
                var playerSpawnPosition = ProjectSpellGameManager.Singleton.GetPlayerSpawnPosition(OwnerClientId);
                StartCoroutine(MovePlayerToPosition(playerSpawnPosition));

                NetworkPlayerHudBootstrapper.Singleton.Initialize(NetworkObject);
                GameEndUi.Singleton.Initialize(GetComponent<NetworkHealthManaManager>());
            }

            var playerInfo = ProjectSpellGameMultiplayer.Singleton.GetPlayerInfoByClientId(OwnerClientId);
            wizardBodyVisual.SetPlayerColor(ProjectSpellGameMultiplayer.Singleton.GetPlayerColorMaterial(playerInfo.ColorId));
        }

        /// <summary>
        /// <br> Moves the player to the specified position over a duration of 0.3 seconds. </br>
        /// <br> Player doesn't move if it is teleported </br>
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        private IEnumerator MovePlayerToPosition(Vector3 targetPosition)
        {
            float duration = 0.3f;
            float elapsedTime = 0f;

            var startPosition = playerArmature.position;

            while (elapsedTime < duration)
            {
                playerArmature.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            playerArmature.position = targetPosition;
        }
    }
}