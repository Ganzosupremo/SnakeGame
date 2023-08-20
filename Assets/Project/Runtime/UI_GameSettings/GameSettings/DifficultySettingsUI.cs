using Cysharp.Threading.Tasks;
using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;
using System.Threading;
using TMPro;
using UnityEngine;
using Timer = SnakeGame.TimeSystem.Timer;

namespace SnakeGame.UI
{
    public class DifficultySettingsUI : MonoBehaviour, IPersistenceData
    {
        [SerializeField] TextMeshProUGUI _DisplayMessage;
        public TMP_Dropdown dropdown;

        private static Difficulty _selectedDifficulty;
        private CancellationTokenSource m_CancellationToken;
        private void Start()
        {
            m_CancellationToken = new();
            //dropdown.onValueChanged.RemoveAllListeners();
            //dropdown.onValueChanged.AddListener(delegate
            //{
            //    OnDropValueChanged();
            //});

            DifficultyManager.CallOnDifficultyChangedEvent(_selectedDifficulty);
        }

        private void OnEnable()
        {
            SaveDataManager.Instance.LoadGame();
            DifficultyManager.CallOnDifficultyChangedEvent(_selectedDifficulty);
            dropdown.value = (int)_selectedDifficulty;
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

        public void SetDifficulty(int index)
        {
            _selectedDifficulty = (Difficulty)index;
            ChangeSecondsToRealtime();
            DifficultyManager.CallOnDifficultyChangedEvent(_selectedDifficulty);
        }

        /// <summary>
        /// Changes the value of <seealso cref="Timer.SecondsToRealTime"/>
        /// changing how fast or slow the in-game time flows.
        /// </summary>
        private void ChangeSecondsToRealtime()
        {
            switch (_selectedDifficulty)
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
            _DisplayMessage.gameObject.SetActive(false);
        }

        public void Load(GameData data)
        {
            _selectedDifficulty = data.DifficultyData.DifficultyToSave;
        }

        public void Save(GameData data)
        {
            data.DifficultyData.DifficultyToSave = _selectedDifficulty;
            data.TimeDataSaved.SecondsToRealTime = Timer.SecondsToRealTime;
        }
    }
}
