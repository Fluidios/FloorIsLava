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
        [SerializeField] private Hitbox _hitbox;
        private NetworkCharacterControllerPrototype _networkCharacterController;
        private bool _isDead;

        [Networked]
        public Vector3 Velocity { get; set; }
        public bool IsGrounded => _networkCharacterController.IsGrounded;
        public NetworkCharacterControllerPrototype Controller => _networkCharacterController;

        public bool IsDead { get => _isDead;}

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
            if(_isDead) return;
            if (_isPushedAndFalling)
            {
                _networkCharacterController.Velocity = _currentPushVelocity + Physics.gravity;
                _networkCharacterController.Move(Vector3.zero);
                return;
            }
            if (GetInput(out NetworkInputData data))
            {
                var direction = data.direction.normalized;
                _networkCharacterController.Move(direction);
                if ((data.buttons & NetworkInputData.MouseButton0) != 0)
                {
                    if (_stamina.EnoughToHit())
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
            if (Runner.IsServer)
            {
                _stamina.SpendOnHit();
                _pusher.TryPushWithHit();
            }
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
            _hitbox.enabled = false;
            return _playerAnimator.TryFall(()=>
            {
                _hitbox.enabled = true;
                _isPushedAndFalling = false;
            });
        }
        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        internal void RPC_OnHitSomething()
        {
            _playerAnimator.OnHitSomething();
        }
        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        public void RPC_HandleDeath()
        {
            _isDead = true;
            _playerAnimator.HandleDeath();
            _hitbox.enabled = false;
            Debug.Log("Dead");
        }
        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        internal void RPC_OnBeingPushed(Vector3 pushPower)
        {
            Vector3 lookDir = -pushPower;
            lookDir.y = 0;
            _networkCharacterController.TeleportToRotation(Quaternion.LookRotation(lookDir));
            _isPushedAndFalling = true;
            pushPower.y *= 2;
            StartCoroutine(SimulateFallVelocity(HandleFall(), pushPower));
        }

        IEnumerator SimulateFallVelocity(float fallLengthTime, Vector3 fallStartVelocity)
        {
            float totalTime = fallLengthTime;
            while(fallLengthTime > 0)
            {
                if (!_isPushedAndFalling) break;
                fallLengthTime -= fallLengthTime * Time.deltaTime;
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
