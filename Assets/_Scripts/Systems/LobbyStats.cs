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
        private int _localReadyPLayersCount = 0;
        private Lobby _lobby;

        private void Awake()
        {
            _lobby = FindObjectOfType<Lobby>();
            _lobby.OnLobbyInfoLoaded(this);
        }

        [Rpc(sources: RpcSources.All, targets: RpcTargets.InputAuthority)]
        public void RPC_PlayerIsReady(PlayerRef player)
        {
            ReadyPlayersCount++; 
        }

        [Rpc(sources: RpcSources.All, targets: RpcTargets.InputAuthority)]
        public void RPC_PlayerIsNotReady(PlayerRef player)
        {
            ReadyPlayersCount--;
        }

        private void Update()
        {
            if(_localReadyPLayersCount != ReadyPlayersCount)
            {
                _localReadyPLayersCount = ReadyPlayersCount;
                Debug.Log(string.Format("Ready players count = {0}/{1}",_localReadyPLayersCount, Runner.SessionInfo.MaxPlayers));

                _lobby.UpdatePlayersWaitingCounter(Runner, this);
                _lobby.TryStartGame(Runner, this);
            }
        }
    }
}
