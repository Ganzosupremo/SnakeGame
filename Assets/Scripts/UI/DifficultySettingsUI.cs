using SnakeGame.Debuging;
using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;
using TMPro;
using UnityEngine;

namespace SnakeGame.UI
{
    public class DifficultySettingsUI : MonoBehaviour, IPersistenceData
    {
        public static Difficulty CurrentDifficulty { get { return selectedDifficulty; } }

        private static Difficulty selectedDifficulty = Difficulty.Medium;
        public TMP_Dropdown dropdown;

        private void Start()
        {
            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.onValueChanged.AddListener(delegate
            {
                OnDropValueChanged();
            });

            DifficultyManager.CallOnDifficultyChangedEvent(selectedDifficulty);
        }

        private void OnEnable()
        {
            DifficultyManager.CallOnDifficultyChangedEvent(selectedDifficulty);
        }

        private void OnDisable()
        {
            dropdown.onValueChanged.RemoveAllListeners();
        }

        private void OnDropValueChanged()
        {
            selectedDifficulty = (Difficulty)dropdown.value;
            DifficultyManager.CallOnDifficultyChangedEvent(selectedDifficulty);
        }

        public void SaveGame()
        {
            SaveDataManager.Instance.SaveGame();
            StartCoroutine(SaveDataManager.Instance.DisplayMessage("Changes Saved Succesfully.", 3.5f));
        }

        public void Load(GameData data)
        {
            selectedDifficulty = data.SavedDifficulty;
            dropdown.value = (int)data.SavedDifficulty;
        }

        public void Save(GameData data)
        {
            data.SavedDifficulty = selectedDifficulty;
        }
    }
}
