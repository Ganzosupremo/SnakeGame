using UnityEngine;

namespace SnakeGame.VisualEffects
{
    [DisallowMultipleComponent]
    public class SpecialAbilityEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem effectParticles;

        public void EnableParticles()
        {
            if (!effectParticles.isPlaying)
            {
                effectParticles.Play();
            }
        }

        public void DisableParticles()
        {
            if (effectParticles.isPlaying)
            {
                effectParticles.Stop();
            }
        }
    }
}
