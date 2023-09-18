using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private PlayerAnimator _playerAnimator;
        private IPlayerInput _playerInput;
        public void Setup(IPlayerInput playerInput)
        {
            _playerInput = playerInput;
            _playerInput.CharacterController = _characterController;

            _playerAnimator.Setup(_playerInput);

        }
    }

    public interface IPlayerInput
    {
        public Action OnTryHit { get; set; }
        public CharacterController CharacterController { get; set; }
    }
}
