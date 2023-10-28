using Game.FirebaseHandler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Lobby
{
    public partial class LobbyAchievementsUI : MonoBehaviour
    {
        [SerializeField] private LobbyAuthUI _auth;
        [SerializeField] private Button _showAchievementsForm;
        [SerializeField] private GameObject _achievementsForm;
        [SerializeField] private AchievementView _achievementViewForm;
        [SerializeField] private AchievementsDict _achievementsDictionary;
        private void Awake()
        {
            _auth.AuthPassed += LoadAchievements;

            _showAchievementsForm.onClick.AddListener(() => _achievementsForm.SetActive(!_achievementsForm.activeSelf));
        }
        public async void LoadAchievements()
        {
            string uid = UserAuth.UserId;
            var checkAchievementsExistance = DatabaseHandler.UserAchievemntsDataExists(uid);
            await checkAchievementsExistance;
            if (checkAchievementsExistance.Result)
            {
                var loadAchievements = DatabaseHandler.LoadUserAchievementsMask(uid, GetEmptyAchievementsMask().Mask);
                await loadAchievements;
                if(loadAchievements.Result != null)
                {
                    SetupUI(loadAchievements.Result);
                }
            }
            else
            {
                var emptyAchievements = GetEmptyAchievementsMask();
                Debug.Log("Empty achievements mask pushed to database.");
                DatabaseHandler.SyncAchievement(uid, emptyAchievements);
                SetupUI(emptyAchievements);
            }
        }
        private void SetupUI(AchivementsMask mask)
        {
            AchievementView view;
            AchievementVisualisationData data;
            foreach (var item in mask.Mask)
            {
                if (_achievementsDictionary.ContainsKey(item.Key))
                {
                    data = _achievementsDictionary[item.Key];
                    view = Instantiate(_achievementViewForm, _achievementViewForm.transform.parent);
                    view.UpdateAchievementView(name: item.Key, data.Description, item.Value, data.Icon);
                    view.gameObject.SetActive(true);
                }
            }
        }

        private AchivementsMask GetEmptyAchievementsMask()
        {
            var mask = new Dictionary<string, float>();

            foreach (var item in _achievementsDictionary)
            {
                mask.Add(item.Key, 0);
            }

            return new AchivementsMask(mask);
        }
    }
}
