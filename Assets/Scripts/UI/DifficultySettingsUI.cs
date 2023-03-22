using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;
using TMPro;
using UnityEngine;

namespace SnakeGame.UI
{
    public class DifficultySettingsUI : MonoBehaviour, IPersistenceData
    {
        private Difficulty selectedDifficulty;
        public TMP_Dropdown dropdown;

        private void Start()
        {
            dropdown.onValueChanged.AddListener(delegate
            {
                OnDropValueChange(dropdown);
            });

            DifficultyManager.ChangeDifficulty(selectedDifficulty);
        }

        private void OnDisable()
        {
            SaveDataManager.Instance.SaveGame();
        }

        private void OnDropValueChange(TMP_Dropdown difficultyDropdown)
        {
            selectedDifficulty = (Difficulty)difficultyDropdown.value;
            DifficultyManager.ChangeDifficulty(selectedDifficulty);
        }

        public void Load(GameData data)
        {
            selectedDifficulty = data.SavedDifficulty;
            dropdown.value = (int)selectedDifficulty;
        }

        public void Save(GameData data)
        {
            data.SavedDifficulty = selectedDifficulty;
        }
    }
}
