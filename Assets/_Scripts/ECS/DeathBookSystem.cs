using Game.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.ECS
{
    public partial class DeathBookSystem : SystemBase
    {
        List<int> _toMarkAsDied = new List<int>();
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            PlayerController.PlayerDied += PlayerDied;
        }
        protected override void OnStopRunning()
        {
            base.OnStopRunning();
            PlayerController.PlayerDied -= PlayerDied;
        }
        private void PlayerDied(PlayerController controller)
        {
            if (PlayersSpawnerSystem.NetworkHandler.Runner.IsServer)//only server handle things
                _toMarkAsDied.Add(controller.Object.InputAuthority.PlayerId);
        }
        protected override void OnUpdate()
        {
            foreach (var deadman in _toMarkAsDied)
            {
                if (PlayersSpawnerSystem.NetworkPlayerEntityAssociations.ContainsKey(deadman))
                {
                    EntityManager.AddComponent<IsDeadTag>(PlayersSpawnerSystem.NetworkPlayerEntityAssociations[deadman]);
                    Debug.Log(string.Format("Entity({0}) - marked as died.", PlayersSpawnerSystem.NetworkPlayerEntityAssociations[deadman].Index));
                }
            }
            _toMarkAsDied.Clear();
        }
    }
}
