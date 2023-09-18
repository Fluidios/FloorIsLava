using Game.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Player
{
    public class PlayerInput : MonoBehaviour, IPlayerInput
    {
        private CharacterController _characterController;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 5f;
        private Controls _controls;
        private Vector2 _inputVector = Vector2.zero;
        private Vector3 _moveVector = Vector3.zero;
        private Quaternion _targetRotation;
        private Camera _camera;
        private Action _onTryHit;

        public Action OnTryHit 
        {
            get => _onTryHit;
            set => _onTryHit = value; 
        }
        public CharacterController CharacterController { get => _characterController; set => _characterController = value; }
        private Vector3 CameraDependentMoveVector
        {
            get 
            {
                if(_camera == null)
                    _camera = Camera.main;
                
                _inputVector = _controls.Default.Move.ReadValue<Vector2>();
                
                _moveVector = _camera.transform.right * _inputVector.x + _camera.transform.forward * _inputVector.y;
                _moveVector.y = 0;

                return _moveVector;
            }
        }

        private void Awake()
        {
            _controls = Controls.Instance;
            _controls.Default.LClick.canceled += (ctx) => { if (_onTryHit != null) _onTryHit(); };
        }

        private void FixedUpdate()
        {

            if (CameraDependentMoveVector != Vector3.zero)
            {
                _targetRotation = Quaternion.LookRotation(_moveVector);
                transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.fixedDeltaTime * _rotationSpeed);
            }
            _characterController.Move(_moveVector * _moveSpeed * Time.fixedDeltaTime);
        }
    }
}
