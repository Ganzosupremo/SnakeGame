using Cysharp.Threading.Tasks;
using SnakeGame.GameUtilities;
using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace SnakeGame.AudioSystem
{
    [DisallowMultipleComponent]
    public class MusicManager : SingletonMonoBehaviour<MusicManager>, IPersistenceData
    {
        public int MusicVolume { get => _musicVolume; set => _musicVolume = value; }

        //public static event Action OnMusicVolumeIncreased;
        //public static event Action OnMusicVolumeDecreased;
        public static event Action<MusicClipChangeEventArgs> OnMusicClipChanged;
        public static event Action<float> OnMusicVolumeChanged;

        [SerializeField] private AudioSource _MusicSource01, _MusicSource02;
        private AudioClip m_CurrentmusicClip = null;
        private bool m_IsMusicSource01Playing = true;

        private CancellationTokenSource m_CancellationTokenSource;
        private int _musicVolume = 9;
        protected override void Awake()
        {
            base.Awake();
            m_CancellationTokenSource = new CancellationTokenSource();
        }

        private void Start()
        {
            SetMusicVolume(_musicVolume);
            m_IsMusicSource01Playing = true;
        }

        private void OnEnable()
        {
            OnMusicClipChanged += MusicManager_OnMusicClipChanged;
            OnMusicVolumeChanged += MusicVolumeChanged;

            if (_MusicSource02 == null)
            {
                _MusicSource02 = gameObject.AddComponent<AudioSource>();
                _MusicSource02.loop = true;
                _MusicSource02.outputAudioMixerGroup = GameResources.Instance.musicMixerGroup;
                _MusicSource02.enabled = true;
            }
        }

        private void MusicVolumeChanged(float musicVolume)
        {
            _musicVolume = (int)musicVolume;

            SetMusicVolume(_musicVolume);
        }

        private void OnDisable()
        {
            m_CancellationTokenSource.Cancel();
            OnMusicClipChanged -= MusicManager_OnMusicClipChanged;
            OnMusicVolumeChanged -= MusicVolumeChanged;
        }

        private void OnDestroy()
        {
            m_CancellationTokenSource.Dispose();
        }

        #region Increase/Decrease Volume Methods

        //private void MusicManager_OnMusicVolumeIncreased()
        //{
        //    IncreaseMusicVolume();
        //}

        //private void MusicManager_OnMusicVolumeDecreased()
        //{
        //    DecreaseMusicVolume();
        //}

        //private void IncreaseMusicVolume()
        //{
        //    int maxVolume = 20;

        //    if (_musicVolume >= maxVolume) return;

        //    _musicVolume += 1;
        //    SetMusicVolume(_musicVolume);
        //}

        //private void DecreaseMusicVolume()
        //{
        //    if (_musicVolume == 0) return;

        //    _musicVolume -= 1;
        //    SetMusicVolume(_musicVolume);
        //}

        #endregion

        private async void MusicManager_OnMusicClipChanged(MusicClipChangeEventArgs args)
        {
            if (args.CanPlayMultipleClips)
                await OnMusicChanged(args);
            else
                await OnMusicChanged(args.Music, args.TimeToFade);
        }

        private async UniTask OnMusicChanged(MusicSO musicSO, float timeToFade)
        {
            // If the audio clip changed, play the new audio
            if (m_CurrentmusicClip != musicSO.musicClip)
            {
                m_CurrentmusicClip = musicSO.musicClip;
                m_IsMusicSource01Playing = !m_IsMusicSource01Playing;

                await FadeMusicAsync(musicSO, timeToFade, m_CancellationTokenSource.Token);
            }
        }

        private async UniTask OnMusicChanged(MusicClipChangeEventArgs args)
        {
            if (m_CurrentmusicClip != args.RandomClip)
            {
                m_CurrentmusicClip = args.RandomClip;
                m_IsMusicSource01Playing = !m_IsMusicSource01Playing;

                 await FadeMusicAsync(args, m_CancellationTokenSource.Token);
            }
        }

        /// <summary>
        /// Fades out the current music and fades in the new specified music, works with one audio clip only
        /// </summary>
        /// <param name="musicTrack">The new Music to fade in</param>
        /// <param name="timeToFade">The time it takes to fade</param>
        private async UniTask FadeMusicAsync(MusicSO musicTrack, float timeToFade, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested || !gameObject.activeSelf) return;

            try
            {
                float timeElapsed = 0f;
                if (m_IsMusicSource01Playing)
                {
                    _MusicSource01.clip = null;
                    _MusicSource01.clip = musicTrack.musicClip;
                    _MusicSource01.Play();

                    while (timeElapsed < timeToFade)
                    {
                        timeElapsed += Time.deltaTime;
                        _MusicSource01.volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
                        _MusicSource02.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);     
                        await UniTask.Yield(cancellationToken);
                    }
                    _MusicSource02.Pause();
                }
                else
                {
                    _MusicSource02.clip = null;
                    _MusicSource02.clip = musicTrack.musicClip;
                    _MusicSource02.Play();

                    while (timeElapsed < timeToFade)
                    {
                        timeElapsed += Time.deltaTime;
                        _MusicSource02.volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
                        _MusicSource01.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);
                        await UniTask.Yield(cancellationToken);
                    
                    }
                    _MusicSource01.Pause();
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        /// <summary>
        /// Fades out the current music and fades in the new specified music, uses the randomly selected clip
        /// </summary>
        /// <param name="clip">The random selected clip</param>
        /// <param name="timeToFade">The time it takes to fade</param>
        private async UniTask FadeMusicAsync(MusicClipChangeEventArgs args, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested || !gameObject.activeSelf) return;

            try
            {
                float timeElapsed = 0f;
                if (m_IsMusicSource01Playing)
                {
                    _MusicSource01.clip = null;
                    _MusicSource01.clip = args.RandomClip;

                    _MusicSource01.Play();

                    while (timeElapsed <= args.TimeToFade)
                    {
                        timeElapsed += Time.deltaTime;
                        _MusicSource01.volume = Mathf.LerpUnclamped(0, 1, timeElapsed / args.TimeToFade);
                        _MusicSource02.volume = Mathf.LerpUnclamped(1, 0, timeElapsed / args.TimeToFade);
                        await UniTask.Yield(cancellationToken);
                    }
                    _MusicSource02.Pause();
                }
                else
                {
                    _MusicSource02.clip = null;
                    _MusicSource02.clip = args.RandomClip;

                    _MusicSource02.Play();

                    while (timeElapsed <= args.TimeToFade)
                    {
                        timeElapsed += Time.deltaTime;
                        _MusicSource02.volume = Mathf.LerpUnclamped(0, 1, timeElapsed / args.TimeToFade);
                        _MusicSource01.volume = Mathf.LerpUnclamped(1, 0, timeElapsed / args.TimeToFade);
                        await UniTask.Yield(cancellationToken);
                    }
                    _MusicSource01.Pause();
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        private void SetMusicVolume(int musicVolume)
        {
            float muteDecibels = -80f;

            if (musicVolume == 0)
                GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("MusicVolume", muteDecibels);
            else
                GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("MusicVolume", HelperUtilities.LinearToDecibels(musicVolume));
        }

        /// <summary>
        /// Calls the <seealso cref="OnMusicVolumeIncreased"/> event, which increases
        /// the volume of the currently played music.
        /// </summary>
        //public static void CallOnMusicVolumeIncreasedEvent()
        //{
        //    OnMusicVolumeIncreased?.Invoke();
        //}

        public static void CallOnMusicVolumeChangedEvent(float volume)
        {
            OnMusicVolumeChanged?.Invoke(volume);
        }


        //public static void CallOnMusicVolumeDecreasedEvent()
        //{
        //    OnMusicVolumeDecreased?.Invoke();
        //}

        /// <summary>
        /// Changes the currently playing music to the one specified.
        /// </summary>
        public static void CallOnMusicClipChangedEvent(MusicSO musicSO, float timeToFade = Settings.MusicFadeTime)
        {
            if (HasMultipleClips(musicSO))
            {
                int randomInt = UnityEngine.Random.Range(0, musicSO.MusicClips.Length);
                AudioClip randomClip = musicSO.MusicClips[randomInt];
                string nameRandomClip = musicSO.MusicNames[randomInt];

                MusicClipChangeEventArgs args = new()
                {
                    Index = randomInt,
                    Music = musicSO,
                    TimeToFade = timeToFade,

                    CanPlayMultipleClips = true,
                    RandomClip = randomClip,
                    NameRandomClip = nameRandomClip
                };

                OnMusicClipChanged?.Invoke(args);
            }
            else
            {
                OnMusicClipChanged?.Invoke(new MusicClipChangeEventArgs()
                {
                    Music = musicSO,
                    TimeToFade = timeToFade,
                    CanPlayMultipleClips = false
                });
            }
        }

        /// <summary>
        /// Checks if the specified MusicSO has the necessary arrays populated to play multiple clips.
        /// </summary>
        /// <param name="musicSO">The MusicSO to check</param>
        /// <returns>True if the requirements are met, false otherwise</returns>
        private static bool HasMultipleClips(MusicSO musicSO)
        {
            if (musicSO.MusicClips == null)
                return false;
            else if (musicSO.MusicClips.Length <= 1)
                return false;
            else if (musicSO.MusicNames.Length <= 1)
                return false;
            else if (musicSO.MusicNames == null)
                return false;
            
            return true;
        }

        public void Load(GameData data)
        {
            _musicVolume = data.GameAudioData.MusicVolume;
        }

        public void Save(GameData data)
        {
            data.GameAudioData.MusicVolume = _musicVolume;
        }

        public class MusicClipChangeEventArgs : EventArgs
        {
            public int Index;
            public MusicSO Music;
            public float TimeToFade;

            public bool CanPlayMultipleClips;
            public AudioClip RandomClip;
            public string NameRandomClip;
        }
    }
}
