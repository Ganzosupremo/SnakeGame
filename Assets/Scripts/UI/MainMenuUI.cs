using SnakeGame.AudioSystem;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SnakeGame.UI
{
    public class MainMenuUI : SingletonMonoBehaviour<MainMenuUI>
    {
        internal enum LoadedScene { None, Settings, HighScores, HowToPlay }

        #region Header Button References
        [Header("Button References")]
        #endregion
        
        [SerializeField] private GameObject playButton;
        [SerializeField] private GameObject returnButton;
        [SerializeField] private GameObject settingsButton;
        [SerializeField] private GameObject instructionsButton;
        [SerializeField] private GameObject highScoresButton;
        [SerializeField] private GameObject quitButton;
        
        #region Header Audio References
        [Space]
        [Header("Audio References")]
        #endregion
        [SerializeField] private MusicSO mainMenuMusic;

        [Space]
        [Header("Modal Window Reference")]
        [SerializeField] private ModalWindow _mainMenuModalWindow;
        [SerializeField] string _modalWindowTitle;
        [SerializeField] private Sprite _modalWindowHeroImage;
        [SerializeField] string _modalWindowMessage;


        private LoadedScene m_loadedScene;
        private bool _characterSceneLoaded = true;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            returnButton.SetActive(false);
            MusicManager.CallOnMusicClipChangedEvent(mainMenuMusic);

            SceneManager.LoadScene((int)SceneIndex.PlayerSettings, LoadSceneMode.Additive);
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
            
            SceneManager.UnloadSceneAsync((int)SceneIndex.PlayerSettings);
            
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
            quitButton.SetActive(false);

            SceneManager.UnloadSceneAsync((int)SceneIndex.PlayerSettings);

            returnButton.SetActive(false);

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

            SceneManager.UnloadSceneAsync((int)SceneIndex.PlayerSettings);

            returnButton.SetActive(false);

            m_loadedScene = LoadedScene.HowToPlay;
            SceneManager.LoadScene((int)SceneIndex.HowToPlay, LoadSceneMode.Additive);
        }

        /// <summary>
        /// Returns to the main Menu unloading the scene that was open additively.
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
            
            SceneManager.LoadScene((int)SceneIndex.PlayerSettings, LoadSceneMode.Additive);

            returnButton.SetActive(false);

            playButton.SetActive(true);
            settingsButton.SetActive(true);
            instructionsButton.SetActive(true);
            highScoresButton.SetActive(true);
            quitButton.SetActive(true);
        }

        private void ChangeStateCharacterSelectorScene()
        {
            if (_characterSceneLoaded)
            {
                SceneManager.UnloadSceneAsync((int)SceneIndex.PlayerSettings);
                _characterSceneLoaded = false;
            }
            else
            {
                SceneManager.LoadScene((int)SceneIndex.PlayerSettings, LoadSceneMode.Additive);
                _characterSceneLoaded = true;
            }
        }

        public void QuitGameButton()
        {
            ChangeStateCharacterSelectorScene();
            _mainMenuModalWindow.ShowWindow(_modalWindowTitle, _modalWindowHeroImage,
                _modalWindowMessage,QuitGame, "Quit Game", CloseWindow, "Return");
        }

        private void CloseWindow()
        {
            ChangeStateCharacterSelectorScene();
            _mainMenuModalWindow.CloseWindow();
        }

        public void QuitGame()
        {
            Application.Quit();
        }

    }
}
