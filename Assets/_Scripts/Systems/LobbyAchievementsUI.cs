using Game.FirebaseHandler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Systems
{
    public class LobbyAchievementsUI : MonoBehaviour
    {
        [SerializeField] private Button _showAchievementsViewModel;
        private void Awake()
        {
            _showAchievementsViewModel.onClick.AddListener(LoadAchievements);
        }
        public async void LoadAchievements()
        {
            string uid = UserAuth.UserId;
            var checkAchievementsExistance = DatabaseHandler.UserAchievemntsDataExists(uid);
            await checkAchievementsExistance;
            if (checkAchievementsExistance.Result)
            {
                var loadAchievements = DatabaseHandler.LoadUserAchievementsMask(uid);
                await loadAchievements;
                if(loadAchievements.Result != null)
                {
                    PrintAchievementsMask(loadAchievements.Result);
                }
            }
            else
            {
                var emptyAchievements = Achievements.GetEmptyPlayerAchievementsMask();
                Debug.Log("Empty achievements mask pushed to database.");
                DatabaseHandler.SyncAchievement(uid, emptyAchievements);
            }
        }
        private void PrintAchievementsMask(Achievements.AchivementsMask mask)
        {
            foreach (var item in mask.Mask)
            {
                Debug.LogFormat("{0}: progress = {1}%", item.Key, item.Value * 100);
            }
        }
    }
}
