using Game.Level;
using Game.Player;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.TextCore.Text;

namespace Game.ECS
{
    public partial class CollectedStarsCounterSystem : SystemBase
    {
        SystemHandle m_EndSimulationECBSystem;
        List<int> _addStarsToThisPlayers;
        private Entity _entity;
        private int _alreadyCollectedStars;
        protected override void OnCreate()
        {
            base.OnCreate();
            _addStarsToThisPlayers = new List<int>();
            CollectableStar.PlayerCollectTheStar += PlayerCollectTheStar;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CollectableStar.PlayerCollectTheStar -= PlayerCollectTheStar;
        }
        private void PlayerCollectTheStar(PlayerController playercontroller)
        {
            if(PlayersSpawnerSystem.NetworkHandler.Runner.IsServer)//only server handle things
                _addStarsToThisPlayers.Add(playercontroller.Object.InputAuthority.PlayerId);
        }
        protected override void OnUpdate()
        {
            foreach (var item in _addStarsToThisPlayers)
            {
                if (PlayersSpawnerSystem.NetworkPlayerEntityAssociations.ContainsKey(item))
                {
                    _entity = PlayersSpawnerSystem.NetworkPlayerEntityAssociations[item];
                    //_alreadyCollectedStars = EntityManager.GetComponentData<PlayerStarAchievement>(_entity).CollectedStarsAmount;
                    SystemAPI.GetComponentRW<PlayerStarAchievement>(_entity).ValueRW.CollectedStarsAmount += 1;
                    Debug.Log(string.Format("Entity({0}): {1}", _entity.Index, SystemAPI.GetComponentRO<PlayerStarAchievement>(_entity).ValueRO.CollectedStarsAmount));
                }
            }
            _addStarsToThisPlayers.Clear();
        }
    }
}
