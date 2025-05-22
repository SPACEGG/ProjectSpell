using Entity.Prefabs;
using Gameplay;
using Gameplay.UI.PlayerHud;
using Multiplay;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class PlayerInitializer : NetworkBehaviour
    {
        [SerializeField] private WizardBodyVisual wizardBodyVisual;

        public static PlayerInitializer LocalInstance { get; set; }

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
        }
    }
}