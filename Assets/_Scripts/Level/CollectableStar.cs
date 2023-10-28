using Game.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Level
{
    public class CollectableStar : InteractableObject
    {
        public static Action<PlayerController> PlayerCollectTheStar;
        protected override void HandleInteraction(PlayerController _playerController)
        {
            //Debug.LogFormat("{0}({1}) collected a star", _playerController.name, _playerController.Object.InputAuthority.PlayerId);
            PlayerCollectTheStar?.Invoke(_playerController);
        }
    }
}
