using UnityEngine;

namespace SnakeGame.AudioSystem
{
    /// <summary>
    /// The SFX's for the game, it populates the <see cref="_audioSource"/> with the variables in the <seealso cref="SoundEffectSO"/> class.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    public class SoundEffect : MonoBehaviour
    {
        private AudioSource _audioSource;

        public AudioSource AudioSource { get { return _audioSource; } }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            if (_audioSource != null)
            {
                _audioSource.Play();
            }
        }

        private void OnDisable()
        {
            if (_audioSource != null)
            {
                _audioSource.Stop();

            }
        }

        /// <summary>
        /// Sets The Sound That Will Be Played
        /// </summary>
        public void SetSound(SoundEffectSO soundEffect)
        {
            _audioSource.pitch = Random.Range(soundEffect.soundEffectMinRandomValuePitch,
                soundEffect.soundEffectMaxRandomValuePitch);

            _audioSource.volume = soundEffect.soundEffectVolume;

            _audioSource.clip = soundEffect.soundEffectClip;
        }

        public void StopSound()
        {
            if (_audioSource != null)
            {
                _audioSource.Stop();
            }
            gameObject.SetActive(false);
        }
    }
}