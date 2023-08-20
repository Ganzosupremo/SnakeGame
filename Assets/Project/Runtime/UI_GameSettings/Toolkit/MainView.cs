using UnityEngine;
using UnityEngine.UIElements;

namespace SnakeGame.PlayerSystem
{
    public class MainView : MonoBehaviour
    {
        [SerializeField]
        VisualTreeAsset ListEntryTemplate;

        void OnEnable()
        {
            // The UXML is already instantiated by the UIDocument component
            var uiDocument = GetComponent<UIDocument>();

            // Initialize the character list controller
            var characterListController = new SnakeListController();
            characterListController.InitializeSnakeList(uiDocument.rootVisualElement, ListEntryTemplate);
        }
    }
}
