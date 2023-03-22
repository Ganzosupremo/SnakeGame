using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SnakeGame.UI
{
    public class ReturnButtonUI : MonoBehaviour
    {
        internal enum SceneToUnload { Settings, HighScores}

        private Button button;
        internal static SceneToUnload sceneToUnload;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        // Start is called before the first frame update
        void Start()
        {
            button.onClick.AddListener(delegate
            {
                OnClick(button);
            });
        }

        private void OnClick(Button button)
        {
            switch (sceneToUnload)
            {
                case SceneToUnload.Settings:
                    SceneManager.UnloadSceneAsync((int)SceneIndex.Settings);
                    MainMenuUI.Instance.ReturnButtonMainMenu();
                    break;
                case SceneToUnload.HighScores:
                    SceneManager.UnloadSceneAsync((int)SceneIndex.HighScores);
                    MainMenuUI.Instance.ReturnButtonMainMenu();
                    break;
                default:
                    break;
            }
        }
    }
}
