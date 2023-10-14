using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Level
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private int _objectsToPopulate = 4;
        [SerializeField] private NetworkPrefabRef _prefab;
        [SerializeField] private Vector3 _spawnVolume = Vector3.one;
        [SerializeField] private bool _randomizeYRotation;
        private NetworkRunner _runner;
        void Start()
        {
            _runner = FindObjectOfType<NetworkRunner>();
            if (_runner.IsServer)
            {
                for (int i = 0; i < _objectsToPopulate; i++)
                {
                    _runner.Spawn(
                              _prefab,
                              new Vector3(
                                  Random.Range(transform.position.x - _spawnVolume.x, transform.position.x + _spawnVolume.x),
                                  Random.Range(transform.position.y - _spawnVolume.y, transform.position.y + _spawnVolume.y),
                                  Random.Range(transform.position.z - _spawnVolume.z, transform.position.z + _spawnVolume.z)
                                  ),
                              _randomizeYRotation ? Quaternion.Euler(0, Random.value * 360, 0) : Quaternion.identity
                              );
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, _spawnVolume);
        }
    }
}
