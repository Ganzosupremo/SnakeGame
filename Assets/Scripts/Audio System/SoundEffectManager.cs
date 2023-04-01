using System.Collections;
using UnityEngine;
using SnakeGame;
using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;
using SnakeGame.Debuging;
using SnakeGame.GameUtilities;
using System;

namespace SnakeGame.AudioSystem
{
    /// <summary>
    /// Manages all the SFX's for the game, utilizing the <seealso cref="SoundEffect"/> and <seealso cref="SoundEffectSO"/> classes.
    /// </summary>
    [DisallowMultipleComponent]
    public class SoundEffectManager : SingletonMonoBehaviour<SoundEffectManager>, IPersistenceData
    {
        [Range(0, 20)]
        public int SoundsVolume = 9;
        [Range(0, 20)]
        public int MinigunFireVolume = 9;

        public static event Action OnSFXVolumeIncreased;
        public static event Action OnSFXVolumeDecreased;
        public static event Action OnMinigunSFXVolumeIncreased;
        public static event Action OnMinigunSFXVolumeDecreased;

        public static event Action<SoundEffectSO> OnSoundEffectSelected;

        private void Start()
        {
            SetSoundsVolume(SoundsVolume);
            SetMinigunFireSound(MinigunFireVolume);
        }

        private void OnEnable()
        {
            OnSFXVolumeIncreased += SoundEffectManager_OnSFXVolumeIncreased;
            OnSFXVolumeDecreased += SoundEffectManager_OnSFXVolumeDecreased;
            OnMinigunSFXVolumeIncreased += SoundEffectManager_OnMinigunSFXVolumeIncreased;
            OnMinigunSFXVolumeDecreased += SoundEffectManager_OnMinigunSFXVolumeDecreased;
            OnSoundEffectSelected += SoundEffectManager_OnSoundEffectChanged;
        }



        private void OnDisable()
        {
            OnSFXVolumeIncreased -= SoundEffectManager_OnSFXVolumeIncreased;
            OnSFXVolumeDecreased -= SoundEffectManager_OnSFXVolumeDecreased;
            OnMinigunSFXVolumeIncreased -= SoundEffectManager_OnMinigunSFXVolumeIncreased;
            OnMinigunSFXVolumeDecreased -= SoundEffectManager_OnMinigunSFXVolumeDecreased;
            OnSoundEffectSelected -= SoundEffectManager_OnSoundEffectChanged;
        }

        private void SoundEffectManager_OnSoundEffectChanged(SoundEffectSO soundEffect)
        {
            PlaySoundEffect(soundEffect);
        }

        private void SoundEffectManager_OnMinigunSFXVolumeDecreased()
        {
            DecreaseMinigunFireSound();
        }

        private void SoundEffectManager_OnMinigunSFXVolumeIncreased()
        {
            IncreaseMinigunFireSound();
        }

        private void SoundEffectManager_OnSFXVolumeDecreased()
        {
            DecreaseSoundsVolume();
        }

        private void SoundEffectManager_OnSFXVolumeIncreased()
        {
            IncreaseSoundsVolume();
        }

        /// <summary>
        /// Plays The Selected Sound Effect
        /// </summary>
        private void PlaySoundEffect(SoundEffectSO soundEffect)
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
            if (SoundsVolume >= maxVolume) return;

            SoundsVolume += 1;
            SetSoundsVolume(SoundsVolume);
        }

        /// <summary>
        /// Decrease the sounds volume
        /// </summary>
        [ContextMenu("Decrease Volume")]
        public void DecreaseSoundsVolume()
        {
            if (SoundsVolume == 0) return;

            SoundsVolume -= 1;

            SetSoundsVolume(SoundsVolume);
        }

        public void IncreaseMinigunFireSound()
        {
            int maxVolume = 20;
            if (MinigunFireVolume >= maxVolume) return;

            MinigunFireVolume += 1;

            SetMinigunFireSound(MinigunFireVolume);
        }

        public void DecreaseMinigunFireSound()
        {
            if (MinigunFireVolume == 0) return;

            MinigunFireVolume -= 1;

            SetMinigunFireSound(MinigunFireVolume);
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

        /// <summary>
        /// Calls the <seealso cref="OnSFXVolumeIncreased"/> event, which increases
        /// the volume of the sound effects.
        /// </summary>
        public static void CallSFXVolumeIncreasedEvent()
        {
            OnSFXVolumeIncreased?.Invoke();
        }

        /// <summary>
        /// Calls the <seealso cref="OnSFXVolumeDecreased"/> event, which increases
        /// the volume of the sound effects.
        /// </summary>
        public static void CallSFXVolumeDecreasedEvent()
        {
            OnSFXVolumeDecreased?.Invoke();
        }

        /// <summary>
        /// Calls the <seealso cref="OnMinigunSFXVolumeIncreased"/> event, which only increases
        /// the volume of the sound effects of the weapon minigun.
        /// </summary>
        public static void CallMinigunSFXVolumeIncreasedEvent()
        {
            OnMinigunSFXVolumeIncreased?.Invoke();
        }

        /// <summary>
        /// Calls the <seealso cref="OnMinigunSFXVolumeIncreased"/> event, which only lowers
        /// the volume of the sound effects of the weapon minigun.
        /// </summary>
        public static void CallMinigunSFXVolumeDecreasedEvent()
        {
            OnMinigunSFXVolumeDecreased?.Invoke();
        }

        /// <summary>
        /// Triggers the <seealso cref="OnSoundEffectSelected"/> event.
        /// That event plays the selected SoundEffectSO.
        /// </summary>
        /// <param name="soundEffect"></param>
        public static void CallOnSoundEffectSelectedEvent(SoundEffectSO soundEffect)
        {
            OnSoundEffectSelected?.Invoke(soundEffect);
        }

        public void Load(GameData data)
        {
            SoundsVolume = data.SoundsVolume;
            MinigunFireVolume = data.MinigunVolume;
        }

        public void Save(GameData data)
        {
            data.SoundsVolume = SoundsVolume;
            data.MinigunVolume = MinigunFireVolume;
        }
    }
}