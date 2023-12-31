using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Lobby
{
    public class LobbyStats : NetworkBehaviour
    {
        [Networked]
        public int ReadyPlayersCount { get; set; }
        private int _localReadyPLayersCount = 0;
        private LobbyUI _lobby;

        private void Awake()
        {
            _lobby = FindObjectOfType<LobbyUI>();
            if (_lobby != null)
                _lobby.OnLobbyInfoLoaded(this);
            else
                Debug.LogWarning("No lobby found!");
        }

        [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
        public void RPC_PlayerIsReady(PlayerRef player)
        {
            ReadyPlayersCount++; 
        }

        [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
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

                if(_lobby != null)
                {
                    _lobby.UpdatePlayersWaitingCounter(Runner, this);
                    _lobby.TryStartGame(Runner, this);
                }
            }
        }
    }
}
