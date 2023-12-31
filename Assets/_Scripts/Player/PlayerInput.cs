using Fusion.Sockets;
using Fusion;
using Game.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using NaughtyAttributes;

namespace Game.Player
{
    public class PlayerInput : MonoBehaviour, INetworkRunnerCallbacks
    {
        private Controls _controls;
        [SerializeField, ReadOnly] private Camera _camera;
        private Vector2 _inputVector = Vector2.zero;
        private Vector3 _moveVector = Vector3.zero;
        private Quaternion _targetRotation;
        private bool _mouseButton0;
        private bool _mouseButton1;
        private bool _jumpButton;
        private Vector3 CameraDependentMoveVector
        {
            get
            {
                if (_camera == null)
                    _camera = Camera.main;

                if(_camera != null)
                {
                    _inputVector = _controls.Default.Move.ReadValue<Vector2>();

                    _moveVector = _camera.transform.right * _inputVector.x + _camera.transform.forward * _inputVector.y;
                    _moveVector.y = 0;

                    return _moveVector;
                }
                return Vector3.zero;
            }
        }
        private Vector3 CameraForward
        {
            get
            {
                if (_camera == null)
                    _camera = Camera.main;
                var v = _camera.transform.forward;
                v.y = 0;
                return v;
            }
        }

        private void Awake()
        {
            _controls = Controls.Instance;

            _controls.Default.LClick.performed += (ctx) => { _mouseButton0 = true; };
            _controls.Default.RClick.performed += (ctx) => { _mouseButton1 = true; };
            _controls.Default.Jump.performed += (ctx) => { _jumpButton = true; };
        }

        private void Update()
        {
            //   _mouseButton1 = _mouseButton1 | Input.GetMouseButton(1);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var data = new NetworkInputData();

            data.direction = CameraDependentMoveVector;

            if (_mouseButton0)
            {
                data.buttons |= NetworkInputData.MouseButton0;
                data.direction = CameraForward;
            }
            _mouseButton0 = false;

            if (_mouseButton1)
                data.buttons |= NetworkInputData.MouseButton1;
            _mouseButton1 = false;

            if (_jumpButton)
                data.buttons |= NetworkInputData.JumpCode;
            _jumpButton = false;

            input.Set(data);
        }

        #region Not used callbacks
        public void OnConnectedToServer(NetworkRunner runner) { }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

        public void OnDisconnectedFromServer(NetworkRunner runner) { }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

        public void OnSceneLoadDone(NetworkRunner runner) { }

        public void OnSceneLoadStart(NetworkRunner runner) { }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        #endregion
    }
}
