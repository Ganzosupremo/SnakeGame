using SnakeGame.GameUtilities;
using UnityEngine;

namespace SnakeGame.VisualEffects
{
    [CreateAssetMenu(fileName = "AmmoHitEffect_", menuName = "Scriptable Objects/Weapon System/Ammo Hit Effect")]
    public class AmmoHitEffectSO : ScriptableObject
    {
        #region Header Bullet Hit Effect Details
        [Space(10)]
        [Header("Bullet Hit Effect Details")]
        #endregion

        #region Tooltip
        [Tooltip("The color gradient for the hit effect.  This gradient show the color of particles during their lifetime - from left to right ")]
        #endregion Tooltip
        public Gradient colorGradient;

        #region Tooltip
        [Tooltip("Can Define the shape where the particles come from and also define the Arc Mode in the Shape Modul.")]
        #endregion Tooltip
        public EffectType Type = EffectType.None;

        #region Tooltip
        [Tooltip("The length of time the particle system is emitting particles")]
        [Range(0f, 1f)]
        #endregion Tooltip
        public float particleDuration = 0.5f;

        #region Tooltip
        [Tooltip("The start particle size for the particle effect")]
        [Range(0.01f, 2f)]
        #endregion Tooltip
        public float startParticleSize = 0.2f;

        #region Tooltip
        [Tooltip("The start particle speed for the particle effect")]
        [Range (1f, 20f)]
        #endregion Tooltip
        public float startParticleSpeed = 3f;

        #region Tooltip
        [Tooltip("The particle lifetime for the particle effect")]
        [Range(0.01f, 1f)]
        #endregion Tooltip
        public float startLifetime = 0.5f;

        #region Tooltip
        [Tooltip("The maximum number of particles to be emitted")]
        [Range(1, 1000)]
        #endregion Tooltip
        public int maxParticles = 100;

        #region Tooltip
        [Tooltip("The number of particles emitted per second. " +
            "If zero it will just be the burst number")]
        [Range(0, 1000)]
        #endregion Tooltip
        public int emissionRate = 100;

        #region Tooltip
        [Tooltip("How many particles should be emmitted in the particle effect burst")]
        [Range(0, 1000)]
        #endregion Tooltip
        public int burstNumber = 30;

        #region Tooltip
        [Tooltip("The gravity on the particles - a small negative number will make them float up")]
        [Range(-5f, 5f)]
        #endregion
        public float gravityEffect = -0.01f;

        #region Tooltip
        [Tooltip("The sprite for the particle effect.  If none is specified then the default particle sprite will be used")]
        #endregion Tooltip
        public Sprite sprite;

        #region Tooltip
        [Tooltip("The min velocity for the particle over its lifetime. A random value between min and max will be generated.")]
        #endregion Tooltip
        public Vector3 minVelocityOverLifetime;

        #region Tooltip
        [Tooltip("The max velocity for the particle over its lifetime. A random value between min and max will be generated.")]
        #endregion Tooltip
        public Vector3 maxVelocityOverLifetime;

        #region Tooltip
        [Tooltip("ammoHitEffectPrefab contains the particle system for the shoot effect - and is configured by the weaponShootEffect SO")]
        #endregion Tooltip
        public GameObject ammoHitEffectPrefab;
        
        #region Shape Module
        [Header("Shape Module")]

        #region Tooltip
        [Tooltip("Radius of the shape")]
        [Range(1f, 10f)]
        #endregion
        public float Radius = 1f;

        #region Tooltip
        [Tooltip("Angle of the cone. Only works with Cone_Upwards")]
        [Range(0f, 90f)]
        #endregion
        public float Angle = 25f;

        #region Tooltip
        [Tooltip("Controls the thickness of the spawn volume.")]
        [Range(0f, 1f)]
        #endregion
        public float RadiusThickness = 0f;

        #region Tooltip
        [Tooltip("Particles are spawned around the arc.")]
        [Range(0f, 360f)]
        #endregion
        public float Arc = 360f;

        #region Tooltip
        [Tooltip("Controls how particles are spawned around the arc")]
        #endregion
        public ParticleSystemShapeMultiModeValue Mode = ParticleSystemShapeMultiModeValue.Random;

        #region Tooltip
        [Tooltip("Spawns particles only at specific angles around the arc. 0 to disable.")]
        [Range(0f, 1f)]
        #endregion Tooltip
        public float Spread = 0f;

        #region Tooltip
        [Tooltip("The speed at which the particles will loop if the mode Loop or Ping Pong is selected.")]
        [Range(0f, 10f)]
        #endregion Tooltip
        public float Speed = 1f;
        #endregion

        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(particleDuration), particleDuration, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSize), startParticleSize, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSpeed), startParticleSpeed, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(startLifetime), startLifetime, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(maxParticles), maxParticles, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(emissionRate), emissionRate, true);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(burstNumber), burstNumber, true);
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHitEffectPrefab), ammoHitEffectPrefab);
        }
#endif
        #endregion Validation
    }
}