using Cinemachine;
using Game.SystemsManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems
{
    public class CameraSystem : GameSystem
    {
        [SerializeField] CinemachineVirtualCamera _camera;

        public override bool AsyncInitialization => false;

        public void SetTarget(Transform target)
        {
            _camera.Follow = target;
            _camera.LookAt = target;
        }
    }
}