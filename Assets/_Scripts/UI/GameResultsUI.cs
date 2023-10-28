using Fusion;
using Game.ECS;
using Game.Lobby;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameResultsUI : MonoBehaviour
    {
        [SerializeField] private GameObject _gameEndsForm;
        [SerializeField] private Button _returnToMenu;
        private void Awake()
        {
            Game.ECS.GameEndsObserverSystem.GameEnds += OnGameEnds;
            _returnToMenu.onClick.AddListener(GoToMenu);
        }
        private void OnDestroy()
        {
            Game.ECS.GameEndsObserverSystem.GameEnds -= OnGameEnds;
        }

        private void OnGameEnds()
        {
            Game.ECS.GameEndsObserverSystem.GameEnds -= OnGameEnds;
            _gameEndsForm.SetActive(true);
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
    }
}
