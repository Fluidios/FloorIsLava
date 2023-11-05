using Fusion;
using Game.ECS;
using Game.FirebaseHandler;
using Game.Lobby;
using Game.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameResultsUI : NetworkBehaviour
    {
        [SerializeField] private GameObject _gameEndsForm;
        [SerializeField] private PlayerResultUI _playerResultUiPrefab;
        [SerializeField] private Button _returnButton;
        [SerializeField] private int _requiredStarsForAchievement = 5;

        public override void Spawned()
        {
            base.Spawned();
            Game.ECS.GameEndsObserverSystem.GameEnds += OnGameEnds;
            _returnButton.onClick.AddListener(GoToMenu);
        }
        private void OnDestroy()
        {
            Game.ECS.GameEndsObserverSystem.GameEnds -= OnGameEnds;
        }
        private void OnGameEnds()
        {
            Game.ECS.GameEndsObserverSystem.GameEnds -= OnGameEnds;
            RPC_BroadcastGameEndsEventToClients(CollectedStarsCounterSystem.CurrentResults);
        }
        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        private void RPC_BroadcastGameEndsEventToClients(Vector2Int[] results)
        {
            _gameEndsForm.SetActive(true);
            List<PlayerScore> scoreList = new List<PlayerScore>();
            foreach (var item in Runner.ActivePlayers)
            {
                GetPlayerScore(item, results, out string nickname, out int score);
                if (item == Runner.LocalPlayer) UpdateStarAchievementState(score);
                var view = Instantiate(_playerResultUiPrefab, _playerResultUiPrefab.transform.parent);
                view.Set(nickname, score);
                view.gameObject.SetActive(true);
                scoreList.Add(new PlayerScore() { Score = score, ScoreView = view.transform });
            }
            scoreList = scoreList.OrderBy(o => o.Score).Reverse().ToList();
            for (int i = 0; i < scoreList.Count; i++)
            {
                scoreList[i].ScoreView.SetSiblingIndex(i);
            }
        }

        private void UpdateStarAchievementState(int score)
        {
            if (score >= _requiredStarsForAchievement)
            {
                DatabaseHandler.SyncAchievement(UserAuth.UserId, "Super Star", 1);
                Debug.Log("Achievement Super Star obtained!");
            }
            else
            {
                Debug.Log("Achievement Super Star not obtained, not enough stars collected. Stars collected: " + score);
            }
        }

        private void GetPlayerScore(PlayerRef searchFor, Vector2Int[] resultsProvidedByServer, out string playerNickname, out int score)
        {
            if (Runner.TryGetPlayerObject(searchFor, out var playerNO))
            {
                playerNickname = playerNO.GetComponent<Nickname>().PlayerNickname;
                foreach (var item in resultsProvidedByServer)
                {
                    if(item.x == searchFor)
                    {
                        score = item.y;
                        return;
                    }
                }
                score = 0;
            }
            else
            {
                playerNickname = "ERROR";
                score = 0;
                Debug.LogError(string.Format("For player with Id = {0}, no body found!", searchFor));
            }
        }
        private async void GoToMenu()
        {
            var runner = FindObjectOfType<NetworkRunner>();
            if (runner != null)
            {
                //disconnect self
                await runner.Shutdown(true, ShutdownReason.Ok);
            }
            await ScenesManager.Instance.ChangeScene("Game", "Lobby");
        }
        struct PlayerScore
        {
            public int Score;
            public Transform ScoreView;
        }
    }
}
