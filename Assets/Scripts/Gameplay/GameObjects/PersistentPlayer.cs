using Gameplay.GameObjects.RuntimeDataContainers;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay.GameObjects
{
    [RequireComponent(typeof(NetworkObject))]
    public class PersistentPlayer : NetworkBehaviour
    {
        [SerializeField] private PersistentPlayerRuntimeCollection persistentPlayerRuntimeCollection;

        public override void OnNetworkSpawn()
        {
            gameObject.name = "PersistentPlayer" + OwnerClientId;

            persistentPlayerRuntimeCollection.Add(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            RemovePersistentPlayer();
        }

        private void RemovePersistentPlayer()
        {
            persistentPlayerRuntimeCollection.Remove(this);
        }
    }
}