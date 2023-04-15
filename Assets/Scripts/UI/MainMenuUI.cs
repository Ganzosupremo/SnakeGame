using SnakeGame.AudioSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SnakeGame.UI
{
    public class MainMenuUI : SingletonMonoBehaviour<MainMenuUI>
    {
        internal enum LoadedScene { None, Settings, HighScores, HowToPlay}


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

        private LoadedScene m_loadedScene;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            returnButton.SetActive(false);
            MusicManager.CallOnMusicClipChangedEvent(mainMenuMusic);
        }

        public void StartGame()
        {
            DifficultyManager.CallOnDifficultyChangedEvent(DifficultySettingsUI.CurrentDifficulty);
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

            m_loadedScene = LoadedScene.Settings;
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

            m_loadedScene = LoadedScene.HighScores;
            SceneManager.LoadScene((int)SceneIndex.HighScores, LoadSceneMode.Additive);
        }

        public void LoadHowToPlay()
        {
            playButton.SetActive(false);
            settingsButton.SetActive(false);
            instructionsButton.SetActive(false);
            highScoresButton.SetActive(false);
            quitButton.SetActive(false);
            returnButton.SetActive(false);

            m_loadedScene = LoadedScene.HowToPlay;
            SceneManager.LoadScene((int)SceneIndex.HowToPlay, LoadSceneMode.Additive);
        }

        /// <summary>
        /// Returns to the main Menu unloading the scene that was open additively.
        /// Recomended to use on the buttons that are on the Main Menu Scene
        /// </summary>
        public void LoadMainMenu()
        {
            switch (m_loadedScene)
            {
                case LoadedScene.Settings:
                    SceneManager.UnloadSceneAsync((int)SceneIndex.Settings);
                    m_loadedScene = LoadedScene.None;
                    break;
                case LoadedScene.HighScores:
                    SceneManager.UnloadSceneAsync((int)SceneIndex.HighScores);
                    m_loadedScene = LoadedScene.None;
                    break;
                case LoadedScene.HowToPlay:
                    SceneManager.UnloadSceneAsync((int)SceneIndex.HowToPlay);
                    m_loadedScene = LoadedScene.None;
                    break;
                default:
                    break;
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
