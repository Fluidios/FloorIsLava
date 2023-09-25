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

        [Header("Prefabs"), SerializeField] private PlayerEntity _playerPrefab;
        [SerializeField] private NetworkObject _lobbyStatsPrefab;

        NetworkRoomStats _roomStats;
        LobbyStats _lobbyStats;

        private bool _iAmReadyToPlay;
        private string EMail
        {
            get { return PlayerPrefs.GetString("Email"); }
            set { PlayerPrefs.SetString("Email", value); }
        }
        private string Nickname
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

        public void JoinTheGame()
        {
            //SceneManager.LoadScene("Game");
            GameObject Room = new GameObject("NetworkRoomStats");
            _roomStats = Room.AddComponent<NetworkRoomStats>();
            _roomStats.PlayerJoined += OnPlayerJoined;
            _roomStats.PlayerLeft += OnPlayerLeft;
            JoinLobby(_roomStats);
        }

        private void OnDestroy()
        {
            if(NetworkRoomStats.Instance != null)
            {
                NetworkRoomStats.Instance.PlayerJoined -= OnPlayerJoined;
                NetworkRoomStats.Instance.PlayerLeft -= OnPlayerLeft;
            }
        }

        private async void JoinLobby(NetworkRoomStats room)
        {
            _loadingScreen.gameObject.SetActive(true);
            await room.Runner.JoinSessionLobby(SessionLobby.Custom, lobbyID: "MainLobby", authentication: new Fusion.Photon.Realtime.AuthenticationValues(Nickname));

            _waitingForPlayersForm.SetActive(true);
            _roomStatusText.text = string.Format("{0}/{1}", 0, PlayersPerMatch);
        }
        private void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                _lobbyStats = runner.Spawn(_lobbyStatsPrefab).GetComponent<LobbyStats>();
                var newPlayer = runner.Spawn(_playerPrefab, inputAuthority: player);
                Debug.Log("sending rpc call...");
                //RPC_PlayerEntitySpawned(player, player.PlayerId);
                //RPC_StaticCall(runner, player, newPlayer.Object);
            }
            else
            {
                _lobbyStats = FindObjectOfType<LobbyStats>();
            }
            if (player == runner.LocalPlayer)//if spawning self player
            {
                _loadingScreen.gameObject.SetActive(false);
                _playerReadyButton.onClick.AddListener(() =>
                {
                    if(_iAmReadyToPlay)
                    {
                        _iAmReadyToPlay = false;
                        _playerReadyButtonText.text = "Not ready";
                        _playerReadyButton.targetGraphic.color = Color.white;
                        RPC_PlayerIsNotReady();
                    }
                    else
                    {
                        _iAmReadyToPlay = true;
                        _playerReadyButtonText.text = "Ready";
                        _playerReadyButton.targetGraphic.color = _playerReadyButtonColor;
                        RPC_PlayerIsReady();
                    }
                });
            }
            Debug.Log("player joined (lobby)");
        }

        [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
        public  void RPC_PlayerEntitySpawned([RpcTarget] PlayerRef playerRef, int spawnedPlayerID)
        {
            Debug.Log(string.Format("Player {0} is spawned.", spawnedPlayerID));
            if (_roomStats.Runner.LocalPlayer == spawnedPlayerID)
            {
                var playerEntity = _roomStats.Runner.GetPlayerObject(spawnedPlayerID);
                if (playerEntity != null)
                    playerEntity.GetComponent<PlayerEntity>().Name = Nickname;
                else 
                    Debug.LogWarning("No player object registered!");
            }
        }
        [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
        public static void RPC_StaticCall(NetworkRunner runner, PlayerRef player, NetworkObject playerNetworkObject)
        {
            Debug.Log(string.Format("{0} is spawned.", player));
            runner.SetPlayerObject(player, playerNetworkObject);

            if (runner.LocalPlayer == player)
            {
                foreach (var p in runner.ActivePlayers)
                {
                    Debug.Log(string.Format("For player {0} body exist: {1}", p.PlayerId, runner.GetPlayerObject(p) != null));
                }
                var playerEntity = runner.GetPlayerObject(player);
                if (playerEntity != null)
                    playerEntity.GetComponent<PlayerEntity>().Name = PlayerPrefs.GetString("Nick");
                else
                    Debug.LogWarning("No player object registered!");
            }
        }


        [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
        public void RPC_PlayerIsReady()
        {
            if (_roomStats.Runner.IsServer)
                _lobbyStats.ReadyPlayersCount++;
            Debug.Log(string.Format("Some player is ready {0}/{1}", _lobbyStats.ReadyPlayersCount, _roomStats.Runner.SessionInfo.PlayerCount));
            TryStartGame();
        }
        [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
        public void RPC_PlayerIsNotReady()
        {
            if (_roomStats.Runner.IsServer)
                _lobbyStats.ReadyPlayersCount--;
            Debug.Log(string.Format("Some player is not ready {0}/{1}", _lobbyStats.ReadyPlayersCount, _roomStats.Runner.SessionInfo.PlayerCount));
            TryStartGame();
        }
        private void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                runner.Despawn(runner.GetPlayerObject(player));
                TryStartGame();
                Debug.Log((runner.SessionInfo.PlayerCount - 1)+ " players from " + runner.SessionInfo.MaxPlayers);
                if (runner.SessionInfo.PlayerCount - 1 < runner.SessionInfo.MaxPlayers)
                {
                    runner.SessionInfo.IsVisible = true;
                    Debug.Log("Session is not full");
                }
            }
        }
        private void TryStartGame()
        {
            int requiredPlayersCount = NetworkRoomStats.Instance.Runner.SessionInfo.MaxPlayers;
            _roomStatusText.text = string.Format("{0}/{1}", _lobbyStats.ReadyPlayersCount, requiredPlayersCount);

            if (_lobbyStats.ReadyPlayersCount >= requiredPlayersCount)
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