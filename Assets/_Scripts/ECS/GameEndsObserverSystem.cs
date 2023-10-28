using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Game.ECS
{
    public partial class GameEndsObserverSystem : SystemBase
    {
        public static Action GameEnds;
        protected override void OnUpdate()
        {
            if (PlayersSpawnerSystem.NetworkHandler != null && PlayersSpawnerSystem.NetworkHandler.Runner.IsServer)//only server handle things
            {
                int alive = 0, dead = 0;
                Entities.WithAll<IsPlayerTag>().ForEach((ref IsPlayerTag playerTag, in Entity entity) =>
                {
                    alive++;
                }).WithBurst().Run();
                Entities.WithAll<IsPlayerTag, IsDeadTag>().ForEach((ref IsPlayerTag playerTag, in Entity entity) =>
                {
                    alive++;
                    dead++;
                }).WithBurst().Run();
                if (alive - dead <= 1) GameEnds?.Invoke();
            }
        }
    }
}
