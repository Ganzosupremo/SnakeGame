using UnityEngine.UIElements;

namespace SnakeGame.PlayerSystem
{
    public class SnakeListEntryController
    {
        private Label NameLabel;

        /// <summary>
        /// This function retrieves a reference to the
        /// character name label inside the UI element.
        /// </summary>
        /// <param name="visualElement"></param>
        public void SetVisualElement(VisualElement visualElement)
        {
            NameLabel = visualElement.Q<Label>("character-name");
        }

        /// <summary>
        /// This function receives the character whose name this list element displays
        /// Since the elements listedin a `ListView` are pooled and reused, 
        /// it's necessary tohave a `Set` function to change 
        /// which character's data to display.
        /// </summary>
        /// <param name="characterData"></param>
        public void SetCharacterData(SnakeDetailsSO characterData)
        {
            NameLabel.text = characterData.snakeName;
        }
    }
}
