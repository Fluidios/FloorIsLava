using Fusion;
using Fusion.Sockets;
using Game.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Systems
{
    public class Lobby : MonoBehaviour
    {
        public const int PlayersPerMatch = 2;
        [Header("Authentification"), SerializeField] private GameObject _authentificationForm;
        [SerializeField] TMP_InputField _emailInput;
        [SerializeField] TMP_InputField _nicknameInput;
        [SerializeField] GameObject _profileCreationConfirmationForm;
        [SerializeField] Button _createNewProfile;
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
        private string EMail
        {
            get { return PlayerPrefs.GetString("Email"); }
            set { PlayerPrefs.SetString("Email", value); }
        }
        internal string Nickname
        {
            get { return PlayerPrefs.GetString("Nick"); }
            set { PlayerPrefs.SetString("Nick", value); }
        }
        private void Awake()
        {
            Authentification();
        }
        #region Authentification
        private void Authentification()
        {
            if (EMail.Length <= 0)
            {
                _authentificationForm.SetActive(true);
                _emailInput.onEndEdit.AddListener(ValidateAuthentificationInfo);
                _nicknameInput.onEndEdit.AddListener(ValidateAuthentificationInfo);
                _createNewProfile.onClick.AddListener(CreateNewProfileWithCurrentData);
            }
            else
            {
                Debug.Log(string.Format("Signed in as {0} ({1})", Nickname, EMail));
            }
        }
        private void ValidateAuthentificationInfo(string str)
        {
            if (_emailInput.text.Length > 0 && _nicknameInput.text.Length > 0)
            {
                _profileCreationConfirmationForm.SetActive(true);
            }
            else
            {
                _profileCreationConfirmationForm.SetActive(false);
            }
        }
        private void CreateNewProfileWithCurrentData()
        {
            EMail = _emailInput.text;
            Nickname = _nicknameInput.text;
            _profileCreationConfirmationForm.SetActive(false);
            _authentificationForm.SetActive(false);
            Debug.Log(string.Format("Registered as {0} ({1})", Nickname, EMail));
        }
        #endregion
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
                StartCoroutine(LoadScene());
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
        IEnumerator LoadScene()
        {
            _loadingScreen.gameObject.SetActive(true);
            for (float t = 0; t <= 1;)
            {
                t += Time.deltaTime*2;
                _loadingScreen.alpha = t;
                yield return null;
            }

            var process = SceneManager.LoadSceneAsync("Game");
            int j = 0;
            while (!process.isDone)
            {
                _loadingScreenText.text = string.Format("Loading{0}{1}{2}", j > 0 ? "." : "", j > 1 ? "." : "", j > 2 ? "." : "");
                j = (j + 1) % 4;
                yield return new WaitForSeconds(1);
            }

            for (float t = 0; t <= 1;)
            {
                t += Time.deltaTime*2;
                _loadingScreen.alpha = 1-t;
                yield return null;
            }

            _loadingScreen.gameObject.SetActive(false);
        }
    }
}