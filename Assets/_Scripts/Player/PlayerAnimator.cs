using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem _jumpParticles;
        [SerializeField] private ParticleSystem _hitParticles;
        private Vector3 _velocity;
        private PlayerController _playerController;
        private bool _isFalling;
        internal void Setup(PlayerController controller)
        {
            _playerController = controller;
        }
        internal void TryHit()
        {
            _animator.SetInteger("attackVar", UnityEngine.Random.Range(0, 3));
            _animator.SetTrigger("attack");
        }
        internal void TryJump()
        {
            _animator.SetTrigger("jump");
            _jumpParticles.Play();
        }
        internal float TryFall(Action onEndFall)
        {
            _animator.SetTrigger("fall");
            float fallLength = _animator.GetCurrentAnimatorStateInfo(0).length;
            StartCoroutine(ShowParticlesOnSurfaceCollision());
            StartCoroutine(DoWithDelay(fallLength, onEndFall));
            return fallLength;
        }

        private void Update()
        {
            _velocity = _playerController.Velocity;
            _velocity.y = 0;
            _animator.SetFloat("velocity", _velocity.sqrMagnitude);
            //if (_playerController.WantToHit)
            //    TryHit();
            //if (_playerController.WantToJump)
            //    TryJump();
        }

        IEnumerator DoWithDelay(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action();
        }
        IEnumerator ShowParticlesOnSurfaceCollision()
        {
            while (!_playerController.IsGrounded)
            {
                yield return null;
            }
            _jumpParticles.Play();
        }

        internal void OnHitSomething()
        {
            _hitParticles.Play();
        }
    }
}