using SnakeGame.SoundsSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SnakeGame.UI
{
    public class MainMenuUI : SingletonMonoBehaviour<MainMenuUI>
    {
        [Header("Button References")]
        [Space(5)]
        [SerializeField] private GameObject playButton;
        [SerializeField] private GameObject returnButton;
        [SerializeField] private GameObject settingsButton;
        [SerializeField] private GameObject instructionsButton;
        [SerializeField] private GameObject highScoresButton;
        [SerializeField] private GameObject quitButton;

        [Header("Audio References")]
        [Space(5)]
        [SerializeField] private MusicSO mainMenuMusic;

        private bool isSettingsLoaded = false;
        private bool isHighScoresLoaded = false;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            returnButton.SetActive(false);
            MusicManager.Instance.PlayMusic(mainMenuMusic);
        }

        public void StartGame()
        {
            SceneManager.LoadScene((int)SceneIndex.MainGame);
        }

        public void LoadSettings()
        {
            playButton.SetActive(false);
            settingsButton.SetActive(false);
            instructionsButton.SetActive(false);
            highScoresButton.SetActive(false);
            quitButton.SetActive(false);

            returnButton.SetActive(false);

            isSettingsLoaded = true;
            ReturnButtonUI.sceneToUnload = ReturnButtonUI.SceneToUnload.Settings;
            SceneManager.LoadScene((int)SceneIndex.Settings, LoadSceneMode.Additive);
        }

        public void LoadHighScores()
        {
            playButton.SetActive(false);
            settingsButton.SetActive(false);
            instructionsButton.SetActive(false);
            highScoresButton.SetActive(false);
            returnButton.SetActive(false);
            
            quitButton.SetActive(true);

            isHighScoresLoaded = true;
            ReturnButtonUI.sceneToUnload = ReturnButtonUI.SceneToUnload.HighScores;
            SceneManager.LoadScene((int)SceneIndex.HighScores, LoadSceneMode.Additive);
        }

        /// <summary>
        /// Returns to the main Menu unloading the scene that was open additively.
        /// Recomended to use on the buttons that are on the Main Menu Scene
        /// </summary>
        public void LoadMainMenu()
        {
            if (isSettingsLoaded)
            {
                SceneManager.UnloadSceneAsync((int)SceneIndex.Settings);
                isSettingsLoaded = false;
            }
            else if (isHighScoresLoaded)
            {
                SceneManager.UnloadSceneAsync((int)SceneIndex.HighScores);
                isHighScoresLoaded = false;
            }

            returnButton.SetActive(false);
            playButton.SetActive(true);
            settingsButton.SetActive(true);
            instructionsButton.SetActive(true);
            highScoresButton.SetActive(true);
            quitButton.SetActive(true);
        }

        /// <summary>
        /// Returns to the main menu without unloading the scene that was opened additively.
        /// Recomended to use on the buttons or gameobjects that are outside the Main Menu Scene.
        /// </summary>
        public void ReturnButtonMainMenu()
        {
            returnButton.SetActive(false);
            playButton.SetActive(true);
            settingsButton.SetActive(true);
            instructionsButton.SetActive(true);
            highScoresButton.SetActive(true);
            quitButton.SetActive(true);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

    }
}
