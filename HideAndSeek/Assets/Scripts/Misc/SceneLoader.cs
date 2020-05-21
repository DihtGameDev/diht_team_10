using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader {
    private static object LOCK = new object();

    private static string currentSceneName = "Launcher"; // default scene name

    public static void LoadSceneOnce(string sceneName) {
        lock(LOCK) {
            if (SceneManager.GetSceneByName(sceneName).isLoaded == false && currentSceneName != sceneName) {
                currentSceneName = sceneName;
                SceneManager.LoadScene(sceneName);
            } else {
                Debugger.Log($"Scene {sceneName} already loaded");
            }
        }
    }
}
