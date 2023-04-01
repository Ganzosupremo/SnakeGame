using SnakeGame.Debuging;
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
            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.onValueChanged.AddListener(delegate
            {
                OnDropValueChange();
            });

            DifficultyManager.CallOnDifficultyChangedEvent(selectedDifficulty);
        }

        private void OnEnable()
        {
            //SaveDataManager.Instance.LoadGame();
            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.onValueChanged.AddListener(delegate
            {
                OnDropValueChange();
            });

            DifficultyManager.CallOnDifficultyChangedEvent(selectedDifficulty);
        }

        //private void OnDisable()
        //{
        //    SaveDataManager.Instance.SaveGame();
        //}

        private void OnDropValueChange()
        {
            selectedDifficulty = (Difficulty)dropdown.value;
            DifficultyManager.CallOnDifficultyChangedEvent(selectedDifficulty);
            //SaveDataManager.Instance.SaveGame();
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
