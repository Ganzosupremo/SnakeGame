using SnakeGame.Debuging;
using SnakeGame.GameUtilities;
using SnakeGame.HealthSystem;
using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public SoundEffectSO RailgunReloadTest;

        private static event Action OnSFXVolumeIncreased;
        private static event Action OnSFXVolumeDecreased;
        private static event Action OnMinigunSFXVolumeIncreased;
        private static event Action OnMinigunSFXVolumeDecreased;

        private static event Action<SoundEffectSO> OnSoundEffectChanged;

        private static SoundEffectSO _CurrentSoundEffect;
        private static Dictionary<SoundEffectSO, SoundEffect> _ActiveSoundEffects = new();

        private void Start()
        {
            SetSoundsVolume(SoundsVolume);
            SetMinigunFireSound(MinigunFireVolume);
        }

        private void OnEnable()
        {
            OnSFXVolumeIncreased += SFXVolumeIncreased;
            OnSFXVolumeDecreased += SFXVolumeDecreased;
            OnMinigunSFXVolumeIncreased += MinigunSFXVolumeIncreased;
            OnMinigunSFXVolumeDecreased += MninigunSFXVolumeDecreased;
            OnSoundEffectChanged += SoundEffectChanged;
            
            if (GameManager.Instance != null)
                GameManager.Instance.GetSnake().healthEvent.OnHealthChanged += OnHealthChanged;
        }

        private void OnDisable()
        {
            OnSFXVolumeIncreased -= SFXVolumeIncreased;
            OnSFXVolumeDecreased -= SFXVolumeDecreased;
            OnMinigunSFXVolumeIncreased -= MinigunSFXVolumeIncreased;
            OnMinigunSFXVolumeDecreased -= MninigunSFXVolumeDecreased;
            OnSoundEffectChanged -= SoundEffectChanged;
            
            if (GameManager.Instance != null)
                GameManager.Instance.GetSnake().healthEvent.OnHealthChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(HealthEvent healthEvent, HealthEventArgs args)
        {
            float lowHealthThreshold = .33f;
            if (args.healthPercent <= lowHealthThreshold && args.healthPercent >= 0f)
            {
                if (!_ActiveSoundEffects.ContainsKey(GameResources.Instance.LowHealthSoundEffect))
                {
                    PlaySoundEffect(GameResources.Instance.LowHealthSoundEffect, true);
                }
            }
            else
            {
                StopSoundEffect(GameResources.Instance.LowHealthSoundEffect);
            }
        }

        private void SoundEffectChanged(SoundEffectSO soundEffect)
        {
            PlaySoundEffect(soundEffect);
        }

        private void MninigunSFXVolumeDecreased()
        {
            DecreaseMinigunFireSound();
        }

        private void MinigunSFXVolumeIncreased()
        {
            IncreaseMinigunFireSound();
        }

        private void SFXVolumeDecreased()
        {
            DecreaseSoundsVolume();
        }

        private void SFXVolumeIncreased()
        {
            IncreaseSoundsVolume();
        }

        /// <summary>
        /// Plays The Selected Sound Effect
        /// </summary>
        private void PlaySoundEffect(SoundEffectSO soundEffect)
        {
            SoundEffect sound = (SoundEffect)PoolManager.Instance.ReuseComponent(soundEffect.soundPrefab, Vector3.zero);
            sound.SetSound(soundEffect);
            sound.gameObject.SetActive(true);
            
            AddSoundToDictionary(soundEffect, sound);
            
            StartCoroutine(DisableSound(sound, soundEffect.soundEffectClip.length));
            RemoveSoundFromDictionary(soundEffect);
        }

        private void PlaySoundEffect(SoundEffectSO soundEffect, bool shouldLoop)
        {
            SoundEffect sound = (SoundEffect)PoolManager.Instance.ReuseComponent(soundEffect.soundPrefab, Vector3.zero);
            
            sound.SetSound(soundEffect);
            sound.AudioSource.loop = shouldLoop;
            sound.gameObject.SetActive(true);
            
            AddSoundToDictionary(soundEffect, sound);
        }

        private void StopSoundEffect(SoundEffectSO soundEffect)
        {
            if (_ActiveSoundEffects.TryGetValue(soundEffect, out SoundEffect sound))
            {
                StartCoroutine(DisableSound(sound, soundEffect.soundEffectClip.length));
                RemoveSoundFromDictionary(soundEffect);
            }
        }

        /// <summary>
        /// Disable The Sound GameObject After It Has Finished Playing And Thus Returning It To The Pool
        /// </summary>
        private IEnumerator DisableSound(SoundEffect sound, float soundDuration)
        {
            yield return new WaitForSeconds(soundDuration);

            sound.StopSound();
            sound.AudioSource.loop = false;
            sound.gameObject.SetActive(false);
        }

        private void AddSoundToDictionary(SoundEffectSO soundEffectSO, SoundEffect sound)
        {
            if (!_ActiveSoundEffects.ContainsKey(soundEffectSO))
                _ActiveSoundEffects.Add(soundEffectSO, sound);
        }

        private void RemoveSoundFromDictionary(SoundEffectSO soundEffectSO)
        {
            if (_ActiveSoundEffects.ContainsKey(soundEffectSO))
            {
                _ActiveSoundEffects.Remove(soundEffectSO);
            }
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
        /// Triggers the <seealso cref="OnSoundEffectChanged"/> event.
        /// That event plays the selected SoundEffectSO.
        /// </summary>
        /// <param name="soundEffect"></param>
        public static void CallOnSoundEffectChangedEvent(SoundEffectSO soundEffect)
        {
            _CurrentSoundEffect = soundEffect;
            OnSoundEffectChanged?.Invoke(soundEffect);
        }

        public static float GetSoundEffectLength()
        {
            return _CurrentSoundEffect.soundEffectClip.length;
        }

        public void Load(GameData data)
        {
            SoundsVolume = data.VolumeDataSaved.GetSoundsVolume();
            MinigunFireVolume = data.VolumeDataSaved.GetMinigunVolume();
        }

        public void Save(GameData data)
        {
            data.VolumeDataSaved.SoundsVolume = SoundsVolume;
            data.VolumeDataSaved.MinigunVolume = MinigunFireVolume;
        }
    }
}