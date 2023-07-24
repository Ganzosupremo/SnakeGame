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
        public int SoundsVolume { get => _soundsVolume; set => _soundsVolume = value; }
        public int HeavyArsenalVolume { get => _heavyArsenalVolume; set => _heavyArsenalVolume = value; }

        private static event Action OnSFXVolumeIncreased;
        private static event Action OnSFXVolumeDecreased;
        private static event Action OnHeavyArsenalVolumeIncreased;
        private static event Action OnHeavyArsenalVolumeDecreased;

        private static event Action<SoundEffectSO, bool> OnSoundEffectChanged;

        private static SoundEffectSO _CurrentSoundEffect;
        private readonly static Dictionary<SoundEffectSO, SoundEffect> _ActiveSoundEffects = new();


        private int _soundsVolume = 9;
        private int _heavyArsenalVolume = 7;
        private void Start()
        {
            SetSoundsVolume(_soundsVolume);
            SetHeavyArsenalVolume(_heavyArsenalVolume);
        }

        private void OnEnable()
        {
            OnSFXVolumeIncreased += SFXVolumeIncreased;
            OnSFXVolumeDecreased += SFXVolumeDecreased;
            OnHeavyArsenalVolumeIncreased += MinigunSFXVolumeIncreased;
            OnHeavyArsenalVolumeDecreased += MninigunSFXVolumeDecreased;
            OnSoundEffectChanged += SoundEffectChanged;
            
            if (GameManager.Instance != null)
                GameManager.Instance.GetSnake().healthEvent.OnHealthChanged += OnHealthChanged;
        }

        private void OnDisable()
        {
            OnSFXVolumeIncreased -= SFXVolumeIncreased;
            OnSFXVolumeDecreased -= SFXVolumeDecreased;
            OnHeavyArsenalVolumeIncreased -= MinigunSFXVolumeIncreased;
            OnHeavyArsenalVolumeDecreased -= MninigunSFXVolumeDecreased;
            OnSoundEffectChanged -= SoundEffectChanged;
            
            if (GameManager.Instance != null)
                GameManager.Instance.GetSnake().healthEvent.OnHealthChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(HealthEvent healthEvent, HealthEventArgs args)
        {
            float lowHealthThreshold = .35f;
            if (args.healthPercent <= lowHealthThreshold && args.healthPercent >= 0f)
            {
                if (!_ActiveSoundEffects.ContainsKey(GameResources.Instance.LowHealthSoundEffect))
                    PlaySoundEffect(GameResources.Instance.LowHealthSoundEffect, true);
            }
            else
                StopSoundEffect(GameResources.Instance.LowHealthSoundEffect);
        }

        private void SoundEffectChanged(SoundEffectSO soundEffect, bool usePoolManager)
        {
            if (usePoolManager)
                PlaySoundEffect(soundEffect);
            else
                PlaySoundEffect(soundEffect, 0);
        }

        private void MninigunSFXVolumeDecreased()
        {
            DecreaseHeavyArsenalVolume();
        }

        private void MinigunSFXVolumeIncreased()
        {
            IncreaseHeavyArsenalVolume();
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
            
            StartCoroutine(StopSoundEffectRoutine(sound, soundEffect.soundEffectClip.length));
            RemoveSoundFromDictionary(soundEffect);
        }

        /// <summary>
        /// User this method only when the PoolManager Instance is not available
        /// </summary>
        /// <param name="soundEffect"></param>
        /// <param name="i"></param>
        private void PlaySoundEffect(SoundEffectSO soundEffect, int i = 0)
        {
            SoundEffect sound = Instantiate(soundEffect.soundPrefab.GetComponent<SoundEffect>(), this.transform);

            sound.SetSound(soundEffect);
            sound.gameObject.SetActive(true);

            AddSoundToDictionary(soundEffect, sound);

            StartCoroutine(StopSoundEffectRoutine(sound, soundEffect.soundEffectClip.length));
            RemoveSoundFromDictionary(soundEffect);
        }

        /// <summary>
        /// Plays sounds and makes them loop indefinitely.
        /// The sounds should be stopped manually with the StopSoundEffect method.
        /// </summary>
        /// <param name="soundEffect"></param>
        /// <param name="shouldLoop"></param>
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
                StartCoroutine(StopSoundEffectRoutine(sound, soundEffect.soundEffectClip.length));
                RemoveSoundFromDictionary(soundEffect);
            }
        }

        /// <summary>
        /// Disable The Sound GameObject After It Has Finished Playing And Thus Returning It To The Pool
        /// </summary>
        private IEnumerator StopSoundEffectRoutine(SoundEffect sound, float soundDuration)
        {
            yield return new WaitForSeconds(soundDuration + 0.5f);

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
                _ActiveSoundEffects.Remove(soundEffectSO);
        }

        /// <summary>
        /// Increase the sounds volume
        /// </summary>
        [ContextMenu("Increase Volume")]
        public void IncreaseSoundsVolume()
        {
            int maxVolume = 20;
            if (_soundsVolume >= maxVolume) return;

            _soundsVolume += 1;
            SetSoundsVolume(_soundsVolume);
        }

        /// <summary>
        /// Decrease the sounds volume
        /// </summary>
        [ContextMenu("Decrease Volume")]
        public void DecreaseSoundsVolume()
        {
            if (_soundsVolume == 0) return;

            _soundsVolume -= 1;

            SetSoundsVolume(_soundsVolume);
        }

        public void IncreaseHeavyArsenalVolume()
        {
            int maxVolume = 20;
            if (_heavyArsenalVolume >= maxVolume) return;

            _heavyArsenalVolume += 1;

            SetHeavyArsenalVolume(_heavyArsenalVolume);
        }

        public void DecreaseHeavyArsenalVolume()
        {
            if (_heavyArsenalVolume == 0) return;

            _heavyArsenalVolume -= 1;

            SetHeavyArsenalVolume(_heavyArsenalVolume);
        }

        private void SetHeavyArsenalVolume(int volume)
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
        /// Calls the <seealso cref="OnHeavyArsenalVolumeIncreased"/> event, which only increases
        /// the volume of the sound effects of the weapon minigun.
        /// </summary>
        public static void CallHeavyArsenalVolumeIncreasedEvent()
        {
            OnHeavyArsenalVolumeIncreased?.Invoke();
        }

        /// <summary>
        /// Calls the <seealso cref="OnHeavyArsenalVolumeIncreased"/> event, which only lowers
        /// the volume of the sound effects of the weapon minigun.
        /// </summary>
        public static void CalHeavyArsenalVolumeDecreasedEvent()
        {
            OnHeavyArsenalVolumeDecreased?.Invoke();
        }

        /// <summary>
        /// Triggers the <seealso cref="OnSoundEffectChanged"/> event.
        /// That event plays the selected SoundEffectSO.
        /// </summary>
        /// <param name="soundEffect"></param>
        public static void CallOnSoundEffectChangedEvent(SoundEffectSO soundEffect, bool usePoolManager = true)
        {
            _CurrentSoundEffect = soundEffect;
            OnSoundEffectChanged?.Invoke(soundEffect, usePoolManager);
        }

        public void Load(GameData data)
        {
            SoundsVolume = data.GameAudioData.GetSoundsVolume();
            HeavyArsenalVolume = data.GameAudioData.GetHeavyArsenalVolume();
        }

        public void Save(GameData data)
        {
            data.GameAudioData.SoundsVolume = SoundsVolume;
            data.GameAudioData.HeavyArsenalVolume = HeavyArsenalVolume;
        }
    }
}