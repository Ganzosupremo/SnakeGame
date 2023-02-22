using System.Collections;
using UnityEngine;
using SnakeGame;
using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;

namespace SnakeGame.SoundsSystem
{
    /// <summary>
    /// Manages all the SFS's for the game, with the help of <seealso cref="SoundEffect"/> and <seealso cref="SoundEffectSO"/> classes.
    /// </summary>
    [DisallowMultipleComponent]
    public class SoundEffectManager : SingletonMonoBehaviour<SoundEffectManager>, IPersistenceData
    {
        [Range(0, 20)]
        public int soundsVolume = 9;
        [Range(0, 20)]
        public int minigunFireVolume = 9;

        private void Start()
        {
            //if (PlayerPrefs.HasKey(nameof(soundsVolume)))
            //{
            //    soundsVolume = PlayerPrefs.GetInt(nameof(soundsVolume));
            //}

            SetSoundsVolume(soundsVolume);
            SetMinigunFireSound(minigunFireVolume);
        }

        private void OnDisable()
        {
            //PlayerPrefs.SetInt(nameof(soundsVolume), soundsVolume);
        }

        /// <summary>
        /// Plays The Selected Sound Effect
        /// </summary>
        public void PlaySoundEffect(SoundEffectSO soundEffect)
        {
            SoundEffect sound = (SoundEffect)PoolManager.Instance.ReuseComponent(soundEffect.soundPrefab, Vector3.zero,
                Quaternion.identity);
            sound.SetSound(soundEffect);
            sound.gameObject.SetActive(true);
            StartCoroutine(DisableSound(sound, soundEffect.soundEffectClip.length));
        }

        /// <summary>
        /// Disable The Sound GameObject After It Has Finished Playing And Thus Returning It To The Pool
        /// </summary>
        private IEnumerator DisableSound(SoundEffect sound, float soundDuration)
        {
            yield return new WaitForSeconds(soundDuration);
            sound.gameObject.SetActive(false);
        }

        /// <summary>
        /// Increase the sounds volume
        /// </summary>
        [ContextMenu("Increase Volume")]
        public void IncreaseSoundsVolume()
        {
            int maxVolume = 20;
            if (soundsVolume >= maxVolume) return;

            soundsVolume += 1;
            SetSoundsVolume(soundsVolume);
        }

        /// <summary>
        /// Decrease the sounds volume
        /// </summary>
        [ContextMenu("Decrease Volume")]
        public void DecreaseMusicVolume()
        {
            if (soundsVolume == 0) return;

            soundsVolume -= 1;

            SetSoundsVolume(soundsVolume);
        }

        public void IncreaseMinigunFireSound()
        {
            int maxVolume = 20;
            if (minigunFireVolume >= maxVolume) return;

            minigunFireVolume += 1;

            SetMinigunFireSound(minigunFireVolume);
        }

        public void DecreaseMinigunFireSound()
        {
            if (minigunFireVolume == 0) return;

            minigunFireVolume -= 1;

            SetMinigunFireSound(minigunFireVolume);
        }

        private void SetMinigunFireSound(int volume)
        {
            float mutevolumedecibels = -80f;

            if (volume == 0)
            {
                GameResources.Instance.minigunFireMixerGroup.audioMixer.SetFloat("MinigunFire", mutevolumedecibels);
            }
            else
            {
                GameResources.Instance.minigunFireMixerGroup.audioMixer.SetFloat("MinigunFire",
                    HelperUtilities.LinearToDecibels(volume));
            }
        }

        /// <summary>
        /// Sets The Volume Of The Sound Effects
        /// </summary>
        private void SetSoundsVolume(int soundsVolume)
        {
            float muteVolumeDecibels = -80f;

            if (soundsVolume == 0)
            {
                GameResources.Instance.soundMasterMixerGroup.audioMixer.SetFloat("SFXVolume", muteVolumeDecibels);
            }
            else
            {
                GameResources.Instance.soundMasterMixerGroup.audioMixer.SetFloat("SFXVolume",
                    HelperUtilities.LinearToDecibels(soundsVolume));
            }
        }

        public void Load(GameData data)
        {
            soundsVolume = data.soundsVolume;
            minigunFireVolume = data.minigunVolume;
        }

        public void Save(GameData data)
        {
            data.soundsVolume = soundsVolume;
            data.minigunVolume = minigunFireVolume;
        }
    }
}