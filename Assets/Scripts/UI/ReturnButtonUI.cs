using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SnakeGame.UI
{
    public class ReturnButtonUI : MonoBehaviour
    {
        private Button button;

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
            MainMenuUI.Instance.LoadMainMenu();
        }
    }
}
