using Fusion;
using Game.Input;
using Game.Player;
using Game.SystemsManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
        private Controls _controls;
        private List<PlayerController> AllPlayers
        {
            get
            {
                if (_players == null)
                {
                    _players = new List<PlayerController>();
                    foreach (var player in Runner.ActivePlayers)
                    {
                        _players.Add(Runner.GetPlayerObject(player).GetComponent<PlayerController>());
                    }
                }
                return _players;
            }
        }

        private void Awake()
        {
            _cameraSystem = SystemsManager.GetSystemOfType<CameraSystem>();
            StartCoroutine(WaitForSelfNetworkObjectInitialization(TryAttachCameraToMe));
            _controls = Controls.Instance;
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
                    _active = false;
                    TryRetarget();
                    _controls.Default.Previous.performed += TryRetargetToPreviousAlivePlayer;
                    _controls.Default.Next.performed += TryRetargetToNextAlivePlayer;
                }
            }
            else if(_observerMode)
            {
                if (_observedPlayerController.IsDead)
                    TryRetarget();
            }
        }
        private void TryRetarget()
        {
            _observedPlayerController = FindRandomAlivePlayer();
            _observerMode = _observedPlayerController != null;
            if (_observerMode)
            {
                Debug.Log("Set target to " + _observedPlayerController.name);
                _cameraSystem.SetTarget(_observedPlayerController.transform);
            }
        }
        private List<PlayerController> GetAlivePlayers()
        {
            List<PlayerController> _alive = new List<PlayerController>();
            foreach (PlayerController player in AllPlayers) { if (player.IsDead) continue; _alive.Add(player); }
            return _alive;
        }
        private PlayerController FindRandomAlivePlayer()
        {
            var alive = GetAlivePlayers();
            if(alive.Count > 0)
                return alive[UnityEngine.Random.Range(0, alive.Count)];
            else return null;
        }
        private void TryRetargetToPreviousAlivePlayer(InputAction.CallbackContext context)
        {
            var alive = GetAlivePlayers();
            if (alive.Count > 1)
            {
                int currentlyObservedPlayerIndex = alive.IndexOf(_observedPlayerController);
                int targetIndex = currentlyObservedPlayerIndex - 1;
                RetargetByIndex(targetIndex, alive);
            }
        }
        private void TryRetargetToNextAlivePlayer(InputAction.CallbackContext context)
        {
            var alive = GetAlivePlayers();
            if (alive.Count > 1)
            {
                int currentlyObservedPlayerIndex = alive.IndexOf(_observedPlayerController);
                int targetIndex = currentlyObservedPlayerIndex + 1;
                RetargetByIndex(targetIndex, alive);
            }
        }
        private void RetargetByIndex(int index, List<PlayerController> list)
        {
            if (index < 0)
            {
                index = list.Count - 1;
            }
            else if (index >= list.Count)
            {
                index = 0;
            }
            Debug.Log("Set target to " + _observedPlayerController.name);
            _observedPlayerController = list[index];
            _cameraSystem.SetTarget(_observedPlayerController.transform);
        }

        IEnumerator WaitForSelfNetworkObjectInitialization(Action actionAfterInitialization)
        {
            yield return new WaitWhile(() => Object == null);
            actionAfterInitialization();
        }
    }
}
