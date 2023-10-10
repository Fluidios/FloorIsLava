using Fusion;
using System;
using UnityEngine;

namespace Game.Player
{
    internal class DynamicBody : NetworkBehaviour
    {
        [SerializeField] private NetworkCharacterControllerPrototype _networkCharacterController;
        [SerializeField] private NetworkRigidbody _physicsBody;
        private PlayerController _playerController;

        internal void Setup(PlayerController playerController)
        {
            _playerController = playerController;
        }
        internal void Push(Vector3 vector3, bool callRPC = false)
        {
            if (!Runner.IsServer) return;
            Debug.Log(Runner.LocalPlayer + ": is pushed.");
            if (_physicsBody != null)
                _physicsBody.Rigidbody.AddForce(vector3, ForceMode.VelocityChange);
            else if (_networkCharacterController != null)
            {
                _networkCharacterController.Velocity += vector3;
                if(callRPC) _playerController.RPC_OnBeingPushed(vector3);
            }
        }
    }
}