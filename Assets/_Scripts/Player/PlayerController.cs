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

        [Networked]
        public Vector3 Velocity { get; set; }
        [Networked]
        public bool WantToJump { get; set; }
        [Networked]
        public bool WantToHit { get; set; }

        private void Awake()
        {
            _networkCharacterController = GetComponent<NetworkCharacterControllerPrototype>();
            _playerAnimator.Setup(this);
        }
        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                var direction = data.direction.normalized;
                _networkCharacterController.Move(direction); 
                if ((data.buttons & NetworkInputData.MouseButton0) != 0)
                {
                    _networkCharacterController.TeleportToRotation(Quaternion.LookRotation(direction));
                    WantToHit = true;
                }
                else if(WantToHit)
                {
                    WantToHit = false;
                }
                if((data.buttons & NetworkInputData.JumpCode) != 0 && _networkCharacterController.IsGrounded)
                {
                    _networkCharacterController.Velocity += transform.forward * 10;
                    _networkCharacterController.Jump();
                    WantToJump = true;
                }
                else if(WantToJump)
                {
                    WantToJump = false;
                }
                if ((data.buttons & NetworkInputData.MouseButton1) != 0)
                {
                    Debug.Log("RMC");
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
            Velocity = _networkCharacterController.Velocity;
        }

        public void Hit()
        {
            Debug.Log("Hit");
        }
    }

    public interface IPlayerInput
    {
        public Action OnTryHit { get; set; }
        public CharacterController CharacterController { get; set; }
    }
}
