using Cysharp.Threading.Tasks;
using SnakeGame.GameUtilities;
using SnakeGame.Interfaces;
using SnakeGame.PlayerSystem.AbilitySystem;
using SnakeGame.SaveAndLoadSystem;
using System;
using System.Collections;
using UnityEngine;

namespace SnakeGame.AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    public class MusicManager : SingletonMonoBehaviour<MusicManager>, IPersistenceData
    {
        // On the final build change to a property
        [Range(0, 20)]
        public int MusicVolume = 9;

        public static event Action OnMusicVolumeIncreased;
        public static event Action OnMusicVolumeDecreased;
        public static event Action<MusicSO, float> OnMusicClipChanged;

        [SerializeField] private AudioSource _MusicSource01 = null, _MusicSource02 = null;
        private AudioClip m_CurrentmusicClip = null;
        private bool m_IsMusicSource01Playing = true;

        // On the final build use this int internally and the property will fetch the value
        //private int m_MusicVolume;
        private Coroutine m_FadeMusicCoroutine;

        protected override void Awake()
        {
            base.Awake();

            _MusicSource01 = GetComponent<AudioSource>();
            //_MusicSource02 = gameObject.AddComponent<AudioSource>();
            //_MusicSource02.loop = true;
            //_MusicSource02.outputAudioMixerGroup = GameResources.Instance.musicMixerGroup;
            
            // Start with the music off
            //GameResources.Instance.musicOff.TransitionTo(0f);
        }

        private void Start()
        {
            SetMusicVolume(MusicVolume);
            m_IsMusicSource01Playing = true;
        }

        private void OnEnable()
        {
            OnMusicVolumeIncreased += MusicManager_OnMusicVolumeIncreased;
            OnMusicVolumeDecreased += MusicManager_OnMusicVolumeDecreased;
            OnMusicClipChanged += MusicManager_OnMusicClipChanged;

            if (_MusicSource02 == null)
            {
                _MusicSource02 = gameObject.AddComponent<AudioSource>();
                _MusicSource02.loop = true;
                _MusicSource02.outputAudioMixerGroup = GameResources.Instance.musicMixerGroup;
            }


            //SnakeAbilityManager.OnAbilityActive += SnakeAbilityManager_OnAbilityActive;
            //SnakeAbilityManager.OnAbilityInactive += SnakeAbilityManager_OnAbilityInactive;
        }

        private void OnDisable()
        {
            OnMusicVolumeIncreased -= MusicManager_OnMusicVolumeIncreased;
            OnMusicVolumeDecreased -= MusicManager_OnMusicVolumeDecreased;
            OnMusicClipChanged -= MusicManager_OnMusicClipChanged;

            //SnakeAbilityManager.OnAbilityActive -= SnakeAbilityManager_OnAbilityActive;
            //SnakeAbilityManager.OnAbilityInactive -= SnakeAbilityManager_OnAbilityInactive;
        }

        private void MusicManager_OnMusicVolumeIncreased()
        {
            IncreaseMusicVolume();
        }

        private void MusicManager_OnMusicVolumeDecreased()
        {
            DecreaseMusicVolume();
        }

        private void MusicManager_OnMusicClipChanged(MusicSO musicSO, float timeToFade = Settings.MusicFadeTime)
        {
            OnMusicChanged(musicSO, timeToFade);
        }

        [ContextMenu("Increase Volume")]
        public void IncreaseMusicVolume()
        {
            int maxVolume = 20;

            if (MusicVolume >= maxVolume) return;

            MusicVolume += 1;
            SetMusicVolume(MusicVolume);
        }

        [ContextMenu("Decrease Volume")]
        public void DecreaseMusicVolume()
        {
            if (MusicVolume == 0) return;

            MusicVolume -= 1;
            SetMusicVolume(MusicVolume);
        }

        private void OnMusicChanged(MusicSO musicSO, float timeToFade)
        {
            if (m_FadeMusicCoroutine != null)
                StopCoroutine(m_FadeMusicCoroutine);

            // If the audio clip changed, play the new audio
            if (musicSO.musicClip != m_CurrentmusicClip)
            {
                m_CurrentmusicClip = musicSO.musicClip;
                m_IsMusicSource01Playing = !m_IsMusicSource01Playing;

                FadeMusic(musicSO, timeToFade);
            }
        }

        private async void FadeMusic(MusicSO musicTrack, float timeToFade)
        {
            float timeElapsed = 0f;
            if (m_IsMusicSource01Playing)
            {
                _MusicSource02.clip = musicTrack.musicClip;
                _MusicSource02.Play();

                while (timeElapsed < timeToFade)
                {
                    _MusicSource02.volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
                    _MusicSource01.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);
                    timeElapsed += Time.deltaTime;
                    await UniTask.Yield();
                }
                _MusicSource01.Pause();
            }
            else
            {
                _MusicSource01.clip = musicTrack.musicClip;
                _MusicSource01.Play();

                while (timeElapsed < timeToFade)
                {
                    _MusicSource01.volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
                    _MusicSource02.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);
                    timeElapsed += Time.deltaTime;
                    await UniTask.Yield();
                }
                _MusicSource02.Pause();
            }
        }

        private void SetMusicVolume(int musicVolume)
        {
            float muteDecibels = -80f;

            if (musicVolume == 0)
            {
                GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("MusicVolume", muteDecibels);
            }
            else
            {
                GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("MusicVolume", HelperUtilities.LinearToDecibels(musicVolume));
            }
        }

        /// <summary>
        /// Calls the <seealso cref="OnMusicVolumeIncreased"/> event, which increases
        /// the volume of the currently played music.
        /// </summary>
        public static void CallOnMusicVolumeIncreasedEvent()
        {
            OnMusicVolumeIncreased?.Invoke();
        }

        /// <summary>
        /// Calls the <seealso cref="OnMusicVolumeDecreased"/> event, which decreases
        /// the volume of the currently played music.
        /// </summary>
        public static void CallOnMusicVolumeDecreasedEvent()
        {
            OnMusicVolumeDecreased?.Invoke();
        }

        public static void CallOnMusicClipChangedEvent(MusicSO musicSO, float timeToFade = Settings.MusicFadeTime)
        {
            OnMusicClipChanged?.Invoke(musicSO, timeToFade);
        }

        public void Load(GameData data)
        {
            MusicVolume = data.MusicVolume;
        }

        public void Save(GameData data)
        {
            data.MusicVolume = MusicVolume;
        }
    }
}