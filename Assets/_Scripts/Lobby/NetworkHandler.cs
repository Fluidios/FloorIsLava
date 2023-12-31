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
using Game.FirebaseHandler;

namespace Game.Lobby
{
    public class NetworkHandler : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private LobbyUI _lobby;
        [SerializeField] private int _sessionsSize = 2;
        public UnityEvent<NetworkRunner> OnConnectedToSession = new UnityEvent<NetworkRunner>();
        public UnityEvent<NetworkRunner, PlayerRef> OnPlayerJoinedToCurrentSession = new UnityEvent<NetworkRunner, PlayerRef>();
        public UnityEvent<NetworkRunner, PlayerRef> OnPlayerLeftCurrentSession = new UnityEvent<NetworkRunner, PlayerRef>();
        private NetworkRunner _runner;
        private INetworkSceneManager _sceneManager;

        public NetworkRunner Runner { get { return _runner; } }

        private void Start()
        {
            InitializeRunner();
            OnConnectedToSession.AddListener((r) => Debug.Log("Connected to session " + r.SessionInfo.Name));
        }
        public void TryConnect()
        {
            ConnectToLobby(
                _runner,
                "MainLobby",
                new AuthenticationValues(UserAuth.UserNickName),
                null
            );
        }
        private void InitializeRunner()
        {
            Debug.Log("Init runner");
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
            Debug.Log("Start lobby connection...");
            var joinLobbyOperation = runner.JoinSessionLobby(SessionLobby.Custom, lobbyID: lobbyID/*, authentication: authentication*/);
            await joinLobbyOperation;
            if (joinLobbyOperation.Result.Ok)
            {
                Debug.Log("Connected to lobby "+ runner.LobbyInfo.Name);
                connected?.Invoke(_runner);
            }
            else
            { 
                Debug.LogError("Failed to join lobby. Error: " + joinLobbyOperation.Result.ErrorMessage);
            }
        }
        private async void FindSessionToJoinOrCreateNew(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            SessionInfo sessionToJoin = null;
            foreach (SessionInfo sessionInfo in sessionList)
            {
                if (sessionInfo.IsVisible)
                {
                    sessionToJoin = sessionInfo;
                    break;
                }
            }

            Task<StartGameResult> joinSessionOperation;
            if (sessionToJoin != null)
            {
                Debug.Log("Joining existing session...");
                joinSessionOperation = JoinSession(runner, GameMode.Client, sessionToJoin.Name, _sceneManager, (runner) => OnConnectedToSession.Invoke(runner));
            }
            else
            {
                Debug.Log("Creating new session...");
                joinSessionOperation = JoinSession(runner, GameMode.Host, string.Empty, _sceneManager, (runner) => OnConnectedToSession.Invoke(runner));
            }
            await joinSessionOperation;
            if (!joinSessionOperation.Result.Ok)
            {

                Debug.LogError(string.Format("Failed to join session. Reason: {0}; Error: {1};", joinSessionOperation.Result.ShutdownReason, joinSessionOperation.Result.ErrorMessage));
            }
        }
        protected virtual Task<StartGameResult> JoinSession(
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
            if (Runner.SessionInfo.PlayerCount == Runner.SessionInfo.MaxPlayers)
                Runner.SessionInfo.IsVisible = false;
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) 
        {
            OnPlayerLeftCurrentSession.Invoke(runner, player);
            Runner.SessionInfo.IsVisible = true;
        }

        public void OnInput(NetworkRunner runner, NetworkInput input) { }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

        public void OnConnectedToServer(NetworkRunner runner) { }

        public void OnDisconnectedFromServer(NetworkRunner runner) { }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { Debug.LogError(string.Format("Server connection failed: {0}", reason)); }

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
