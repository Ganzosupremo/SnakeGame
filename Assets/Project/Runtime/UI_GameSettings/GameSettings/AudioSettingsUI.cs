using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;
using SnakeGame.AudioSystem;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SnakeGame.UI
{
    public class AudioSettingsUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI musicLevelText;
        [SerializeField] private TextMeshProUGUI soundsLevelText;
        [SerializeField] private TextMeshProUGUI minigunFireVolume;

        [Space]
        [Header("Sound Sliders")]
        [SerializeField] private Slider _musicLevelSlider;
        [SerializeField] private Slider _sfxLevelSlider;
        [SerializeField] private Slider _heavyArsenalLevelSlider;

        private void OnEnable()
        {
            SaveDataManager.Instance.LoadGame();
            StartCoroutine(InitializeUI());
        }

        private void OnDisable()
        {
            SaveDataManager.Instance.SaveGame();
        }

        private IEnumerator InitializeUI()
        {
            yield return null;

            soundsLevelText.SetText($"SFXs Volume: {SoundEffectManager.Instance.SoundsVolume}");
            musicLevelText.SetText($"Music Volume: {MusicManager.Instance.MusicVolume}");
            minigunFireVolume.SetText($"Heavy Arsenal Volume: {SoundEffectManager.Instance.HeavyArsenalVolume}");

            _musicLevelSlider.value = MusicManager.Instance.MusicVolume;
            _sfxLevelSlider.value = SoundEffectManager.Instance.SoundsVolume;
            _heavyArsenalLevelSlider.value = SoundEffectManager.Instance.HeavyArsenalVolume;
        }

        public void ChangeMusicVolume(float volume)
        {
            MusicManager.CallOnMusicVolumeChangedEvent(volume);
            musicLevelText.SetText($"Music Volume: {volume}");
        }

        public void ChangeSFXVolume(float volume)
        {
            SoundEffectManager.CallSFXVolumeChangedEvent(volume);
            soundsLevelText.SetText($"SFXs Volume: {volume}");
        }

        public void ChangeHeavyArsenalVolume(float volume)
        {
            SoundEffectManager.CallHeavyArsenalVolumeChangedEvent(volume);
            minigunFireVolume.SetText($"Heavy Arsenal Volume: {volume}");
        }
    }
}
