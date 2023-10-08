using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        private float _velocity;
        private bool _executingHitAnimation;
        private Coroutine _executingHitCoroutine;
        private PlayerController _playerController;

        internal void Setup(PlayerController controller)
        {
            _playerController = controller;
        }
        private void TryHit()
        {
            if (!_executingHitAnimation && _playerController.Velocity.sqrMagnitude < 0.1f)
            {
                _animator.SetTrigger("attack");
                _executingHitAnimation = true;
                _executingHitCoroutine = StartCoroutine(DoWithDelay(_animator.GetCurrentAnimatorStateInfo(0).length, () => _executingHitAnimation = false));
            }
        }

        private void Update()
        {
            _velocity = _playerController.Velocity.sqrMagnitude;
            _animator.SetFloat("velocity", _velocity);
            if (_velocity > 0.1f && _executingHitCoroutine != null)
            {
                StopCoroutine(_executingHitCoroutine);
                _executingHitCoroutine = null;
                _executingHitAnimation = false;
            }

            if(_playerController.WantToHit)
                TryHit();
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