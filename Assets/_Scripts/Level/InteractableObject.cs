using Fusion;
using System.Collections;
using UnityEngine;

namespace Game.Level
{
    public abstract class InteractableObject : NetworkBehaviour
    {
        [SerializeField, Tooltip("0 mean no respawning behaviour")] private float _respawnTime;
        [SerializeField] private Hitbox _visualWithHitbox;
        [SerializeField] private bool _hideAfterInteraction = true;
        public bool Disabled { get; set; }

        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        internal void RPC_Interact(Player.PlayerController _playerController)
        {
            if (Disabled) return;
            HandleInteraction(_playerController);
            if(_hideAfterInteraction) HideAfterInteraction();
        }
        protected virtual void HandleInteraction(Player.PlayerController _playerController) { }
        private void HideAfterInteraction()
        {
            Disabled = true;
            _visualWithHitbox.gameObject.SetActive(false);
            _visualWithHitbox.HitboxActive = false;
            if (_respawnTime > 0)
                StartCoroutine(RespawnR());
        }
        private void Respawn()
        {
            Disabled = false;
            _visualWithHitbox.gameObject.SetActive(true);
            _visualWithHitbox.HitboxActive = true;
        }

        IEnumerator RespawnR()
        {
            yield return new WaitForSeconds(_respawnTime);
            Respawn();
        }
    }
}
