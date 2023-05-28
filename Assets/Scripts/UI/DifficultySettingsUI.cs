using Cysharp.Threading.Tasks;
using SnakeGame.Debuging;
using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;
using System;
using System.Threading;
using SnakeGame.TimeSystem;
using TMPro;
using UnityEngine;
using Timer = SnakeGame.TimeSystem.Timer;
using SnakeGame.Enemies;

namespace SnakeGame.UI
{
    public class DifficultySettingsUI : MonoBehaviour, IPersistenceData
    {
        [SerializeField] TextMeshProUGUI _DisplayMessage;
        public TMP_Dropdown dropdown;

        private static Difficulty m_SelectedDifficulty;
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
            SaveDataManager.Instance.SaveGame();
            dropdown.onValueChanged.RemoveAllListeners();
        }

        private void OnDestroy()
        {
            m_CancellationToken.Dispose();
        }

        private void OnDropValueChanged()
        {
            m_SelectedDifficulty = (Difficulty)dropdown.value;
            ChangeSecondsToRealtime();
            DifficultyManager.CallOnDifficultyChangedEvent(m_SelectedDifficulty);
        }

        /// <summary>
        /// Changes the value of <seealso cref="Timer.SecondsToRealTime"/>
        /// changing how fast or slow the in-game time flows.
        /// </summary>
        private void ChangeSecondsToRealtime()
        {
            switch (m_SelectedDifficulty)
            {
                case Difficulty.Noob:
                    Timer.SecondsToRealTime = 1f;
                    break;
                case Difficulty.Easy:
                    Timer.SecondsToRealTime = .9f;
                    break;
                case Difficulty.Medium:
                    Timer.SecondsToRealTime = .8f;
                    break;
                case Difficulty.Hard:
                    Timer.SecondsToRealTime = .6f;
                    break;
                case Difficulty.DarkSouls:
                    Timer.SecondsToRealTime = .5f;
                    break;
                case Difficulty.EmotionalDamage:
                    Timer.SecondsToRealTime = .3f;
                    break;
                default:
                    break;
            }
        }

        public async void SaveGame()
        {
            SaveDataManager.Instance.SaveGame();
            await DisplayMessage("Changes Saved!", 1.5f, m_CancellationToken.Token);
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
            m_SelectedDifficulty = data.DifficultyData.DifficultyToSave;
            dropdown.value = (int)data.DifficultyData.DifficultyToSave;
        }

        public void Save(GameData data)
        {
            data.DifficultyData.DifficultyToSave = m_SelectedDifficulty;
            data.TimeDataSaved.SecondsToRealTime = Timer.SecondsToRealTime;
        }
    }
}
