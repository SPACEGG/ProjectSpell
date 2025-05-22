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

        public static NetworkPlayer LocalInstance { get; set; }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                LocalInstance = this;
                transform.position = ProjectSpellGameManager.Singleton.GetPlayerSpawnPosition(OwnerClientId);
            }

            var playerInfo = ProjectSpellGameMultiplayer.Singleton.GetPlayerInfoByClientId(OwnerClientId);
            wizardBodyVisual.SetPlayerColor(ProjectSpellGameMultiplayer.Singleton.GetPlayerColorMaterial(playerInfo.ColorId));

            NetworkPlayerHudBootstrapper.Singleton.Initialize(NetworkObject);
            GameEndUi.Singleton.Initialize(GetComponent<NetworkHealthManaManager>());
        }
    }
}