using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems
{
    public class LobbyStats : NetworkBehaviour
    {
        [Networked]
        public int ReadyPlayersCount { get; set; }
    }
}
