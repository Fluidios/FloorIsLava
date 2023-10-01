using Fusion.Sockets;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.Events;
using Fusion.Photon.Realtime;
using Game.Player;
using UnityEngine.UIElements;

namespace Game.Systems
{
    public class NetworkHandler : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private Lobby _lobby;
        [SerializeField] private int _sessionsSize = 2;
        public UnityEvent<NetworkRunner> OnConnectedToSession = new UnityEvent<NetworkRunner>();
        public UnityEvent<NetworkRunner, PlayerRef> OnPlayerJoinedToCurrentSession = new UnityEvent<NetworkRunner, PlayerRef>();
        public UnityEvent<NetworkRunner, PlayerRef> OnPlayerLeftCurrentSession = new UnityEvent<NetworkRunner, PlayerRef>();
        private NetworkRunner _runner;
        private INetworkSceneManager _sceneManager;

        private void Start()
        {
            InitializeRunner();
        }
        public void TryConnect()
        {
            ConnectToLobby(
                _runner,
                "MainLobby",
                new AuthenticationValues(_lobby.Nickname),
                null
            );
        }
        private void InitializeRunner()
        {
            _runner = gameObject.AddComponent<NetworkRunner>();
            _sceneManager = _runner.GetComponents<INetworkSceneManager>().FirstOrDefault();
            if (_sceneManager == null)
            {
                _sceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
            }
            _runner.ProvideInput = true;
        }
        protected virtual async void ConnectToLobby(
            NetworkRunner runner,
            string lobbyID,
            AuthenticationValues authentication,
            Action<NetworkRunner> connected
        )
        {
            var joinLobbyOperation = runner.JoinSessionLobby(SessionLobby.Custom, lobbyID: lobbyID/*, authentication: authentication*/);
            await joinLobbyOperation;
            if (joinLobbyOperation.Result.Ok)
            {
                connected?.Invoke(_runner);
            }
            else
            {
                Debug.LogError("Failed to join lobby. Error: " + joinLobbyOperation.Result.ErrorMessage);
            }
        }
        private void FindSessionToJoinOrCreateNew(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            string sessionToJoin = string.Empty;
            foreach (SessionInfo sessionInfo in sessionList)
            {
                if (sessionInfo.IsVisible)
                {
                    sessionToJoin = sessionInfo.Name;
                    break;
                }
            }

            Task joinSessionOperation;
            if (sessionToJoin.Length > 0)
            {
                joinSessionOperation = JoinSession(runner, GameMode.Client, sessionToJoin, _sceneManager, (runner) => OnConnectedToSession.Invoke(runner));
            }
            else
            {
                joinSessionOperation = JoinSession(runner, GameMode.Host, sessionToJoin, _sceneManager, (runner) => OnConnectedToSession.Invoke(runner));
            }
            if (joinSessionOperation.IsFaulted)
            {
                Debug.LogError("Failed to join session. Error: " + joinSessionOperation.Exception.Message);
            }
        }
        protected virtual Task JoinSession(
            NetworkRunner runner,
            GameMode gameMode,
            string sessionName,
            INetworkSceneManager sceneManager,
            Action<NetworkRunner> initialized
        )
        {

            var startGameArgs = new StartGameArgs
            {
                GameMode = gameMode,
                SessionName = sessionName,
                Initialized = initialized,
                SceneManager = sceneManager,
                PlayerCount = _sessionsSize
            };

            var startGameResult = runner.StartGame(startGameArgs);

            return startGameResult;
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
        {
            OnPlayerJoinedToCurrentSession.Invoke(runner, player); 
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) 
        {
            OnPlayerLeftCurrentSession.Invoke(runner, player);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input) { }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

        public void OnConnectedToServer(NetworkRunner runner) { }

        public void OnDisconnectedFromServer(NetworkRunner runner) { }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            FindSessionToJoinOrCreateNew(runner, sessionList);
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

        public void OnSceneLoadDone(NetworkRunner runner) { }

        public void OnSceneLoadStart(NetworkRunner runner) { }
    }
}
