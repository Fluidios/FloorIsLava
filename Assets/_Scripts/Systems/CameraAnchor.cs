using Fusion;
using Game.Player;
using Game.SystemsManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Systems
{
    public class CameraAnchor : NetworkBehaviour
    {
        private CameraSystem _cameraSystem;
        private bool _active;
        private PlayerController _myPlayerController;
        private PlayerController _observedPlayerController;
        private bool _observerMode;
        private List<PlayerController> _players;
        private void Awake()
        {
            _cameraSystem = SystemsManager.GetSystemOfType<CameraSystem>();
            StartCoroutine(WaitForSelfNetworkObjectInitialization(TryAttachCameraToMe));
        }
        private void TryAttachCameraToMe()
        {
            if (Object.HasInputAuthority)
            {
                _cameraSystem.SetTarget(transform);
                _myPlayerController = GetComponent<PlayerController>();
                _active = true;
            }
        }
        private void Update()
        {
            if (_active)
            {
                if (_myPlayerController.IsDead)
                {
                    TryRetarget();
                }
            }
            else if(_observerMode)
            {
                //switching between alive players
                if (_observedPlayerController.IsDead)
                    TryRetarget();
            }
        }
        private void TryRetarget()
        {
            _observedPlayerController = FindRandomAlivePlayer();
            _observerMode = _observedPlayerController != null;
            if (_observerMode)
                _cameraSystem.SetTarget(_observedPlayerController.transform);
        }
        private PlayerController FindRandomAlivePlayer()
        {
            if(_players == null)
            {
                _players = new List<PlayerController>();
                _players.AddRange(GameObject.FindObjectsOfType<PlayerController>());
            }
            List<PlayerController> _alive = new List<PlayerController>();
            foreach (PlayerController player in _players) { if(player.IsDead) continue; _alive.Add(player); }
            if(_alive.Count > 0)
                return _alive[UnityEngine.Random.Range(0, _alive.Count)];
            else return null;
        }

        IEnumerator WaitForSelfNetworkObjectInitialization(Action actionAfterInitialization)
        {
            yield return new WaitWhile(() => Object == null);
            actionAfterInitialization();
        }
    }
}
