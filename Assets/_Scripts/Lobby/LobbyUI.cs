using Fusion;
using Fusion.Sockets;
using Game.FirebaseHandler;
using Game.Player;
using Game.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Lobby
{
    public class LobbyUI : MonoBehaviour
    {
        public const int PlayersPerMatch = 2;
        [Header("Authentification"), SerializeField] private LobbyAuthUI _authentification;
        [Header("Waiting room"), SerializeField] private GameObject _waitingForPlayersForm;
        [SerializeField] private TextMeshProUGUI _roomStatusText;
        [SerializeField] private Button _playerReadyButton;
        [SerializeField] private TextMeshProUGUI _playerReadyButtonText;
        [SerializeField] private Color _playerReadyButtonColor;
        [Header("Loading screen"), SerializeField] private CanvasGroup _loadingScreen;
        [SerializeField] private TextMeshProUGUI _loadingScreenText;

        [Header("Network prefabs&links"), SerializeField] private NetworkHandler _networkHandler;
        [SerializeField] private NetworkPrefabRef _lobbyStatsPrefab;
        LobbyStats _lobbyStats;
        NetworkRunner _networkRunner;

        private bool _iAmReadyToPlay;
        private void Start()
        {
            _authentification.Authentificate();
            _networkHandler.OnConnectedToSession.AddListener(OnConnectedToSession);
            _networkHandler.OnPlayerJoinedToCurrentSession.AddListener(OnPlayerJoined);
            _networkHandler.OnPlayerLeftCurrentSession.AddListener(OnPlayerLeft);
        }
        private void OnDestroy()
        {
            _networkHandler.OnConnectedToSession.RemoveListener(OnConnectedToSession);
            _networkHandler.OnPlayerJoinedToCurrentSession.RemoveListener(OnPlayerJoined);
            _networkHandler.OnPlayerLeftCurrentSession.RemoveListener(OnPlayerLeft);
        }
        public void TryToConnectToSession()
        {
            _loadingScreen.gameObject.SetActive(true);
            _networkHandler.TryConnect();
        }
        public void OnConnectedToSession(NetworkRunner runner)
        {
            _networkRunner = runner;
            if (runner.IsServer)
            {
                _lobbyStats = runner.Spawn(_lobbyStatsPrefab, inputAuthority: runner.LocalPlayer).gameObject.GetComponent<LobbyStats>();
            }
            //reset players ready text
            _roomStatusText.text = string.Format("0/{0}", runner.SessionInfo.MaxPlayers);
        }
        public void OnLobbyInfoLoaded(LobbyStats lobbyStats)
        {
            _lobbyStats = lobbyStats;
            _waitingForPlayersForm.SetActive(true);
            SubscribeToPlayerReadyButton(_networkRunner, _lobbyStats);
            _loadingScreen.gameObject.SetActive(false);
        }
        public void UpdatePlayersWaitingCounter(NetworkRunner runner, LobbyStats lobbyStats)
        {
            int requiredPlayersCount = runner.SessionInfo.MaxPlayers;
            _roomStatusText.text = string.Format("{0}/{1}", lobbyStats.ReadyPlayersCount, requiredPlayersCount);
        }
        private void SubscribeToPlayerReadyButton(NetworkRunner runner, LobbyStats lobbyStats)
        {
            _playerReadyButton.onClick.AddListener(() =>
            {
                if (_iAmReadyToPlay)
                {
                    _iAmReadyToPlay = false;
                    _playerReadyButtonText.text = "Not ready";
                    _playerReadyButton.targetGraphic.color = Color.white;
                    lobbyStats.RPC_PlayerIsNotReady(runner.LocalPlayer);
                }
                else
                {
                    _iAmReadyToPlay = true;
                    _playerReadyButtonText.text = "Ready";
                    _playerReadyButton.targetGraphic.color = _playerReadyButtonColor;
                    lobbyStats.RPC_PlayerIsReady(runner.LocalPlayer);
                }
            });
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
        }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
        }
        public void TryStartGame(NetworkRunner runner, LobbyStats lobbyStats)
        {
            if (lobbyStats.ReadyPlayersCount >= runner.SessionInfo.MaxPlayers)
            {

                LoadGame();
            }
            else
            {
                Debug.Log("Not enough players");
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
        private ScenesManager CreateSceneManager()
        {
            var go = new GameObject("ScenesManager");
            return go.AddComponent<ScenesManager>();
        }
        private async void LoadGame()
        {
            var sm = ScenesManager.Instance;
            if (sm == null)
                sm = CreateSceneManager();

            await sm.ChangeScene("Lobby", "Game");
        }
    }
}