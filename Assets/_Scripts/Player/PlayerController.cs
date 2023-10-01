using Fusion;
using Fusion.Sockets;
using Game.Input;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private PlayerAnimator _playerAnimator;
        private NetworkCharacterControllerPrototype _networkCharacterController;

        private void Awake()
        {
            _networkCharacterController = GetComponent<NetworkCharacterControllerPrototype>();
        }
        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                var direction = data.direction.normalized;
                _networkCharacterController.Move(direction*Time.fixedDeltaTime);

                if ((data.buttons & NetworkInputData.MouseButton0) != 0)
                {
                    _playerAnimator.TryHit();
                }

                //if (direction.sqrMagnitude > 0)
                //    _forward = direction;

                    //if (delay.ExpiredOrNotRunning(Runner))
                    //{
                    //    delay = TickTimer.CreateFromSeconds(Runner, _shootDelay);

                    //    if ((data.buttons & NetworkInputData.MouseButton0) != 0)
                    //    {
                    //        Runner.Spawn(
                    //            prefab: _bulletPrefab,
                    //            position: transform.position + _forward,
                    //            rotation: Quaternion.LookRotation(_forward),
                    //            inputAuthority: Object.InputAuthority,
                    //            onBeforeSpawned: (_, networkObject) => { networkObject.GetComponent<Bullet>().Init(); }
                    //        );
                    //    }
                    //    else if ((data.buttons & NetworkInputData.MouseButton1) != 0)
                    //    {
                    //        Runner.Spawn(
                    //            prefab: _bombPrefab,
                    //            position: transform.position + _forward,
                    //            rotation: Quaternion.LookRotation(_forward),
                    //            inputAuthority: Object.InputAuthority,
                    //            onBeforeSpawned: (_, networkObject) => { networkObject.GetComponent<Bomb>().Init(_force); }
                    //        );
                    //        bombSpawned = !bombSpawned;
                    //    }
                    //}
            }
        }
    }

    public interface IPlayerInput
    {
        public Action OnTryHit { get; set; }
        public CharacterController CharacterController { get; set; }
    }
}
