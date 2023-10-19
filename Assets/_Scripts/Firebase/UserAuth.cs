using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.FirebaseHandler
{
    public class UserAuth : MonoBehaviour
    {
        public static string LocalEmail
        {
            get { return PlayerPrefs.GetString("Email"); }
            set { PlayerPrefs.SetString("Email", value); }
        }
        public static string LocalPassword
        {
            get { return PlayerPrefs.GetString("Password"); }
            set { PlayerPrefs.SetString("Password", value); }
        }
        public static string UserNickName
        {
            get
            {
                var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                if(auth != null && auth.CurrentUser != null)
                    return auth.CurrentUser.DisplayName;
                return "UNKNOWN";
            }
        }
        public static async Task<bool> CreateUser(string email, string password, string nickname)
        {
            var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            var authTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            await authTask;
            if (authTask.IsCompleted)
            {
                Debug.LogWarning("Update Nick!!!!");
                return true;
            }
            else if (authTask.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
            }
            else if(authTask.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + authTask.Exception);
            }
            return false;
        }
        public static async Task<bool> TrySignIn(string email, string password)
        {
            var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            var authTask = auth.SignInWithEmailAndPasswordAsync(email, password);
            await authTask;
            if (authTask.IsCompleted)
            {
                Debug.Log("Signed In");
                return true;
            }
            else if (authTask.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
            }
            else if (authTask.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + authTask.Exception);
            }
            return false;
        }
    }
}
