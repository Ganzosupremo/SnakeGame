using Cysharp.Threading.Tasks;
using SnakeGame.GameUtilities;
using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;
using System;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace SnakeGame.AudioSystem
{
    [DisallowMultipleComponent]
    public class MusicManager : SingletonMonoBehaviour<MusicManager>, IPersistenceData
    {
        // On the final build change to a property
        [Range(0, 20)]
        public int MusicVolume = 9;

        public static event Action OnMusicVolumeIncreased;
        public static event Action OnMusicVolumeDecreased;
        public static event Action<MusicSO, float> OnMusicClipChanged;

        [SerializeField] private AudioSource _MusicSource01, _MusicSource02;
        private AudioClip m_CurrentmusicClip = null;
        [SerializeField] private bool m_IsMusicSource01Playing = true;

        private CancellationTokenSource m_CancellationTokenSource;

        // On the final build use this int internally and the property will fetch the value
        //private int m_MusicVolume;

        protected override void Awake()
        {
            base.Awake();
            m_CancellationTokenSource = new CancellationTokenSource();
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
                _MusicSource02.enabled = true;
            }
        }

        private void OnDisable()
        {
            m_CancellationTokenSource.Cancel();
            OnMusicVolumeIncreased -= MusicManager_OnMusicVolumeIncreased;
            OnMusicVolumeDecreased -= MusicManager_OnMusicVolumeDecreased;
            OnMusicClipChanged -= MusicManager_OnMusicClipChanged;
        }

        private void OnDestroy()
        {
            m_CancellationTokenSource.Dispose();
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

        private async void OnMusicChanged(MusicSO musicSO, float timeToFade)
        {
            // If the audio clip changed, play the new audio
            if (m_CurrentmusicClip != musicSO.musicClip)
            {
                m_CurrentmusicClip = musicSO.musicClip;
                m_IsMusicSource01Playing = !m_IsMusicSource01Playing;

                await FadeMusicAsync(musicSO, timeToFade, m_CancellationTokenSource.Token);
            }
        }

        private async UniTask FadeMusicAsync(MusicSO musicTrack, float timeToFade, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested || !gameObject.activeSelf) return;

            try
            {
                float timeElapsed = 0f;
                if (m_IsMusicSource01Playing)
                {

                    _MusicSource01.clip = musicTrack.musicClip;
                    _MusicSource02.Pause();
                    _MusicSource01.Play();

                    while (timeElapsed < timeToFade)
                    {
                        timeElapsed += Time.deltaTime;
                        _MusicSource01.volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
                        _MusicSource02.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);     
                        await UniTask.NextFrame(cancellationToken);
                    }
                }
                else
                {
                    _MusicSource02.clip = musicTrack.musicClip;
                    _MusicSource01.Pause();
                    _MusicSource02.Play();

                    while (timeElapsed < timeToFade)
                    {
                        timeElapsed += Time.deltaTime;
                        _MusicSource02.volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
                        _MusicSource01.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);
                        await UniTask.NextFrame(cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        //private async UniTask FadeInNewMusic(MusicSO music, float timeToFade, CancellationToken token)
        //{
        //    float timeElapsed = 0f;

        //    while (timeElapsed <= timeToFade)
        //    {
        //        _MusicSource01.clip = music.musicClip;
        //        _MusicSource01.Play();

        //        timeElapsed += Time.deltaTime;
        //        _MusicSource01.volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
        //        //_MusicSource02.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);
        //        await UniTask.NextFrame(token);
        //    }
        //}

        //private async UniTask FadeOutCurrentMusic(float timeToFade, CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        float timeElapsed = 0f;

        //        while (timeElapsed <= timeToFade)
        //        {
        //            timeElapsed += Time.deltaTime;
        //            _MusicSource01.volume = Mathf.Lerp(1f, 0f, timeElapsed / timeToFade);
        //            //_MusicSource02.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);
        //            await UniTask.NextFrame(cancellationToken);
        //        }
        //        _MusicSource01.Pause();
        //        m_CurrentmusicClip = null;
        //    }
        //    catch (OperationCanceledException)
        //    {
        //        return;
        //    }
        //}

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
            MusicVolume = data.VolumeDataSaved.MusicVolume;
        }

        public void Save(GameData data)
        {
            data.VolumeDataSaved.MusicVolume = MusicVolume;
        }
    }
}
