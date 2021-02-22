using UnityEngine;
using UnityEngine.SceneManagement;

public static class SSceneManager 
{
    public const string MenuScene = "menu_0";
    public const string ClientScene = "client_0";

    public static void GoMainMenu()
    { 
        SceneManager.LoadScene(MenuScene, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(ClientScene);
    }

    public static void GoClientScene()
    {
        SceneManager.LoadScene(ClientScene, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(MenuScene);
    }
}
