using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class PlayerEntity : NetworkBehaviour
    {
        [Networked]
        public string Name { get; set; }

        //private void Start()
        //{
        //    if(Object.HasInputAuthority)
        //        RPC_PlayerEntitySpawned();
        //}

        //[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
        //public void RPC_PlayerEntitySpawned()
        //{
        //    Debug.Log(string.Format("Player is spawned. Secret = " + PlayerPrefs.GetString("Nick")));
        //}
    }
}
 