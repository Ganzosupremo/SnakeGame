using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using SnakeGame.Interfaces;

namespace SnakeGame.SaveAndLoadSystem
{
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
        #region Tooltip
        [Tooltip("The file name that will contain the game data.")]
        #endregion
        [SerializeField] private string fileName;
        #region Tooltip
        [Tooltip("True to use XOR encryption")]
        #endregion
        [SerializeField] private bool useEncryption;
        #region Tooltip
        [Tooltip("True if multiple savings are supported.")]
        #endregion
        [SerializeField] private bool supportsMultipleSaves;
        #region Tooltip
        [Tooltip("The name of folder where the game data will be saved, " +
            "if multiple saving is supported then this will profile ID won't be used.")]
        #endregion
        [SerializeField] private string profileID;

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
        private List<IPersistenceData> persistenceDataobjects;
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

            if (supportsMultipleSaves)
                this.selectedProfileID = fileDataHandler.GetMostRecentProfileID();
            else
                this.selectedProfileID = profileID;

            if (overrideProfileID)
            {
                this.selectedProfileID = testProfileID;
                Debug.LogWarning($"Overrided the selected profile ID with the test id: {testProfileID}");
            }
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

        private void OnApplicationQuit()
        {
            SaveGame();
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

        private void OnSceneUnloaded(Scene scene)
        {
            this.persistenceDataobjects = FindAllPersistenceObjects();

            if (autoSaveRoutine != null)
                StopCoroutine(autoSaveRoutine);
           
            SaveGame();
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

            // Load from the selected profile ID if multiple saves are supported
            if (supportsMultipleSaves)
                this.gameData = fileDataHandler.Load(selectedProfileID);
            else
                this.gameData = fileDataHandler.Load();

            // Just for debugging purposes, start a new game if the data is null
            if (this.gameData == null && initializeDataIfNull)
                NewGame();

            // If no data is found return right away
            if (this.gameData == null)
            {
                Debug.Log("No Data to load found. A New Game Needs to be started first");
                NewGame();
                return;
            }

            foreach (IPersistenceData persistenceData in persistenceDataobjects)
            {
                persistenceData.Load(gameData);
            }
        }

        /// <summary>
        /// Saves the data of the game, in order to save data the class calling this method needs to implement the ISaveData interface
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

            foreach (IPersistenceData persistenceData in persistenceDataobjects)
            {
                persistenceData.Save(gameData);
            }

            // Timestamp the game data so we know when was the last save
            gameData.lastUpdated = System.DateTime.Now.ToBinary();

            // Save with the profile ID if multiple saves are supported
            if (supportsMultipleSaves)
                fileDataHandler.Save(gameData, selectedProfileID);
            else
                fileDataHandler.Save(gameData);
        }

        /// <summary>
        /// Finds all the objects that implement the <seealso cref="IPersistenceData"/> interface.
        /// </summary>
        /// <returns>Returns a new list with all <seealso cref="IPersistenceData"/> objects found.</returns>
        private List<IPersistenceData> FindAllPersistenceObjects()
        {
            IEnumerable<IPersistenceData> persistenceDataObjects = FindObjectsOfType<MonoBehaviour>().OfType<IPersistenceData>();

            return new List<IPersistenceData>(persistenceDataObjects);
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
        private IEnumerator DisplayMessage(string message, float timeToDisplay)
        {
            messageToDisplay.gameObject.SetActive(true);
            messageToDisplay.text = message;

            yield return new WaitForSeconds(timeToDisplay);

            messageToDisplay.text = "";
            messageToDisplay.gameObject.SetActive(false);
        }
    }
}