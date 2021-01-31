﻿using UnityEditor;
using UnityEditor.SceneManagement;

public static class SceneMenu
{
    [MenuItem("Scenes/Menu")]
    public static void OpenMenu()
    {
        OpenScene("Menu");
    }

    [MenuItem("Scenes/Beach")]
    public static void OpenTutorial()
    {
        OpenScene("Beach");
    }

    private static void OpenScene(string sceneName)
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Persistent.unity", OpenSceneMode.Single);
        EditorSceneManager.OpenScene("Assets/Scenes/" + sceneName + ".unity", OpenSceneMode.Additive);
    }
}