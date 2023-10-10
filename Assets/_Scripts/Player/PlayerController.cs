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
        [SerializeField] private Pusher _pusher;
        [SerializeField] private DynamicBody _dynamicBody;
        private NetworkCharacterControllerPrototype _networkCharacterController;

        [Networked]
        public Vector3 Velocity { get; set; }
        private bool _isPushedAndFalling;

        private void Awake()
        {
            _networkCharacterController = GetComponent<NetworkCharacterControllerPrototype>();
            _playerAnimator.Setup(this);
            _pusher.Setup(this);
            _dynamicBody.Setup(this);
        }
        public override void FixedUpdateNetwork()
        {
            if (_isPushedAndFalling)
            {
                _networkCharacterController.Move(Vector3.zero);
                return;
            }
            if (GetInput(out NetworkInputData data))
            {
                var direction = data.direction.normalized;
                _networkCharacterController.Move(direction);
                if ((data.buttons & NetworkInputData.MouseButton0) != 0)
                {
                    _networkCharacterController.TeleportToRotation(Quaternion.LookRotation(direction));
                    if (Object.HasInputAuthority) RPC_HandleHit();
                }
                if ((data.buttons & NetworkInputData.JumpCode) != 0 && _networkCharacterController.IsGrounded)
                {
                    _networkCharacterController.Jump();
                    if(Object.HasInputAuthority) RPC_HandleJump();
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
            if(Runner.IsServer)
                _pusher.TryPushWithHit();
        }
        [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
        internal void RPC_HandleHit()
        {
            Debug.Log("H");
            _playerAnimator.TryHit();
        }
        [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
        internal void RPC_HandleJump()
        {
            Debug.Log("J");
            _playerAnimator.TryJump();
        }
        internal void HandleFall()
        { 
            Debug.Log("F");
            _playerAnimator.TryFall(()=>_isPushedAndFalling = false);
        }
        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        internal void RPC_OnBeingPushed(Vector3 pushPower)
        {
            _networkCharacterController.TeleportToRotation(Quaternion.LookRotation(-pushPower));
            _isPushedAndFalling = true;
            HandleFall();
        }
    }

    public interface IPlayerInput
    {
        public Action OnTryHit { get; set; }
        public CharacterController CharacterController { get; set; }
    }
}
