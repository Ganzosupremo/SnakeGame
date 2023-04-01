using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;
using SnakeGame.AudioSystem;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace SnakeGame.UI
{
    public class AudioSettingsUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI musicLevelText;
        [SerializeField] private TextMeshProUGUI soundsLevelText;
        [SerializeField] private TextMeshProUGUI minigunFireVolume;

        private void OnEnable()
        {
            StartCoroutine(InitializeUI());
        }

        private IEnumerator InitializeUI()
        {
            yield return null;

            soundsLevelText.SetText(SoundEffectManager.Instance.SoundsVolume.ToString());
            musicLevelText.SetText(MusicManager.Instance.MusicVolume.ToString());
            minigunFireVolume.SetText(SoundEffectManager.Instance.MinigunFireVolume.ToString());
        }

        /// <summary>
        /// Increase the volume of the music - the volume can be changed with a button in the pause menu UI
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
    }
}
