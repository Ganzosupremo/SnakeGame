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
        // On the final build change to a property
        [Range(0, 20)]
        public int MusicVolume = 9;

        public static event Action OnMusicVolumeIncreased;
        public static event Action OnMusicVolumeDecreased;
        public static event Action<MusicClipChangeEventArgs> OnMusicClipChanged;

        [SerializeField] private AudioSource _MusicSource01, _MusicSource02;
        private AudioClip m_CurrentmusicClip = null;
        [SerializeField] private bool m_IsMusicSource01Playing = true;

        private CancellationTokenSource m_CancellationTokenSource;
        private Coroutine _FadeMusicRoutine;

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

        private void MusicManager_OnMusicClipChanged(MusicClipChangeEventArgs args)
        {
            if (args.CanPlayMultipleClips)
                OnMusicChanged(args);
            else
                OnMusicChanged(args.Music, args.TimeToFade);
        }

        private void IncreaseMusicVolume()
        {
            int maxVolume = 20;

            if (MusicVolume >= maxVolume) return;

            MusicVolume += 1;
            SetMusicVolume(MusicVolume);
        }

        private void DecreaseMusicVolume()
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

        private async void OnMusicChanged(MusicClipChangeEventArgs args)
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
            if (CanPlayMultipleClips(musicSO))
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
        private static bool CanPlayMultipleClips(MusicSO musicSO)
        {
            if (musicSO.MusicClips == null)
                return false;
            else if (musicSO.MusicClips.Length <= 1)
                return false;
            else if (musicSO.MusicNames.Length <= 1)
                return false;
            else if (musicSO.MusicNames == null)
                return false;
            else if (musicSO.MusicClips.Length != musicSO.MusicNames.Length)
                return false;
            
            return true;
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
