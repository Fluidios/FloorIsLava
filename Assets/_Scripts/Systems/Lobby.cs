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
            var roomStats = Room.AddComponent<NetworkRoomStats>();
            roomStats.PlayerJoined += OnPlayerJoined;
            roomStats.PlayerLeft += OnPlayerLeft;
            JoinLobby(roomStats);
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
            _loadingScreen.gameObject.SetActive(false);

            _waitingForPlayersForm.SetActive(true);
            _roomStatusText.text = string.Format("{0}/{1}", 0, PlayersPerMatch);
        }

        private void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {

            if (runner.IsServer)
            {
                var newPlayer = runner.Spawn(_playerPrefab, inputAuthority: player);
                newPlayer.Name = player.PlayerId.ToString();
                runner.SetPlayerObject(player, newPlayer.Object);
                newPlayer.ReadyToPlay.Subscribe(TryStartGame);
                TryStartGame(false);

                //TODO: Working only for server, but not for client!
                if (player == runner.LocalPlayer)//if spawning self player
                {
                    _playerReadyButton.onClick.AddListener(() =>
                    {
                        if (NetworkRoomStats.Instance.Runner.TryGetPlayerObject(player, out var networkObject))
                        {
                            var playerEntity = networkObject.GetComponent<PlayerEntity>();
                            if (playerEntity.ReadyToPlay.Value)
                            {
                                _playerReadyButton.targetGraphic.color = Color.white;
                                _playerReadyButtonText.text = "Not Ready";
                                playerEntity.ReadyToPlay.Value = false;
                            }
                            else
                            {
                                _playerReadyButton.targetGraphic.color = _playerReadyButtonColor;
                                _playerReadyButtonText.text = "Ready";
                                playerEntity.ReadyToPlay.Value = true;
                            }
                        }
                    });
                }
            }
        }
        private void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            runner.GetPlayerObject(player).GetBehaviour<PlayerEntity>().ReadyToPlay.UnSubscribe(TryStartGame);
            if (runner.IsServer)
            {
                runner.Despawn(runner.GetPlayerObject(player));
                TryStartGame(false);
                Debug.Log(runner.SessionInfo.PlayerCount + " :  " + runner.SessionInfo.MaxPlayers);
                if (runner.SessionInfo.PlayerCount < runner.SessionInfo.MaxPlayers)
                {
                    runner.SessionInfo.IsVisible = true;
                    Debug.Log("Session is not full");
                }
            }
        }
        private void TryStartGame(bool v)
        {
            int readyPlayersCount = 0;
            var runner = NetworkRoomStats.Instance.Runner;
            foreach (var player in runner.ActivePlayers) 
            {
                if (runner.TryGetPlayerObject(player, out var networkObject))
                {
                    if (networkObject.GetBehaviour<PlayerEntity>().ReadyToPlay.Value)
                        readyPlayersCount++;
                    Debug.Log("For player " + player.PlayerId + " NObject - exist");
                }
                else
                {
                    Debug.Log("For player " + player.PlayerId + " NObject - Not exist");
                }
            }
            int requiredPlayersCount = NetworkRoomStats.Instance.Runner.SessionInfo.MaxPlayers;
            _roomStatusText.text = string.Format("{0}/{1}", readyPlayersCount, requiredPlayersCount);

            if (readyPlayersCount >= requiredPlayersCount)
                StartCoroutine(LoadScene());
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