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
        [SerializeField] private PlayerController _playerPrefab;
        [SerializeField] private Transform[] _spawnPoints;

        public override bool AsyncInitialization => false;

        private void Awake()
        {
            SpawnClient();
        }
        private void SpawnClient()
        {
            var playerController = Instantiate(_playerPrefab, _spawnPoints[Random.Range(0, _spawnPoints.Length)].position, Quaternion.Euler(0,Random.value*360,0));
  
            PlayerInput playerInput = playerController.gameObject.AddComponent<PlayerInput>();

            playerController.Setup(playerInput);

            SystemsManager.GetSystemOfType<CameraSystem>().SetTarget(playerController.transform);
        }
    }
}
