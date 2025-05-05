#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneTool : EditorWindow
{
    #region PROPERTIES
    public static string BoostrapName = "BootstrapScene";
    public static string MapSceneName = "Boston_Hell";
    public static string GameplaySceneName = "Gameplay";
    #endregion

    #region MAIN
    [MenuItem("VTools/Load Scene Tool")]
    public static void ShowWindow()
    {
        GetWindow<LoadSceneTool>("Load Scene Tool");
    }

    private void OnGUI()
    {
        var headerStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
        GUILayout.Label("--- SCENE MANAGER ---", headerStyle);
        GUILayout.Label("____________", headerStyle);
        EditorGUILayout.Space();

        BoostrapName = EditorGUILayout.TextField("Bootstrap Scene Name", BoostrapName);
        MapSceneName = EditorGUILayout.TextField("Map Scene Name", MapSceneName);
        GameplaySceneName = EditorGUILayout.TextField("Gameplay Scene Name", GameplaySceneName);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (GUILayout.Button("Load Bootstrap Scene"))
        {
            EditorSceneManager.OpenScene("Assets/Scenes/" + BoostrapName + ".unity");
        }

        if (GUILayout.Button("Load Map Scene"))
        {
            EditorSceneManager.OpenScene("Assets/Scenes/" + MapSceneName + ".unity");
        }

        if (GUILayout.Button("Load Gameplay Scene"))
        {
            EditorSceneManager.OpenScene("Assets/Scenes/" + GameplaySceneName + ".unity");
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("PLAY"))
        {
            EditorSceneManager.OpenScene("Assets/Scenes/" + BoostrapName + ".unity");
            EditorApplication.isPlaying = true;
        }
    }
    #endregion
}
#endif