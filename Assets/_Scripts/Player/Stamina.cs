using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class Stamina : NetworkBehaviour
    {
        private const float _maxStamina = 100;
        private const float _restortionPerSecond = 20;
        private const float _jumpStaminaPrice = 50;
        private const float _hitStaminaPrice = 50;
        [Networked]
        public float CurrentStamina { get; set; }
        public float Normalized
        {
            get
            {
                if (Runner == null || Runner.IsShutdown) return 0;
                return CurrentStamina / _maxStamina;
            }
        } 
        public override void Spawned()
        {
            base.Spawned();
            CurrentStamina = _maxStamina;
        }
        private void Update()
        {
            if (Runner.IsServer)
            {
                if(CurrentStamina < _maxStamina)
                {
                    CurrentStamina += _restortionPerSecond * Time.deltaTime;
                }
            }
        }
        public bool TryJump()
        {
            if(CurrentStamina >= _jumpStaminaPrice)
            {
                CurrentStamina -= _jumpStaminaPrice;
                return true;
            }
            return false;
        }
        public bool EnoughToHit()
        {
            return CurrentStamina >= _hitStaminaPrice;
        }
        public void SpendOnHit()
        {
            if (CurrentStamina >= _hitStaminaPrice)
            {
                CurrentStamina -= _hitStaminaPrice;
            }
        }
    }
}
