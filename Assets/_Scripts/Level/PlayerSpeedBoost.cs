using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Level
{
    public class PlayerSpeedBoost : InteractableObject
    {
        [SerializeField] private float _boostPower = 2;
        [SerializeField] private float _boostLength = 10;
        [SerializeField] private GameObject _boostFx;

        static Dictionary<PlayerController, AttachedBoost> _boostedPlayers = new Dictionary<PlayerController, AttachedBoost>();

        protected override void HandleInteraction(PlayerController playerController)
        {
            if (_boostedPlayers.ContainsKey(playerController))
            {
                RemoveBoost(playerController);
            }
            AddBoost(playerController);
        }
        private void AddBoost(PlayerController playerController)
        {
            playerController.Controller.maxSpeed *= _boostPower;
            var boost = new AttachedBoost(playerController.StartCoroutine(RemoveBoostEffectFromPlayer(playerController)), Instantiate(_boostFx, playerController.transform));
            _boostedPlayers.Add(playerController, boost);
        }
        private void RemoveBoost(PlayerController playerController)
        {
            playerController.StopCoroutine(_boostedPlayers[playerController].Coroutine);
            Destroy(_boostedPlayers[playerController].FX);
            _boostedPlayers.Remove(playerController);
            playerController.Controller.maxSpeed /= _boostPower;
        }
        IEnumerator RemoveBoostEffectFromPlayer(PlayerController playerController)
        {
            yield return new WaitForSeconds(_boostLength);
            RemoveBoost(playerController);
        }

        class AttachedBoost
        {
            public Coroutine Coroutine;
            public GameObject FX;

            public AttachedBoost(Coroutine coroutine, GameObject fX)
            {
                Coroutine = coroutine;
                FX = fX;
            }
        }
    }
}
