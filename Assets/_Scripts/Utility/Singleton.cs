using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Utility
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;
        public static T Instance
        {
            get 
            { 
                if(_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if(_instance == null)
                        Debug.LogError("CANT FIND INSTANCE OF " + typeof(T).Name);
                }
                return _instance; 
            }
        }

        private void Awake()
        {
            if(_instance == null)
            {
                _instance = (T)this;
            }
            else
            {
                if (_instance != this)
                {
                    Debug.Log(string.Format("FOUND SECOND INSTANCE OF {0}, BUT THIS CLASS SHOULD BE SINGLETON!", typeof(T).Name));
                    Destroy(this);
                }
            }
        }
    }
}
