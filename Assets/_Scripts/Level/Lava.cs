using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Level
{
    public class Lava : InteractableObject
    {
        [SerializeField] private float _flow_speed = 0.1f;
        internal override void RPC_Interact(PlayerController _playerController)
        {
            _playerController.RPC_HandleDeath();
        }
        public override void FixedUpdateNetwork()
        {
            transform.position += Vector3.up * _flow_speed * Time.fixedDeltaTime;
        }
    }
}
