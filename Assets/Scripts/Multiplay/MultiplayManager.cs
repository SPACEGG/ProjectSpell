using System.Collections.Generic;
using System.Linq;
using Spell.Model.Actions;
using Spell.Model.Core;
using Spell.Model.Data;
using Unity.Netcode;
using UnityEngine;

namespace Multiplay
{
    // 멀티플레이 전투 관련 클래스 (MultiplayManager의 컴포넌트)
    public class MultiplayManager : NetworkBehaviour
    {
        public static MultiplayManager Singleton { get; private set; }

        public List<GameObject> GetAllPlayers()
        {
            return NetworkManager.Singleton.SpawnManager.PlayerObjects.Select(i => i.gameObject).ToList();
        }

        public void ApplyActions(List<SpellActionData> actionList, GameObject collided, GameObject origin)
        {
            if (actionList == null) return;

            foreach (var actionData in actionList)
            {
                var action = ActionFactory.CreateAction(actionData);
                if (action == null) continue;
                ActionContext context = new(actionData, collided, origin);
                action.Apply(context);
            }
        }

        private void Awake()
        {
            if (Singleton != null && Singleton != this)
            {
                Destroy(gameObject);
                return;
            }

            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}