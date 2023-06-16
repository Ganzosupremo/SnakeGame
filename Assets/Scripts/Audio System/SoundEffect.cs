using UnityEngine;

namespace SnakeGame.AudioSystem
{
    /// <summary>
    /// The SFX's for the game, it populates the <see cref="audioSource"/> with the variables in the <seealso cref="SoundEffectSO"/> class.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    public class SoundEffect : MonoBehaviour
    {
        private AudioSource audioSource;

        public AudioSource AudioSource { get { return audioSource; } }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }

        private void OnDisable()
        {
            audioSource.Stop();
        }

        /// <summary>
        /// Sets The Sound That Will Be Played
        /// </summary>
        public void SetSound(SoundEffectSO soundEffect)
        {
            audioSource.pitch = Random.Range(soundEffect.soundEffectMinRandomValuePitch,
                soundEffect.soundEffectMaxRandomValuePitch);

            audioSource.volume = soundEffect.soundEffectVolume;

            audioSource.clip = soundEffect.soundEffectClip;
        }

        public void StopSound()
        {
            audioSource.Stop();
        }
    }
}