using SnakeGame.VisualEffects;
using UnityEngine;

namespace SnakeGame
{
    public class HideVarTest : MonoBehaviour
    {
        public enum Test
        {
            Random,
            Loop,
            PingPong,
            BurstSpread,
            Whirpool,
            None,
        }

        public Test test = Test.None;

        // Always serialize these 

        #region Tooltip
        [Tooltip("The color gradient for the hit effect. This gradient show the color of particles during their lifetime - from left to right")]
        #endregion Tooltip
        public Gradient ColorGradient;

        #region Tooltip
        [Tooltip("Defines the shape that the particles will emmit from." +
            " Depending on the type selected some values in the shape module will change.")]
        #endregion Tooltip
        public EffectType Type = EffectType.None;

        #region Tooltip
        [Tooltip("The length of time the particle system is emitting particles")]
        [Range(0.01f, 1f)]
        #endregion Tooltip
        public float ParticleDuration = 0.5f;

        #region Tooltip
        [Tooltip("The start particle size for the particle effect")]
        [Range(0.1f, 10f)]
        #endregion Tooltip
        public float StartParticleSize = 0.2f;

        #region Tooltip
        [Tooltip("The start particle speed for the particle effect")]
        [Range(1f, 20f)]
        #endregion Tooltip
        public float StartParticleSpeed = 3f;

        #region Tooltip
        [Tooltip("The particle lifetime for the particle effect")]
        [Range(0.01f, 1f)]
        #endregion Tooltip
        public float StartLifetime = 0.5f;

        #region Tooltip
        [Tooltip("The maximum number of particles to be emitted")]
        [Range(10, 1000)]
        #endregion Tooltip
        public int MaxParticles = 100;

        #region Tooltip
        [Tooltip("The number of particles emitted per second. If zero it will just be the burst number")]
        [Range(0, 500)]
        #endregion Tooltip
        [SerializeField] public int EmissionRate = 100;

        #region Tooltip
        [Tooltip("How many particles should be emmitted in the particle effect burst")]
        [Range(0, 500)]
        #endregion Tooltip
        [SerializeField]
        public int BurstNumber = 30;

        // -----------------------


        // These will always serialize in the inspector
        [SerializeField]
        [Range(0.1f, 5f)]
        private float Radius;
        [SerializeField]
        [Range(0f, 1f)]
        private float RadiusThickness;
        [SerializeField]
        [Range(0f, 1f)]
        private float Spread;

        // These will serialize depending on the enum type selected
        [SerializeField]
        [Range(0f, 90f)]
        private float Angle;
        [SerializeField]
        [Range(1f, 10f)]
        private float Speed;
    }
}
