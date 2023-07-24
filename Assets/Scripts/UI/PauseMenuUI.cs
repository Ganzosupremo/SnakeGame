using SnakeGame.AudioSystem;
using SnakeGame.GameUtilities;
using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;
using SnakeGame.TimeSystem;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SnakeGame.UI
{
    public class PauseMenuUI : MonoBehaviour, IPersistenceData
    {
        [SerializeField] private TextMeshProUGUI musicLevelText;
        [SerializeField] private TextMeshProUGUI soundsLevelText;
        [SerializeField] private TextMeshProUGUI minigunFireVolume;
        [SerializeField] private TMP_Dropdown dayCicleDropdown;

        private DayCicle _selectedTime;

        void Start()
        {
            //dayCicleDropdown.onValueChanged.RemoveAllListeners();
            //dayCicleDropdown.onValueChanged.AddListener(delegate
            //{
            //    OnValueChanged(dayCicleDropdown);
            //});

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            SaveDataManager.Instance.LoadGame();
            Time.timeScale = 0f;
            StartCoroutine(InitializeUI());
        }

        private void OnDisable()
        {
            Time.timeScale = 1f;
            SaveDataManager.Instance.SaveGame();
        }

        private void OnValueChanged(TMP_Dropdown dayCicleDropdown)
        {
            TimeManager.Instance.CallOnTimeChangedEvent((DayCicle)dayCicleDropdown.value);
        }

        public void SetDayTime(int dayIndex)
        {
            _selectedTime = (DayCicle)dayIndex;
            TimeManager.Instance.CallOnTimeChangedEvent((DayCicle)dayIndex);
        }

        private IEnumerator InitializeUI()
        {
            yield return null;

            soundsLevelText.SetText(SoundEffectManager.Instance.SoundsVolume.ToString());
            musicLevelText.SetText(MusicManager.Instance.MusicVolume.ToString());
            minigunFireVolume.SetText(SoundEffectManager.Instance.HeavyArsenalVolume.ToString());
            dayCicleDropdown.value = (int)_selectedTime;
        }

        public void LoadMainMenu()
        {
            SceneManager.LoadSceneAsync((int)SceneIndex.MainMenu, LoadSceneMode.Single);
            //SceneManager.UnloadSceneAsync((int)SceneIndex.MainGame);
        }

        /// <summary>
        /// Increase the volume of the music.
        /// The volume can be changed with a button in the pause menu UI
        /// </summary>
        public void IncreaseMusicVolume()
        {
            MusicManager.CallOnMusicVolumeIncreasedEvent();
            musicLevelText.SetText(MusicManager.Instance.MusicVolume.ToString());
        }

        /// <summary>
        /// Decrease the volume of the music - the volume can be changed with a button in the pause menu UI
        /// </summary>
        public void DecreaseMusicVolume()
        {
            MusicManager.CallOnMusicVolumeDecreasedEvent();
            musicLevelText.SetText(MusicManager.Instance.MusicVolume.ToString());
        }

        /// <summary>
        /// Increase the volume of the sound effects - the volume can be changed with a button in the pause menu UI
        /// </summary>
        public void IncreaseSoundsVolume()
        {
            SoundEffectManager.CallSFXVolumeIncreasedEvent();
            soundsLevelText.SetText(SoundEffectManager.Instance.SoundsVolume.ToString());
        }

        /// <summary>
        /// Decrease the volume of the sound effects - the volume can be changed with a button in the pause menu UI
        /// </summary>
        public void DecreaseSoundsVolume()
        {
            SoundEffectManager.CallSFXVolumeDecreasedEvent();
            soundsLevelText.SetText(SoundEffectManager.Instance.SoundsVolume.ToString());
        }

        public void IncreaseMinigunFireSound()
        {
            SoundEffectManager.CallHeavyArsenalVolumeIncreasedEvent();
            minigunFireVolume.SetText(SoundEffectManager.Instance.HeavyArsenalVolume.ToString());
        }

        public void DecreaseMinigunFireSound()
        {
            SoundEffectManager.CalHeavyArsenalVolumeDecreasedEvent();
            minigunFireVolume.SetText(SoundEffectManager.Instance.HeavyArsenalVolume.ToString());
        }

        public void Load(GameData data)
        {
            _selectedTime = data.TimeDataSaved.SavedTime;
            dayCicleDropdown.value = (int)_selectedTime;
        }

        public void Save(GameData data)
        {
            data.TimeDataSaved.SavedTime = _selectedTime;
        }

        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValue(this, nameof(musicLevelText), musicLevelText);
            HelperUtilities.ValidateCheckNullValue(this, nameof(soundsLevelText), soundsLevelText);
            HelperUtilities.ValidateCheckNullValue(this, nameof(minigunFireVolume), minigunFireVolume);
        }
#endif
        #endregion
    }
}
