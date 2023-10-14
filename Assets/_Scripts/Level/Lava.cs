using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Level
{
    public class Lava : InteractableObject
    {
        [SerializeField] private float _flow_speed = 0.1f;
        protected override void HandleInteraction(PlayerController _playerController)
        {
            _playerController.RPC_HandleDeath();
        }
        public override void FixedUpdateNetwork()
        {
            if(Runner.IsServer)
                transform.position += Vector3.up * _flow_speed * Time.fixedDeltaTime;
        }
    }
}
