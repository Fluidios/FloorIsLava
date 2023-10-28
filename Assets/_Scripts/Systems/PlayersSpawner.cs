using Fusion;
using Game.Lobby;
using Game.Player;
using Game.Systems;
using Game.SystemsManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.System
{
    public class PlayersSpawner : GameSystem
    {
        [SerializeField] private NetworkPrefabRef _playerPrefab;
        [SerializeField] private Transform[] _spawnPoints;
        private NetworkHandler _networkHandler;
        public override bool AsyncInitialization => false;

        private void Awake()
        {
            _networkHandler = FindObjectOfType<NetworkHandler>();
            _networkHandler.OnPlayerJoinedToCurrentSession.AddListener(OnPlayerJoined);
            _networkHandler.OnPlayerLeftCurrentSession.AddListener(OnPlayerLeft);

            SpawnClients();
        }
        private void SpawnClients()
        {
            var runner = FindObjectOfType<NetworkRunner>();

            if (runner.IsServer)
            {
                NetworkObject player; 
                foreach (var playerRef in runner.ActivePlayers)
                {
                    player = runner.Spawn(
                              _playerPrefab,
                              _spawnPoints[Random.Range(0, _spawnPoints.Length)].position,
                              Quaternion.Euler(0, Random.value * 360, 0),
                              inputAuthority: playerRef
                              );

                    runner.SetPlayerObject(playerRef, player);
                    //PlayerInput playerInput = player.gameObject.AddComponent<PlayerInput>();

                    //player.GetComponent<PlayerController>().Setup(playerInput);
                }
            }
        }

        private void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer) //spawn back players if they was disconnected?
            {
                if (!runner.TryGetPlayerObject(player, out var playerObj))
                {
                    playerObj = runner.Spawn(
                              _playerPrefab,
                              _spawnPoints[Random.Range(0, _spawnPoints.Length)].position,
                              Quaternion.Euler(0, Random.value * 360, 0),
                              inputAuthority: player
                              );

                    runner.SetPlayerObject(player, playerObj);
                }
            }
        }
        private void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer) //only server should remove player bodies?
            {
                if (runner.TryGetPlayerObject(player, out var playerObj))
                {
                    runner.Despawn(playerObj);
                    runner.SetPlayerObject(player, null);
                }
            }
        }
    }
}
