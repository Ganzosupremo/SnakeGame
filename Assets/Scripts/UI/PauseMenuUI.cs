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

        private DayCicle selectedTime;

        void Start()
        {
            dayCicleDropdown.onValueChanged.RemoveAllListeners();
            dayCicleDropdown.onValueChanged.AddListener(delegate
            {
                OnValueChanged(dayCicleDropdown);
            });

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
            //PlayerPrefs.SetInt("CurrentTime", (int)selectedTime);
        }

        private void OnValueChanged(TMP_Dropdown dayCicleDropdown)
        {
            TimeManager.Instance.CallOnTimeChangedEvent((DayCicle)dayCicleDropdown.value);
            //selectedTime = (DayCicle)dayCicleDropdown.value;
            //ChangeDayCicle();
        }

        /// <summary>
        /// Change the global light intensity depending on the selected day time
        /// </summary>
        public void ChangeDayCicle()
        {
            TimeManager.Instance.CallOnTimeChangedEvent(selectedTime);
        }

        private IEnumerator InitializeUI()
        {
            yield return null;

            soundsLevelText.SetText(SoundEffectManager.Instance.SoundsVolume.ToString());
            musicLevelText.SetText(MusicManager.Instance.MusicVolume.ToString());
            minigunFireVolume.SetText(SoundEffectManager.Instance.MinigunFireVolume.ToString());
            dayCicleDropdown.value = (int)selectedTime;
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
            SoundEffectManager.CallMinigunSFXVolumeIncreasedEvent();
            minigunFireVolume.SetText(SoundEffectManager.Instance.MinigunFireVolume.ToString());
        }

        public void DecreaseMinigunFireSound()
        {
            SoundEffectManager.CallMinigunSFXVolumeDecreasedEvent();
            minigunFireVolume.SetText(SoundEffectManager.Instance.MinigunFireVolume.ToString());
        }

        public void Load(GameData data)
        {
            selectedTime = data.SavedTime;
            dayCicleDropdown.value = (int)selectedTime;
        }

        public void Save(GameData data)
        {
            data.SavedTime = selectedTime;
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
