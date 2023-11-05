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
        private bool _playersAreReady;
        private float _timer = 3;
        protected override void OnUpdate()
        {
            if (PlayersSpawnerSystem.NetworkHandler != null && PlayersSpawnerSystem.NetworkHandler.Runner.IsServer)//only server handle things
            {
                if (!PlayersSpawnerSystem.AllPlayersSpawnedAndRegistered) return;
                else if (!_playersAreReady)
                {
                    _playersAreReady = true;
                    _timer = 3;
                }
                if(_timer > 0) { _timer -= SystemAPI.Time.DeltaTime; return; }//prevent from ending in first 3 seconds (for testing when launching solo game)
                int alive = 0, dead = 0;
                Entities.WithAll<IsPlayerTag>().ForEach((ref IsPlayerTag playerTag, in Entity entity) =>
                {
                    alive++;
                }).WithBurst().Run();
                Entities.WithAll<IsPlayerTag, IsDeadTag>().ForEach((ref IsPlayerTag playerTag, in Entity entity) =>
                {
                    dead++;
                }).WithBurst().Run();
                //Debug.Log(string.Format("Game running: alive={0}; dead={1}", alive, dead));
                if (alive - dead <= 1)
                {
                    Debug.Log(string.Format("Game ends: alive={0}; dead={1}", alive, dead));
                    GameEnds?.Invoke();
                    _playersAreReady = false;
                }
            }
        }
    }
}
