using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Entities;
using System;
using Game.Lobby;
using Fusion;
using System.Collections.Generic;
using Game.System;
using Game.SystemsManagement;

namespace Game.ECS
{
    public partial class PlayersSpawnerSystem : SystemBase
    {
        internal static NetworkHandler NetworkHandler { get; private set; }
        internal static Dictionary<int, Entity> NetworkPlayerEntityAssociations = new Dictionary<int, Entity>();
        private PlayersSpawner _playersSpawner;
        private List<PlayerRef> _playersToSpawn;
        private List<PlayerRef> _playersToDespawn;
        private bool _isGameSceneActive;
        private Unity.Mathematics.Random _random;
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            _random = Unity.Mathematics.Random.CreateFromIndex(0);
            _playersToSpawn = new List<PlayerRef>();
            _playersToDespawn = new List<PlayerRef>();
            SceneManager.sceneLoaded += OnNewSceneLoaded;
        }
        protected override void OnStopRunning()
        {
            base.OnStopRunning();
            SceneManager.sceneLoaded -= OnNewSceneLoaded;
        }

        private void OnNewSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            switch (arg0.name)
            {
                case "Lobby":
                    FindAndSubscribeToNetworkHandlerIfNeeded();
                    _playersToSpawn.Clear();
                    _playersToDespawn.Clear();
                    _isGameSceneActive = false;
                    break;
                case "Game":
                    OnLoadToGameScene();
                    break;
            }
        }

        private void FindAndSubscribeToNetworkHandlerIfNeeded()
        {
            if(NetworkHandler == null)
            {
                NetworkHandler = GameObject.FindObjectOfType<NetworkHandler>();
                NetworkHandler.OnPlayerJoinedToCurrentSession.AddListener(OnPlayerJoined);
                NetworkHandler.OnPlayerLeftCurrentSession.AddListener(OnPlayerLeft);
            }
        }
        private void OnLoadToGameScene()
        {
            _playersSpawner = SystemsManager.GetSystemOfType<PlayersSpawner>();
            _isGameSceneActive = true;
        }

        private void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
        {
            _playersToSpawn.Add(playerRef);
        }
        private void OnPlayerLeft(NetworkRunner runner, PlayerRef playerRef)
        {
            _playersToDespawn.Add(playerRef);
        }

        protected override void OnUpdate()
        {
            if (NetworkHandler == null || NetworkHandler.Runner == null) { return; }
            if (NetworkHandler.Runner.IsServer) //only server should remove player bodies and spawn them
            {
                foreach (var player in _playersToDespawn)
                {
                    if (NetworkHandler.Runner.TryGetPlayerObject(player, out var playerObj))
                    {
                        TryReleasePlayersEntity(playerObj);
                        NetworkHandler.Runner.Despawn(playerObj);
                        NetworkHandler.Runner.SetPlayerObject(player, null);
                    }
                }
                _playersToDespawn.Clear();
                if (!_isGameSceneActive) return;//we dont wont to spawn players in waiting room of lobby
                foreach (var player in _playersToSpawn)
                {
                    if (!NetworkHandler.Runner.TryGetPlayerObject(player, out var playerObj))
                    {
                        playerObj = NetworkHandler.Runner.Spawn(
                                  _playersSpawner.PrefabRef,
                                  _playersSpawner.SpawnPoints[_random.NextInt(0, _playersSpawner.SpawnPoints.Length)].position,
                                  Quaternion.Euler(0, _random.NextFloat() * 360, 0),
                                  inputAuthority: player
                                  );

                        NetworkHandler.Runner.SetPlayerObject(player, playerObj);
                        TryConvertPlayerToEntity(player, playerObj);
                    }
                }
                _playersToSpawn.Clear();
            }
        }

        private void TryConvertPlayerToEntity(PlayerRef playerRef, NetworkObject spawnedPlayer)
        {
            var playerAuthoring = spawnedPlayer.GetComponent<PlayerComponentsAuthoring>();
            if (playerAuthoring != null)
            {
                var archetype = EntityManager.CreateArchetype(playerAuthoring.GetArchetypeTypes());
                var entity = EntityManager.CreateEntity(archetype);
                EntityManager.SetComponentData(entity, playerAuthoring.GetStarAchievementAuthoringData());
                NetworkPlayerEntityAssociations.Add(playerRef.PlayerId, entity);
            }
        }
        private void TryReleasePlayersEntity(NetworkObject deadman)
        {
            if(NetworkPlayerEntityAssociations.ContainsKey(deadman.InputAuthority.PlayerId))
            {
                EntityManager.DestroyEntity(NetworkPlayerEntityAssociations[deadman.InputAuthority.PlayerId]);
                NetworkPlayerEntityAssociations.Remove(deadman.InputAuthority.PlayerId);
            }
        }
    }
}
