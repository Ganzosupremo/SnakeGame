using UnityEngine;
using UnityEngine.SceneManagement;

namespace SnakeGame.UI
{
    public class MainMenuUI : SingletonMonoBehaviour<MainMenuUI>
    {
        [SerializeField] private GameObject playButton;
        [SerializeField] private GameObject returnButton;
        [SerializeField] private GameObject settingsButton;
        [SerializeField] private GameObject instructionsButton;

        private bool isSettingsLoaded = false;


        private void Start()
        {
            returnButton.SetActive(false);
        }

        public void StartGame()
        {
            SceneManager.LoadSceneAsync((int)SceneIndex.MainGame);
        }

        public void LoadSettings()
        {
            playButton.SetActive(false);
            settingsButton.SetActive(false);
            instructionsButton.SetActive(false);

            returnButton.SetActive(false);

            isSettingsLoaded = true;

            SceneManager.LoadScene((int)SceneIndex.Settings, LoadSceneMode.Additive);
        }

        // Used in the return button on the main menu
        public void LoadMainMenu()
        {
            if (isSettingsLoaded)
            {
                SceneManager.UnloadSceneAsync((int)SceneIndex.Settings);
                isSettingsLoaded = false;
            }

            SceneManager.LoadScene((int)SceneIndex.MainMenu);

            returnButton.SetActive(false);
            playButton.SetActive(true);
            settingsButton.SetActive(true);
            instructionsButton.SetActive(true);
        }

        public void ReturnMainMenu()
        {
            SceneManager.LoadScene((int)SceneIndex.MainMenu, LoadSceneMode.Additive);

            playButton.SetActive(true);
            settingsButton.SetActive(true);
            instructionsButton.SetActive(true);

            returnButton.SetActive(false);
        }

    }
}
