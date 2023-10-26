using Firebase.AppCheck;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.FirebaseHandler
{
    public class AppCheck : MonoBehaviour
    {
        private void Awake()
        {
            //init app check system
#if UNITY_EDITOR
            Debug.Log("Fake security check. Real security check would work only on Android build.");
#elif UNITY_ANDROID
            FirebaseAppCheck.SetAppCheckProviderFactory(PlayIntegrityProviderFactory.Instance);
#else
            Debug.LogError("This app is not supporting IOS platform!");
#endif
        }
    }
}
