using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapsManager : MonoBehaviour
{
    #region PROPERTIES
    public string GameplaySceneName;
    public string MapSceneName;
    #endregion

    #region UNITY CORE
    private void Awake()
    {
        StartCoroutine(LoadSceneFlow());
    }

    #endregion

    #region MAIN
    private IEnumerator LoadSceneFlow()
    {
        yield return SceneManager.LoadSceneAsync(MapSceneName, LoadSceneMode.Additive);
        yield return SceneManager.LoadSceneAsync(GameplaySceneName, LoadSceneMode.Additive);
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }
    #endregion
}
