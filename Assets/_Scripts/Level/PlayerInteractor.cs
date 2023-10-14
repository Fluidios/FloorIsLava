using Fusion;
using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Level
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerInteractor : NetworkBehaviour
    {
        [SerializeField] private Vector3 _collideableZone;
        [SerializeField] private LayerMask _interactableLayerMask;
        List<LagCompensatedHit> _hits = new List<LagCompensatedHit>();
        private InteractableObject _interactableObject;
        private PlayerController _playerController;
        private Vector3 _possitionOffset;
        public override void Spawned()
        {
            base.Spawned();
            _possitionOffset = Vector3.up * _collideableZone.y / 2;
            _playerController = GetComponent<PlayerController>();

        }
        public override void FixedUpdateNetwork()
        {
            if(_playerController.IsDead) return;
            if (Runner.IsServer)
            {
                DetectCollisions();
            }
        }
        private void DetectCollisions()
        {
            //_hitCollider = Runner.GetPhysicsScene2D().OverlapBox(transform.position, _collider.bounds.size * .9f, 0, LayerMask.GetMask("Interact"));
            if (Runner.LagCompensation.OverlapBox(transform.position + _possitionOffset, _collideableZone, transform.rotation, Object.InputAuthority, _hits, _interactableLayerMask) > 0)
            {
                foreach (var hit in _hits)
                {
                    if (hit.GameObject.transform.root.TryGetComponent<InteractableObject>(out _interactableObject))
                    {
                        _interactableObject.RPC_Interact(_playerController);
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position + Vector3.up * _collideableZone.y, _collideableZone*2);
        }
    }
}
