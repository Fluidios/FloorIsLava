using Firebase;
using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.FirebaseHandler
{
    public static class UserAuth
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
        public static string UserId
        {
            get
            {
                var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                if (auth != null && auth.CurrentUser != null)
                    return auth.CurrentUser.UserId;
                return "UNKNOWN";
            }
        }
        public static async Task<AuthError> CreateUser(string email, string password, string nickname)
        {
            var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            var authTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            AuthError error = AuthError.Failure;
            await authTask.ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    LocalEmail = authTask.Result.User.Email;
                    LocalPassword = password;
                    error = AuthError.None;
                }
                else
                {
                    if (task.Exception != null && task.Exception.InnerExceptions[0] != null)
                    {
                        FirebaseException firebaseException = authTask.Exception.InnerExceptions[0] as FirebaseException;
                        error = (AuthError)firebaseException.ErrorCode;
                        Debug.LogWarning("Firebase error: " + error);
                    }
                    else
                    {
                        Debug.LogWarning("Unknown error: " + task.Exception.Message);
                        error = AuthError.Failure;
                    }
                }
            });
            if (authTask.IsCompleted)
            {
                await UpdateNickname(nickname);
            }
            return error;
        }
        public static async Task<AuthError> TrySignIn(string email, string password)
        {
            var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            var authTask = auth.SignInWithEmailAndPasswordAsync(email, password);
            AuthError error = AuthError.Failure;
            try
            {
                await authTask;
                if (authTask.IsCompleted)
                {
                    Debug.Log("Signed In");
                    LocalEmail = authTask.Result.User.Email;
                    LocalPassword = password;
                    error = AuthError.None;
                    return error;
                }
            }
            catch(Exception e)
            {
                if(authTask.Exception != null && authTask.Exception.InnerExceptions[0] != null)
                {
                    FirebaseException firebaseException = authTask.Exception.InnerExceptions[0] as FirebaseException;
                    error = (AuthError)firebaseException.ErrorCode;
                    Debug.LogWarning("Firebase error: " + error);
                }
                else
                {
                    error = AuthError.Failure;
                    Debug.LogWarning("Unknown error: " + e.Message);
                }
            }
            return error;
        }

        public static async Task<bool> UpdateNickname(string nickname)
        {
            Firebase.Auth.FirebaseUser user = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser;
            if (user != null)
            {
                Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
                {
                    DisplayName = nickname,
                };
                var updateTask = user.UpdateUserProfileAsync(profile);
                await updateTask;
                if (updateTask.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return false;
                }
                if (updateTask.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + updateTask.Exception);
                    return false;
                }
                Debug.Log("User profile updated successfully.");
                return true;
            }
            return false;
        }
    }
}
