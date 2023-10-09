using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem _jumpParticles;
        private Vector3 _velocity;
        private PlayerController _playerController;

        internal void Setup(PlayerController controller)
        {
            _playerController = controller;
        }
        private void TryHit()
        {
            _animator.SetInteger("attackVar", Random.Range(0, 3));
            _animator.SetTrigger("attack");
        }
        private void TryJump()
        {
            _animator.SetTrigger("jump");
            _jumpParticles.Play();
        }

        private void Update()
        {
            _velocity = _playerController.Velocity;
            _velocity.y = 0;
            _animator.SetFloat("velocity", _velocity.sqrMagnitude);
            if(_playerController.WantToHit) 
                TryHit();
            if (_playerController.WantToJump)
                TryJump();
        }
    }
}