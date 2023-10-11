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
        [SerializeField] private Stamina _stamina;
        private NetworkCharacterControllerPrototype _networkCharacterController;

        [Networked]
        public Vector3 Velocity { get; set; }
        private bool _isPushedAndFalling;
        private Vector3 _currentPushVelocity;

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
                _networkCharacterController.Move(_currentPushVelocity);
                return;
            }
            if (GetInput(out NetworkInputData data))
            {
                var direction = data.direction.normalized;
                _networkCharacterController.Move(direction);
                if ((data.buttons & NetworkInputData.MouseButton0) != 0)
                {
                    if (_stamina.TryHit())
                    {
                        _networkCharacterController.TeleportToRotation(Quaternion.LookRotation(direction));
                        if (Object.HasInputAuthority) RPC_HandleHit();
                    }
                }
                if ((data.buttons & NetworkInputData.JumpCode) != 0 && _networkCharacterController.IsGrounded)
                {
                    if (_stamina.TryJump())
                    {
                        _networkCharacterController.Jump();
                        if (Object.HasInputAuthority) RPC_HandleJump();
                    }
                }
                if ((data.buttons & NetworkInputData.MouseButton1) != 0)
                {
                    Debug.Log("RMC");
                }
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
            _playerAnimator.TryHit();
        }
        [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
        internal void RPC_HandleJump()
        {
            _playerAnimator.TryJump();
        }
        internal float HandleFall()
        { 
            Debug.Log("F");
            return _playerAnimator.TryFall(()=>_isPushedAndFalling = false);
        }
        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        internal void RPC_OnBeingPushed(Vector3 pushPower)
        {
            _networkCharacterController.TeleportToRotation(Quaternion.LookRotation(-pushPower));
            _isPushedAndFalling = true;
            StartCoroutine(SimulateFallVelocity(HandleFall(), pushPower));
        }

        IEnumerator SimulateFallVelocity(float fallLengthTime, Vector3 fallStartVelocity)
        {
            float totalTime = fallLengthTime;
            while(fallLengthTime > 0)
            {
                if (!_isPushedAndFalling) break;
                fallLengthTime -= fallLengthTime * Time.deltaTime * 10;
                _currentPushVelocity = fallStartVelocity * (fallLengthTime/totalTime);
                yield return null;
            } 
        }
    }

    public interface IPlayerInput
    {
        public Action OnTryHit { get; set; }
        public CharacterController CharacterController { get; set; }
    }
}
