using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;
using System;
using System.Collections;
using UnityEngine;

namespace SnakeGame.SoundsSystem
{
    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    public class MusicManager : SingletonMonoBehaviour<MusicManager>, IPersistenceData
    {
        private AudioSource musicSource = null;
        private AudioClip currentmusicClip = null;

        private Coroutine fadeOutMusicCoroutine;
        private Coroutine fadeInMusicCoroutine;

        [Range(0, 20)]
        public int musicVolume;

        protected override void Awake()
        {
            base.Awake();

            musicSource = GetComponent<AudioSource>();

            // Start with the music off
            GameResources.Instance.musicOff.TransitionTo(0f);
        }

        private void Start()
        {
            // Replace later with the save and load system
            //if (PlayerPrefs.HasKey(nameof(musicVolume)))
            //    musicVolume = PlayerPrefs.GetInt(nameof(musicVolume));

            SetMusicVolume(musicVolume);
        }

        private void OnDisable()
        {
            //PlayerPrefs.SetInt(nameof(musicVolume), musicVolume);
        }

        /// <summary>
        /// Plays the selected music
        /// </summary>
        /// <param name="musicTrackSO">The music track to play</param>
        /// <param name="fadeOutTime">The time it takes to stop the other music</param>
        /// <param name="fadeInTime">The time it takes to the selected music to start playing</param>
        public void PlayMusic(MusicSO musicSO, float fadeOutTime = Settings.musicFadeOutTime, float fadeInTime = Settings.musicFadeInTime)
        {
            StartCoroutine(PlayMusicRoutine(musicSO, fadeOutTime, fadeInTime));
        }

        [ContextMenu("Increase Volume")]
        public void IncreaseMusicVolume()
        {
            int maxVolume = 20;

            if (musicVolume >= maxVolume) return;

            musicVolume += 1;
            SetMusicVolume(musicVolume);
        }

        [ContextMenu("Decrease Volume")]
        public void DecreaseMusicVolume()
        {
            if (musicVolume == 0) return;

            musicVolume -= 1;
            SetMusicVolume(musicVolume);
        }

        private IEnumerator PlayMusicRoutine(MusicSO musicSO, float fadeOutTime, float fadeInTime)
        {
            if (fadeOutMusicCoroutine != null)
                StopCoroutine(fadeOutMusicCoroutine);

            if (fadeInMusicCoroutine != null)
                StopCoroutine(fadeInMusicCoroutine);

            // If the clip changed, play the new clip
            if (musicSO.musicClip != currentmusicClip)
            {
                currentmusicClip = musicSO.musicClip;

                yield return fadeOutMusicCoroutine = StartCoroutine(FadeOutMusic(fadeOutTime));

                yield return fadeInMusicCoroutine = StartCoroutine(FadeInMusic(musicSO, fadeInTime));
            }
        }

        private IEnumerator FadeOutMusic(float fadeOutTime)
        {
            GameResources.Instance.musicOnLow.TransitionTo(fadeOutTime);

            yield return new WaitForSeconds(fadeOutTime);
        }

        private IEnumerator FadeInMusic(MusicSO musicSO, float fadeInTime)
        {
            // Set the clip and play it
            musicSource.clip = musicSO.musicClip;
            musicSource.volume = musicSO.musicVolume;
            musicSource.Play();

            GameResources.Instance.musicOnFull.TransitionTo(fadeInTime);

            yield return new WaitForSeconds(fadeInTime);
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

        public void Load(GameData data)
        {
            musicVolume = data.musicVolume;
        }

        public void Save(GameData data)
        {
            data.musicVolume = musicVolume;
        }
    }
}
