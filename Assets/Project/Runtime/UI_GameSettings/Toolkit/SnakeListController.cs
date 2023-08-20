using System.Collections.Generic;
using UnityEngine.UIElements;

namespace SnakeGame.PlayerSystem
{
    public class SnakeListController
    {
        // UXML template for list entries
        VisualTreeAsset ListEntryTemplate;

        // UI element references
        ListView SnakeListView;
        Label SnakeAbilityLabel;
        Label SnakeNameLabel;
        VisualElement SnakePortrait;
        VisualElement ScrollerContainer;

        List<SnakeDetailsSO> AllSnakesList;

        private CurrentPlayerSO CurrentSnake;
        public void InitializeSnakeList(VisualElement root, VisualTreeAsset listElementTemplate)
        {
            EnumerateAllSnakes();

            ListEntryTemplate = listElementTemplate;

            // Store a reference to the character list element
            SnakeListView = root.Q<ListView>("character-list");

            // Store references to the selected character info elements
            SnakeAbilityLabel = root.Q<Label>("character-class");
            SnakeNameLabel = root.Q<Label>("character-name");
            SnakePortrait = root.Q<VisualElement>("character-portrait");
            //ScrollerContainer = root.Q<VisualElement>("unity-content-and-vertical-scroll-container");

            //ScrollerContainer.visible = false;
            FillCharacterList();

            // Register to get a callback when an item is selected
            SnakeListView.selectionChanged += OnSnakeSelected;
        }

        private void EnumerateAllSnakes()
        {
            AllSnakesList = new List<SnakeDetailsSO>();
            AllSnakesList = GameResources.Instance.snakeDetailsList;
            CurrentSnake = GameResources.Instance.currentSnake;
        }

        private void FillCharacterList()
        {
            SnakeListView.makeItem = () =>
            {
                // Instantiate the UXML template for the entry
                var newListEntry = ListEntryTemplate.Instantiate();

                // Instantiate a controller for the data
                var newListEntryLogic = new SnakeListEntryController();

                // Assign the controller script to the visual element
                newListEntry.userData = newListEntryLogic;

                // Initialize the controller script
                newListEntryLogic.SetVisualElement(newListEntry);

                // Return the root of the instantiated visual tree
                return newListEntry;
            };

            // Set up bind function for a specific list entry
            SnakeListView.bindItem = (item, index) =>
            {
                (item.userData as SnakeListEntryController).SetCharacterData(AllSnakesList[index]);
            };

            // Set a fixed item height
            SnakeListView.fixedItemHeight = 25f;

            // Set the actual item's source list/array
            SnakeListView.itemsSource = AllSnakesList;
        }

        private void OnSnakeSelected(IEnumerable<object> enumerable)
        {
            // Get the currently selected item directly from the ListView
            SnakeDetailsSO selectedSnake = SnakeListView.selectedItem as SnakeDetailsSO;

            // Handle none-selection (Escape to deselect everything)
            if (selectedSnake == null)
            {
                // Clear
                SnakeAbilityLabel.text = "No Type";
                SnakeNameLabel.text = "No Snake Selected";
                SnakePortrait.style.backgroundImage = null;

                return;
            }

            CurrentSnake.snakeDetails = selectedSnake;

            // Fill in character details
            SnakeAbilityLabel.text = selectedSnake.ability.ToString();
            SnakeNameLabel.text = selectedSnake.snakeName;
            SnakePortrait.style.backgroundImage = new StyleBackground(selectedSnake.PortraitImage);
        }
    }
}
