using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;
using System;

namespace SnakeGame.UI
{
    [Obsolete("This class is obsolete, now that there are different settings tabs," +
        " every setting is managed separately and not on the same class.")]
    public class SettingsUI : MonoBehaviour, IPersistenceData
    {
        [SerializeField] private TMP_Dropdown difficultyDropdown;
        [SerializeField] private TextMeshProUGUI playerHealth;
        [SerializeField] private TextMeshProUGUI enemyHealth;
        [SerializeField] private TextMeshProUGUI enemyImmunity;
        [SerializeField] private TextMeshProUGUI enemyDamage;
        [SerializeField] private TextMeshProUGUI messageToDisplay;
        
        private Difficulty selectedDifficulty;

        private void Start()
        {
            difficultyDropdown.onValueChanged.AddListener(delegate
            {
                OnDropValueChange(difficultyDropdown);
            });

            DifficultyManager.ChangeDifficulty(selectedDifficulty);
        }

        private void OnDropValueChange(TMP_Dropdown difficultyDropdown)
        {
            selectedDifficulty = (Difficulty)difficultyDropdown.value;
        }

        ///// <summary>
        ///// Shows a little of what the player would expect with the selected difficulty
        ///// </summary>
        ///// <param name="text"></param>
        //private void ShowDescription()
        //{
        //    switch (selectedDifficulty)
        //    {
        //        case Difficulty.Noob:
        //            playerHealth.text = "Player's Initial Health: 10";
        //            enemyHealth.text = "Enemies' Health Increased By: 0";
        //            enemyImmunity.text = "Are Enemies Immune After Hit? No.";
        //            enemyDamage.text = "Enemies' Damage Increased By: 0";
        //            break;
        //        case Difficulty.Easy:
        //            playerHealth.text = "Player's Initial Health: 8";
        //            enemyHealth.text = "Enemies' Health Increased By: 50";
        //            enemyImmunity.text = "Are Enemies Immune After Hit? No.";
        //            enemyDamage.text = "Enemies' Damage Increased By: 0";
        //            break;
        //        case Difficulty.Medium:
        //            playerHealth.text = "Player's Initial Health: 6";
        //            enemyHealth.text = "Enemies' Health Increased By: 100";
        //            enemyImmunity.text = "Are Enemies Immune After Hit? No.";
        //            enemyDamage.text = "Enemies' Damage Increased By: 0";
        //            break;
        //        case Difficulty.Hard:
        //            playerHealth.text = "Player's Initial Health: 4";
        //            enemyHealth.text = "Enemies' Health Increased By: 150";
        //            enemyImmunity.text = "Are Enemies Immune After Hit? No.";
        //            enemyDamage.text = "Enemies' Damage Increased By: 0";
        //            break;
        //        case Difficulty.DarkSouls:
        //            playerHealth.text = "Player's Initial Health: 3";
        //            enemyHealth.text = "Enemies' Health Increased By: 250";
        //            enemyImmunity.text = "Are Enemies Immune After Hit? Yes. For: 2s";
        //            enemyDamage.text = "Enemies' Damage Increased By: 1";
        //            break;
        //        case Difficulty.EmotionalDamage:
        //            playerHealth.text = "Player's Initial Health: 1";
        //            enemyHealth.text = "Enemies' Health Increased By: 500";
        //            enemyImmunity.text = "Are Enemies Immune After Hit? YES! For: 4s";
        //            enemyDamage.text = "Enemies' Damage Increased By: 2";
        //            break;
        //        default:
        //            break;
        //    }
        //}

        public void ReturnMainMenu()
        {
            SceneManager.UnloadSceneAsync((int)SceneIndex.Settings);
            SceneManager.LoadScene((int)SceneIndex.MainMenu);
        }

        public void SaveChanges()
        {
            DifficultyManager.ChangeDifficulty(selectedDifficulty);
            SaveDataManager.Instance.SaveGame();
            StartCoroutine(DisplayMessage("Changes Saved", 2f));
        }

        private IEnumerator DisplayMessage(string message, float time)
        {
            yield return null;
            messageToDisplay.text = message;
            yield return new WaitForSeconds(time);
            messageToDisplay.text = "";
        }

        public void Load(GameData data)
        {
            selectedDifficulty = data.SavedDifficulty;
            difficultyDropdown.value = (int)selectedDifficulty;
        }

        public void Save(GameData data)
        {
            data.SavedDifficulty = selectedDifficulty;
        }
    }
}
