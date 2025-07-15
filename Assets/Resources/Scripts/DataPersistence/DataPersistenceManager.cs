using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    #region PROPERTIES
    public CharacterControllerBinding ControllerBinding;
    private GameData gamedata;
    public static DataPersistenceManager Instance { get; private set; }

    [ReadOnly] public List<IDataPersistence> DataPersistenceObjects = new List<IDataPersistence>();

    [Header("FILE HANDLER")]
    [SerializeField] private string fileName = "";
    [SerializeField] private bool useEncryption = false;
    private FileDataHandler fileDataHandler;
    #endregion

    #region UNITY CORE
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(ControllerBinding.SaveGameKey))
        {
            SaveGame();
        }
        if (Input.GetKeyDown(ControllerBinding.LoadGameKey))
        {
            LoadGame();
        }
        if (Input.GetKeyDown(ControllerBinding.ResetSaveFileKey))
        {
            NewGame();
            Application.Quit();
        }
#endif
    }

    //private void OnApplicationQuit()
    //{
    //    SaveGame();
    //}

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();

        Debug.Log("SCENE LOADED: " + scene.name);
    }

    public void OnSceneUnloaded(Scene scene)
    {
        SaveGame();
        Debug.Log("SCENE UNLOADED: " + scene.name);
    }
    #endregion

    #region MAIN
    public void NewGame()
    {
        this.gamedata = new GameData();
    }

    public void LoadGame()
    {
        this.gamedata = fileDataHandler.Load();

        if (this.gamedata == null) NewGame();
        foreach(var dataPersistenceObject in DataPersistenceObjects)
        {
            dataPersistenceObject.LoadData(this.gamedata);
        }

    }

    public void SaveGame()
    {
        foreach (var dataPersistenceObject in DataPersistenceObjects)
        {
            dataPersistenceObject.SaveData(ref this.gamedata);
        }

        fileDataHandler.Save(this.gamedata);
    }
    #endregion

    #region SUPPORTIVE
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        var dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IDataPersistence>();
        return dataPersistenceObjects.ToList();
    }

    public bool HasGameData() => this.gamedata != null;
    #endregion
}
