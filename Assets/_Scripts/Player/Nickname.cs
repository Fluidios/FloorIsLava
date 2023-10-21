using Fusion;
using Game.FirebaseHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class Nickname : NetworkBehaviour
    {
        [Networked]
        public string PlayerNickname { get; set; }
        public override void Spawned()
        {
            if (Object.HasInputAuthority)
                gameObject.name = string.Format("Player({0})", UserAuth.UserNickName);
        }
    }
}
