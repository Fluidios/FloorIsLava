using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Animator _animator;
        private float _velocity;
        private bool _executingHitAnimation;
        private Coroutine _executingHitCoroutine;

        public void Setup(IPlayerInput playerInput)
        {
            playerInput.OnTryHit += TryHit;
        }

        private void TryHit()
        {
            if (!_executingHitAnimation && _characterController.velocity.sqrMagnitude < 0.1f)
            {
                _animator.SetTrigger("attack");
                _executingHitAnimation = true;
                _executingHitCoroutine = StartCoroutine(DoWithDelay(_animator.GetCurrentAnimatorStateInfo(0).length, () => _executingHitAnimation = false));
            }
        }

        private void Update()
        {
            _velocity = _characterController.velocity.sqrMagnitude;
            _animator.SetFloat("velocity", _velocity);
            if (_velocity > 0.1f && _executingHitCoroutine != null)
            {
                StopCoroutine(_executingHitCoroutine);
                _executingHitCoroutine = null;
                _executingHitAnimation = false;
            }
        }

        public void FootR()
        {

        }
        public void FootL()
        {

        }
        public void Hit()
        {

        }
        IEnumerator DoWithDelay(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action();
        }
    }
}