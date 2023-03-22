using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;
using SnakeGame.SoundsSystem;
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

            soundsLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
            musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
            minigunFireVolume.SetText(SoundEffectManager.Instance.minigunFireVolume.ToString());
        }

        /// <summary>
        /// Increase the volume of the music - the volume can be changed with a button in the pause menu UI
        /// </summary>
        public void IncreaseMusicVolume()
        {
            MusicManager.Instance.IncreaseMusicVolume();
            musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
        }

        /// <summary>
        /// Decrease the volume of the music - the volume can be changed with a button in the pause menu UI
        /// </summary>
        public void DecreaseMusicVolume()
        {
            MusicManager.Instance.DecreaseMusicVolume();
            musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
        }

        /// <summary>
        /// Increase the volume of the sound effects - the volume can be changed with a button in the pause menu UI
        /// </summary>
        public void IncreaseSoundsVolume()
        {
            SoundEffectManager.Instance.IncreaseSoundsVolume();
            soundsLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
        }

        /// <summary>
        /// Decrease the volume of the sound effects - the volume can be changed with a button in the pause menu UI
        /// </summary>
        public void DecreaseSoundsVolume()
        {
            SoundEffectManager.Instance.DecreaseSoundsVolume();
            soundsLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
        }

        public void MuteSoundsVolume(bool mute)
        {
            if (mute)
            {
                SoundEffectManager.Instance.MuteSounds();
                soundsLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
            }
        }

        public void IncreaseMinigunFireSound()
        {
            SoundEffectManager.Instance.IncreaseMinigunFireSound();
            minigunFireVolume.SetText(SoundEffectManager.Instance.minigunFireVolume.ToString());
        }

        public void DecreaseMinigunFireSound()
        {
            SoundEffectManager.Instance.DecreaseMinigunFireSound();
            minigunFireVolume.SetText(SoundEffectManager.Instance.minigunFireVolume.ToString());
        }

        public void MuteMinigunSounds(bool mute)
        {
            if (mute)
            {
                SoundEffectManager.Instance.MuteMinigunSoundEffects();
                minigunFireVolume.SetText(SoundEffectManager.Instance.minigunFireVolume.ToString());
            }
        }
    }
}
