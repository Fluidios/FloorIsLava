using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Systems
{
    public class NetworkRoomStats : MonoBehaviour, INetworkRunnerCallbacks
    {
        public static NetworkRoomStats Instance { get; private set; }
        public NetworkRunner Runner { get; private set; }
        public Action<NetworkRunner, PlayerRef> PlayerJoined { get; set; }
        public Action<NetworkRunner, PlayerRef> PlayerLeft { get; set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(Instance.gameObject);
                Instance = this;
            }
            DontDestroyOnLoad(gameObject);

            Runner = gameObject.AddComponent<NetworkRunner>();
            Runner.ProvideInput = true;
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("Player joined");
            if(PlayerJoined != null)
                PlayerJoined(runner, player);

            if (Runner.SessionInfo.PlayerCount == Runner.SessionInfo.MaxPlayers)
            {
                Runner.SessionInfo.IsVisible = false;
                Debug.Log("Session is full");
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("Player left");
            if (PlayerLeft != null)
                PlayerLeft(runner, player);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            //Debug.Log("On input");
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            //Debug.Log("On Input missing");
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log("On Shutdown, reason: " + shutdownReason.ToString());
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("On connected to server");
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            Debug.Log("On deisconnected from server");
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            Debug.Log("On connect request");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Debug.Log("On connect failed");
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            Debug.Log("On user simulation message");
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            Debug.Log("Sessions list updated, sessions count: " + sessionList.Count);
            string sessionToJoin = string.Empty;
            foreach (SessionInfo sessionInfo in sessionList)
            {
                if(sessionInfo.IsVisible)
                {
                    sessionToJoin = sessionInfo.Name;
                    break;
                }
            }

            if(sessionToJoin.Length > 0)
            {
                JoinTheSession(GameMode.Client, sessionToJoin);
            }
            else
            {
                JoinTheSession(GameMode.Host, sessionToJoin);
            }
        }

        private async void JoinTheSession(GameMode gameMode, string sessionName)
        {
            if(gameMode == GameMode.Client)
            {
                Debug.Log("Joining session: " + sessionName);
                await Runner.StartGame(new StartGameArgs()
                {
                    GameMode = gameMode,
                    SessionName = sessionName,
                    Scene = SceneManager.GetActiveScene().buildIndex,
                    SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
                });
            }
            else
            {
                var result = await Runner.StartGame(new StartGameArgs()
                {
                    GameMode = gameMode,
                    CustomLobbyName = "MainLobby",
                    Scene = SceneManager.GetActiveScene().buildIndex,
                    SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
                    PlayerCount = Lobby.PlayersPerMatch
                });
                if(result.ShutdownReason != ShutdownReason.Ok)
                {
                    Debug.Log(result.ShutdownReason.ToString() + " : " + result.ErrorMessage);
                }
                else
                {
                    Debug.Log("Creating session: " + Runner.SessionInfo.Name);
                }
            }
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            Debug.Log("On custom auth response");
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            Debug.Log("On host migration");
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
            Debug.Log("Reliable data received");
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            Debug.Log("Scene loading - done");
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log("Scene loading - started");
        }
    }
}
