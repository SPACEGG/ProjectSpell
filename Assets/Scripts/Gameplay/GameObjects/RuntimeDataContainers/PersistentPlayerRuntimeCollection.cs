using Common.Utils;
using UnityEngine;

namespace Gameplay.GameObjects.RuntimeDataContainers
{
    [CreateAssetMenu(fileName = "PersistentPlayerRuntimeCollection", menuName = "Scriptable Objects/Multiplay/PersistentPlayerRuntimeCollection")]
    public class PersistentPlayerRuntimeCollection : RuntimeCollection<PersistentPlayer>
    {
        public bool TryGetPlayer(ulong clientID, out PersistentPlayer persistentPlayer)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (clientID == items[i].OwnerClientId)
                {
                    persistentPlayer = items[i];
                    return true;
                }
            }

            persistentPlayer = null;
            return false;
        }
    }
}