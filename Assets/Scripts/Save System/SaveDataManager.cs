using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveDataManager : SingletonMonoBehaviour<SaveDataManager>
{
    #region Header Debugging
    [Header("Debugging")]
    #endregion

    #region Tooltip
    [Tooltip("Initialize the data when there's no data, i.e. When the Persistence Manager is in another scene that is not the main menu, use only when developing.")]
    #endregion
    [SerializeField] private bool initializeDataIfNull = false;

    #region Tooltip
    [Tooltip("Disable the Persistence Data Manager, use only when developing, don't include in the final build.")]
    #endregion
    [SerializeField] private bool disablePersistenceManager = false;

    #region Tooltip
    [Tooltip("Check if you want the profile ID to be overriden, just use when developing, don't include in the final build.")]
    #endregion
    [SerializeField] private bool overrideProfileID = false;

    #region Tooltip
    [Tooltip("Write the text that will be used to override the profile ID, use only if the above field is set to true.")]
    #endregion
    [SerializeField] private string testProfileID = "";

    #region Header File Storage Configuration
    [Space(10)]
    [Header("File Storage Settings")]
    #endregion
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    #region Header Auto Save Settings
    [Header("Auto Save Settings")]
    #endregion
    [SerializeField] private int autoSaveTimeSeconds = 5400;

    #region User Feedback
    [Header("User Feedback")]
    [Tooltip("The message that will be displayed in the UI")]
    #endregion
    [SerializeField] private TextMeshProUGUI messageToDisplay;


    private GameData gameData;
    private List<ISaveData> persistenceDataobjects;
    private FileDataHandler fileDataHandler;
    private string selectedProfileID = "";
    private Coroutine autoSaveRoutine;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this.gameObject);

        if (disablePersistenceManager)
        {
            Debug.LogWarning("The persistence data manager is currently disabled");
        }

        this.fileDataHandler = new(Application.persistentDataPath, fileName, useEncryption);

        this.selectedProfileID = fileDataHandler.GetMostRecentProfileID();

        if (overrideProfileID)
        {
            this.selectedProfileID = testProfileID;
            Debug.LogWarning($"Overrided the selected profile ID with the test id: {testProfileID}");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.persistenceDataobjects = FindAllPersistenceObjects();
        LoadGame();

        // Start the auto save
        if (autoSaveRoutine != null)
            StopCoroutine(autoSaveRoutine);
        autoSaveRoutine = StartCoroutine(AutoSave());
    }

    /// <summary>
    /// Update the profile used for loading and saving
    /// </summary>
    /// <param name="newProfileID">The new profile ID used for loading and saving</param>
    public void ChangeSelectedProfileID(string newProfileID)
    {
        this.selectedProfileID = newProfileID;
        // Load the game, which will use the new profile ID, updating the game accordingly
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new();
    }

    /// <summary>
    /// Loads the data that has been saved
    /// </summary>
    public void LoadGame()
    {
        if (disablePersistenceManager) return;

        // Load any saved data from a Json file using the File Data Handler
        this.gameData = fileDataHandler.Load(selectedProfileID);

        // Just for debugging purposes, start a new game if the data is null
        if (this.gameData == null && initializeDataIfNull)
        {
            NewGame();
        }

        // If no data is found return right away
        if (this.gameData == null)
        {
            Debug.Log("No Data to load found. A New Game Needs to be started first");
            NewGame();
            return;
        }

        foreach (ISaveData persistenceData in persistenceDataobjects)
        {
            persistenceData.LoadData(gameData);
        }
    }

    /// <summary>
    /// Saves the data of the game, in order to save data the class needs to implement the IPersistenceData interface
    /// </summary>
    public void SaveGame()
    {
        if (disablePersistenceManager) return;

        // If we don't have any data to save, log a warning
        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. A new game needs to be started before data can be saved");
            return;
        }

        foreach (ISaveData persistenceData in persistenceDataobjects)
        {
            persistenceData.Save(gameData);
        }

        // Timestamp the game data so we know when was the last save
        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        // Save the data to a Json file using the File Data Handler
        fileDataHandler.Save(gameData, selectedProfileID);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    /// <summary>
    /// Finds all the objects that implement the <see cref="IPersistenceData"/> interface.
    /// </summary>
    /// <returns>Returns a new list with all <see cref="IPersistenceData"/> objects found.</returns>
    private List<ISaveData> FindAllPersistenceObjects()
    {
        IEnumerable<ISaveData> persistenceDataObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveData>();

        return new List<ISaveData>(persistenceDataObjects);
    }

    /// <summary>
    /// Check if the game has game data
    /// </summary>
    /// <returns>Returns true if it's not null, false otherwise</returns>
    public bool HasGameData()
    {
        return gameData != null;
    }

    /// <summary>
    /// Gets all the save profiles at once, and they can be displayed in the UI
    /// </summary>
    /// <returns>All the profiles of the game data as part of a dictionary</returns>
    public Dictionary<string, GameData> GetAllProfileGameData()
    {
        return fileDataHandler.LoadAllSaveProfiles();
    }

    /// <summary>
    /// Autosave the game with a timer
    /// </summary>
    private IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveTimeSeconds);
            SaveGame();
            string newMessage = String.Format("The game has been autosaved at {0:HH:mm:ss}!", DateTime.Now);
            StartCoroutine(DisplayMessage(newMessage, 3f));
        }
    }

    /// <summary>
    /// Display a message to the UI
    /// </summary>
    /// <param name="message"></param>
    /// <param name="timeToDisplay"></param>
    /// <returns></returns>
    private IEnumerator DisplayMessage(string message, float timeToDisplay)
    {
        messageToDisplay.gameObject.SetActive(true);
        messageToDisplay.text = message;

        yield return new WaitForSeconds(timeToDisplay);

        messageToDisplay.text = "";
        messageToDisplay.gameObject.SetActive(false);
    }
}
