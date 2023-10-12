using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Level
{
    public abstract class InteractableObject : NetworkBehaviour
    {
        [SerializeField, Tooltip("0 mean no respawning behaviour")] private float _respawnTime;
        [SerializeField] private Hitbox _hitbox;
        [SerializeField] private GameObject _visual;

        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        internal virtual void RPC_Interact(Player.PlayerController _playerController)
        {
            AfterInteraction();
        }
        private void AfterInteraction()
        {
            _hitbox.enabled = false;
            _visual.SetActive(false);
            if (_respawnTime > 0)
                StartCoroutine(RespawnR());
        }
        private void Respawn()
        {
            _hitbox.enabled = true;
            _visual.SetActive(true);
        }

        IEnumerator RespawnR()
        {
            yield return new WaitForSeconds(_respawnTime);
            Respawn();
        }
    }
}
