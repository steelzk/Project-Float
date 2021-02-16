using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviour
{
    private void Awake()
    {
        // load main menu scene
        StartCoroutine(LoadOperation("menu_0", LoadSceneMode.Additive));
    }

    private IEnumerator LoadOperation(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(sceneName, mode);

        if (sceneLoad.isDone)
        {
            Debug.Log("[Launcher] Loaded Start Scene!");
            Destroy(this);
            yield return new WaitForEndOfFrame();
        }
    }
}
