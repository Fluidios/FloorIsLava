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
        public override bool AsyncInitialization => false;

        public NetworkPrefabRef PrefabRef => _playerPrefab;
        public Transform[] SpawnPoints => _spawnPoints;
    }
}
