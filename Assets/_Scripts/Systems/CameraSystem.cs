using Cinemachine;
using Game.SystemsManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems
{
    public class CameraSystem : GameSystem
    {
        [SerializeField] CinemachineVirtualCamera _camera;
        private CinemachinePOV _transposer;
        public Action<Transform> TargetSettled;
        public override bool AsyncInitialization => false;

        private void Awake()
        {
            _transposer = _camera.GetCinemachineComponent<CinemachinePOV>();
        }

        public void SetTarget(Transform target)
        {
            _camera.Follow = target;
            _camera.LookAt = target;
            if(TargetSettled != null)
            {
                TargetSettled(target);
            }
        }
        public void RotateAroundTarget(float rotationInput)
        {
            _transposer.m_HorizontalAxis.Value += rotationInput;
        }
        public void RotateVertically(float rotationInput)
        {
            _transposer.m_VerticalAxis.Value += rotationInput;
        }
    }
}