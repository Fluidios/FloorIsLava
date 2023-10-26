using Game.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Systems
{
    public class ScenesManager : Singleton<ScenesManager>
    {
        [SerializeField] private CanvasGroup _loadingScreen;
        [SerializeField] private Image _loadingProgressBar;
        private void OnEnable()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        public async Task ChangeScene(string from, string to)
        {
            await ShowLoadingScreen();
            float currentProgress = 0;
            int totalOperations = 3;
            await LoadSceneAdditeve(to, currentProgress, totalOperations);
            currentProgress += 1f / totalOperations;
            await UnloadScene(from, currentProgress, totalOperations);
            await HideLoadingScreen();
        }
        private async void InitializeGame()
        {
            if(_loadingScreen != null)
                await ShowLoadingScreen();
            float currentProgress = 0;
            int totalOperations = 3;
            await LoadSceneAdditeve("MainECS", currentProgress, totalOperations);
            currentProgress += 1f / totalOperations;
            await LoadSceneAdditeve("Lobby", currentProgress, totalOperations);
            currentProgress += 1f / totalOperations;
            await UnloadScene("SDK", currentProgress, totalOperations);
            if (_loadingScreen != null)
                await HideLoadingScreen();
        }
        private async Task LoadSceneAdditeve(string sceneName, float currentProgress, int totalOperationsCount)
        {
            var process = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!process.isDone)
            {
                if (_loadingScreen != null)
                    _loadingProgressBar.fillAmount = currentProgress + process.progress/totalOperationsCount;
                await Task.Delay(1);
            }
            Debug.LogFormat("{0} scene loading - done. Active scene - {1}", sceneName, SceneManager.GetActiveScene().name);
        }
        private async Task UnloadScene(string sceneName, float currentProgress, int totalOperationsCount)
        {
            var process = SceneManager.UnloadSceneAsync(sceneName);
            while (!process.isDone)
            {
                if (_loadingScreen != null)
                    _loadingProgressBar.fillAmount = currentProgress + process.progress / totalOperationsCount;
                await Task.Delay(1);
            }
            Debug.LogFormat("{0} scene unloading - done. Active scene - {1}", sceneName, SceneManager.GetActiveScene().name);
        }

        private async Task ShowLoadingScreen()
        {
            _loadingScreen.gameObject.SetActive(true);
            for (float t = 0.0f; t <= 1.0f; t += 0.01f)
            {
                _loadingScreen.alpha = t;
                await Task.Delay(1);
            }
        }
        private async Task HideLoadingScreen()
        {
            for (float t = 0.0f; t <= 1.0f; t += 0.01f)
            {
                _loadingScreen.alpha = 1 - t;
                await Task.Delay(1);
            }
            _loadingScreen.gameObject.SetActive(false);
        }
    }
}
