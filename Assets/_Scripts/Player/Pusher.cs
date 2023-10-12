using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class Pusher : NetworkBehaviour
    {
        private const float _walkingPushLimit = 0.75f;
        [SerializeField] private bool _continuousPushing;
        [SerializeField] private Transform _playerCenterPoint;
        [SerializeField] private float _pushRayLength = 1;
        [SerializeField] private LayerMask _regularPushLayerMask;
        [SerializeField] private float _pushPower = 10;
        private bool _didHit;
        private List<LagCompensatedHit> _hits = new List<LagCompensatedHit>();
        private LagCompensatedHit _hit;
        private PlayerController _playerController;

        internal void Setup(PlayerController playerController)
        {
            _playerController = playerController;
        }

        public override void FixedUpdateNetwork()
        {
            if (_continuousPushing && Runner.IsServer)
            {
                _didHit = Runner.LagCompensation.Raycast(_playerCenterPoint.position, transform.forward, _pushRayLength, Object.InputAuthority, out _hit, _regularPushLayerMask, HitOptions.IncludePhysX);
                Debug.DrawRay(_playerCenterPoint.position, _playerCenterPoint.forward * _pushRayLength, _didHit ? Color.red : Color.white);
                if (_didHit && _hit.Hitbox != null)
                {
                    _hit.Hitbox.transform.root.GetComponent<DynamicBody>().Push((_hit.Point - _playerCenterPoint.position).normalized * Mathf.Clamp(_playerController.Velocity.sqrMagnitude, 0, _walkingPushLimit));
                }
            }
        }

        public void TryPushWithHit()
        {
            if (!Runner.IsServer) return;
            
            _didHit = Runner.LagCompensation.OverlapSphere(_playerCenterPoint.position, 0.4f, Object.InputAuthority, _hits, _regularPushLayerMask, HitOptions.IncludePhysX) > 1;
            //_didHit = Runner.LagCompensation.Raycast(_playerCenterPoint.position, transform.forward, _pushRayLength*2, Object.InputAuthority, out _hit, _regularPushLayerMask, HitOptions.IncludePhysX);

            if (_didHit)
            {
                foreach (var hit in _hits)
                {
                    if (hit.Hitbox != null)
                    {
                        if (hit.Hitbox.transform.root == transform) continue;
                        hit.Hitbox.transform.root.GetComponent<DynamicBody>().Push(Vector3.up * 10 + (hit.Point - _playerCenterPoint.position).normalized * _pushPower, true);
                        _playerController.RPC_OnHitSomething();
                    }
                    else if (hit.Collider != null) 
                    {
                        //do nothing if we hit something static
                    }
                }
            }
        }
    }
}
