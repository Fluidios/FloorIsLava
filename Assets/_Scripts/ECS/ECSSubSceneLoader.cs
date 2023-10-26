using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.ECS
{
    public class ECSSubSceneLoader : MonoBehaviour
    {
        private void OnEnable()
        {
            StartCoroutine(LoadECSScene());
        }

        IEnumerator LoadECSScene()
        {

            var process = SceneManager.LoadSceneAsync("MainECS", LoadSceneMode.Additive);
            yield return new WaitWhile(()=> !process.isDone);
            Debug.Log("ECS scene loading - done. Active scene - " + SceneManager.GetActiveScene().name);
        }
    }
}
