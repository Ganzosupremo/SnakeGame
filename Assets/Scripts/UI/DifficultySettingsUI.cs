using Cysharp.Threading.Tasks;
using SnakeGame.Debuging;
using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;
using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace SnakeGame.UI
{
    public class DifficultySettingsUI : MonoBehaviour, IPersistenceData
    {
        public static Difficulty CurrentDifficulty { get { return m_SelectedDifficulty; } }
        [SerializeField] TextMeshProUGUI _DisplayMessage;
        public TMP_Dropdown dropdown;

        private static Difficulty m_SelectedDifficulty = Difficulty.Medium;
        private CancellationTokenSource m_CancellationToken;
        private void Start()
        {
            m_CancellationToken = new();
            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.onValueChanged.AddListener(delegate
            {
                OnDropValueChanged();
            });

            DifficultyManager.CallOnDifficultyChangedEvent(m_SelectedDifficulty);
        }

        private void OnEnable()
        {
            SaveDataManager.Instance.LoadGame();
            DifficultyManager.CallOnDifficultyChangedEvent(m_SelectedDifficulty);
        }

        private void OnDisable()
        {
            m_CancellationToken.Cancel();
            dropdown.onValueChanged.RemoveAllListeners();
        }

        private void OnDropValueChanged()
        {
            m_SelectedDifficulty = (Difficulty)dropdown.value;
            DifficultyManager.CallOnDifficultyChangedEvent(m_SelectedDifficulty);
        }

        public async void SaveGame()
        {
            SaveDataManager.Instance.SaveGame();
            await DisplayMessage("Changes Saved!", 2f, m_CancellationToken.Token);
        }

        /// <summary>
        /// Displays a message on the screen
        /// </summary>
        /// <param name="message"></param>
        /// <param name="displayTime"></param>
        /// <returns></returns>
        private async UniTask DisplayMessage(string message, float displayTime, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            _DisplayMessage.gameObject.SetActive(true);
            _DisplayMessage.text = message;
            await UniTask.Delay((int)displayTime * 1000, false, PlayerLoopTiming.Update, cancellationToken);
            _DisplayMessage.text = "";
        }

        public void Load(GameData data)
        {
            m_SelectedDifficulty = data.SavedDifficulty;
            dropdown.value = (int)data.SavedDifficulty;
        }

        public void Save(GameData data)
        {
            data.SavedDifficulty = m_SelectedDifficulty;
        }
    }
}
