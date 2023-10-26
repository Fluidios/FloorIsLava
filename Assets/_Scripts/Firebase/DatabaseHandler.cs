using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using System.Threading.Tasks;

namespace Game.FirebaseHandler
{
    public static class DatabaseHandler
    {
        private const string c_achievementsDataBlock = "achievements";
        private static DatabaseReference _databaseReference;
        private static DatabaseReference DB
        {
            get
            {
                //chekout database reference
                if (_databaseReference == null)
                {
                    _databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                }
                return _databaseReference;
            }
        }

        public static async Task<bool> UserAchievemntsDataExists(string userId)
        {
            // Check if data exists for the given user ID
            var checkerTask = DB.Child("achievements").Child(userId).GetValueAsync();
            await checkerTask;

            if (checkerTask.IsFaulted)
            {
                Debug.LogError("Failed to check if data exists: " + checkerTask.Exception.Message);
                return false;
            }
            else if (checkerTask.IsCompleted)
            {
                DataSnapshot snapshot = checkerTask.Result;
                bool exists = snapshot.Exists;
                return exists;
            }
            return false;
        }

        public static async Task<Achievements.AchivementsMask> LoadUserAchievementsMask(string userID)
        {
            // Load all achievements for the given user ID
            var loadTask = DB.Child(c_achievementsDataBlock).Child(userID).GetValueAsync();
            await loadTask;

            if (loadTask.IsFaulted)
            {
                Debug.LogError("Failed to load achievements: " + loadTask.Exception.Message);
            }
            else if (loadTask.IsCompleted)
            {
                var mask = Achievements.GetEmptyPlayerAchievementsMask();
                DataSnapshot snapshot = loadTask.Result;
                foreach (DataSnapshot childSnapshot in snapshot.Children)
                {
                    foreach (var item in childSnapshot.Children)
                    {
                        if(float.TryParse(item.Value.ToString(), out float achievementProgress))
                        {
                            if (mask.Mask.ContainsKey(item.Key))
                            {
                                mask.Mask[item.Key] = achievementProgress;
                            }
                            else
                                Debug.LogWarningFormat("Unexpected achievement \"{0}\" found in database!", item.Key);
                        }
                        else
                        {
                            Debug.LogWarningFormat("Failed to parse {0}", item.Value);
                        }
                    }
                }
                return mask;
            }
            return null;
        }
        public static void SyncAchievement(string userID, Achievements.AchivementsMask achievementMask)
        {
            // Push the achievement object to the database
            string key = DB.Child(c_achievementsDataBlock).Child(userID).Push().Key;
            DB.Child(c_achievementsDataBlock).Child(userID).Child(key).SetValueAsync(achievementMask.Mask).ContinueWith((task) => {
                if (task.IsCanceled)
                {
                    Debug.LogWarningFormat("Achievements pushing into database was canceled. Error: {0}",  task.Exception.Message);
                }
                else if(task.IsFaulted)
                {
                    Debug.LogWarningFormat("Failed to push achievements into database. Error: {0}", task.Exception.Message);
                }
                else
                {
                    Debug.LogFormat("Achievements - successfully pushed into database.");
                }
            });
        }
    }
}
