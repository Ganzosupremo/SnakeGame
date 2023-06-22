using SnakeGame.GameUtilities;
using UnityEditor;
using UnityEditor.Rendering.PostProcessing;
using UnityEngine;

namespace SnakeGame.VisualEffects
{
    [CreateAssetMenu(fileName = "DeathEffect_", menuName = "Scriptable Objects/VFXs/Death Effect")]
    public class DeathEffectSO : ScriptableObject
    {
        #region Main Module
        [Header("Main Module")]
        [Space(10)]

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
        public int EmissionRate = 100;

        #region Tooltip
        [Tooltip("How many particles should be emmitted in the particle effect burst")]
        [Range(0, 500)]
        #endregion Tooltip
        public int BurstNumber = 30;

        #region Tooltip
        [Tooltip("The gravity on the particles")]
        #endregion
        [Range(-9.81f, 9.81f)]
        public float MinGravityEffect = 0.01f;
        [Range(-9.81f, 9.81f)]
        public float MaxGravityEffect = 1f;

        #region Tooltip
        [Tooltip("The sprite for the particle effect.  If none is specified then the default particle sprite will be used")]
        #endregion Tooltip
        public Sprite[] SpriteArray;

        #region Tooltip
        [Tooltip("The min velocity for the particle over its lifetime. A random value between min and max will be generated.")]
        #endregion Tooltip
        public Vector3 MinVelocityOverLifetime;
        #region Tooltip
        [Tooltip("The max velocity for the particle over its lifetime. A random value between min and max will be generated.")]
        #endregion Tooltip
        public Vector3 MaxVelocityOverLifetime;

        #region Tooltip
        [Tooltip("DeathEffectPrefab contains the particle system for the death effect and is configured by this Scriptable Object")]
        #endregion Tooltip
        public GameObject DeathEffectPrefab;
        #endregion

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
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ParticleDuration), ParticleDuration, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(StartParticleSize), StartParticleSize, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(StartParticleSpeed), StartParticleSpeed, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(StartLifetime), StartLifetime, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(MaxParticles), MaxParticles, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(EmissionRate), EmissionRate, true);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(BurstNumber), BurstNumber, true);
            HelperUtilities.ValidateCheckNullValue(this, nameof(DeathEffectPrefab), DeathEffectPrefab);
        }

#endif
        #endregion Validation

        #region Preview
#if UNITY_EDITOR
        [SerializeField] private GameObject _PreviewParticleSystem;

        public void PreviewParticleSystem()
        {
            ParticleSystem source = _PreviewParticleSystem.GetComponent<ParticleSystem>(); ;

            source = Init(source);

            Instantiate(source);
        }

        private ParticleSystem Init(ParticleSystem source = null)
        {
            SetColorGradient(ColorGradient, source);

            SetMainModule(ParticleDuration, StartParticleSize, 
                StartParticleSpeed, StartLifetime, Random.Range(MinGravityEffect, MaxGravityEffect), MaxParticles, source);

            SetShapeModule(source, Type);

            SetParticleEmissionModule(EmissionRate, BurstNumber, source);

            SetAnimationModule(source, SpriteArray);

            SetVelocityOverLifeTimeModule(MinVelocityOverLifetime, MaxVelocityOverLifetime, source);

            source.Play();

            return source;
        }

        /// <summary>
        /// Set The Particle System Color Gradient
        /// </summary>
        private void SetColorGradient(Gradient colorGradient, ParticleSystem source)
        {
            ParticleSystem.ColorOverLifetimeModule colorOverLifetime = source.colorOverLifetime;
            colorOverLifetime.enabled = true;
            colorOverLifetime.color = colorGradient;
        }

        /// <summary>
        /// Set the particle system starting values
        /// </summary>
        private void SetMainModule(float particleDuration, float startParticleSize, float startParticleSpeed, float startLifetime, float gravityEffect, int maxParticles, ParticleSystem source)
        {
            ParticleSystem.MainModule mainModule = source.main;

            // Set particle system duration
            mainModule.duration = particleDuration;

            // Set particle start size
            mainModule.startSize = startParticleSize;

            // Set particle start speed
            mainModule.startSpeed = startParticleSpeed;

            // Set particle start lifetime
            mainModule.startLifetime = startLifetime;

            // Set particle starting gravity
            mainModule.gravityModifier = gravityEffect;

            // Set max particles
            mainModule.maxParticles = maxParticles;
        }

        private void SetShapeModule(ParticleSystem source, EffectType effectType = EffectType.None)
        {
            ParticleSystem.ShapeModule shapeModule = source.shape;

            source.gameObject.transform.localRotation = Quaternion.identity;

            switch (effectType)
            {
                case EffectType.Sphere_Burst:

                    shapeModule.shapeType = ParticleSystemShapeType.Sphere;
                    shapeModule.radius = Radius;
                    shapeModule.radiusThickness = RadiusThickness;
                    shapeModule.arc = Arc;
                    shapeModule.arcMode = Mode;
                    shapeModule.arcSpeed = Speed;
                    shapeModule.arcSpread = Spread;

                    break;
                case EffectType.Circle_Explosion:

                    shapeModule.shapeType = ParticleSystemShapeType.Circle;
                    shapeModule.radius = Radius;
                    shapeModule.radiusThickness = RadiusThickness;
                    shapeModule.arc = Arc;
                    shapeModule.arcMode = Mode;
                    shapeModule.arcSpeed = Speed;
                    shapeModule.arcSpread = Spread;

                    break;
                case EffectType.Cone_Upwards:

                    float xRotation = -0.7071f;
                    source.gameObject.transform.localRotation = new Quaternion(xRotation, 0f, 0f, 0.7071f);

                    shapeModule.shapeType = ParticleSystemShapeType.Cone;
                    shapeModule.radius = Radius;
                    shapeModule.angle = Angle;
                    shapeModule.radiusThickness = RadiusThickness;
                    shapeModule.arc = Arc;
                    shapeModule.arcMode = Mode;
                    shapeModule.arcSpeed = Speed;
                    shapeModule.arcSpread = Spread;

                    break;
                case EffectType.Circle_Whirpool:

                    shapeModule.shapeType = ParticleSystemShapeType.Circle;
                    shapeModule.radius = Radius;
                    shapeModule.radiusThickness = RadiusThickness;
                    shapeModule.arc = Arc;
                    shapeModule.arcMode = Mode;
                    shapeModule.arcSpeed = Speed;
                    shapeModule.arcSpread = Spread;

                    break;
                case EffectType.None:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Set the particle system's particle burst and number of particles
        /// </summary>
        private void SetParticleEmissionModule(int emissionRate, int burstNumber, ParticleSystem source)
        {
            ParticleSystem.EmissionModule emissionModule = source.emission;

            ParticleSystem.MinMaxCurve minMaxCurve = new(burstNumber, MaxParticles);

            // Set particle burst number
            ParticleSystem.Burst burst = new(0, minMaxCurve);
            emissionModule.SetBurst(0, burst);

            // Set particle emission rate
            emissionModule.rateOverTime = emissionRate;
        }

        /// <summary>
        /// Set the particle system sprite
        /// </summary>
        private void SetAnimationModule(ParticleSystem source, params Sprite[] sprites)
        {
            ParticleSystem.TextureSheetAnimationModule animationModule = source.textureSheetAnimation;
            animationModule.enabled = true;
            animationModule.SetSprite(0, sprites[0]);

            foreach (Sprite sprite in sprites)
            {
                animationModule.AddSprite(sprite);
            }
        }

        /// <summary>
        /// Set the velocity over lifetime
        /// </summary>
        private void SetVelocityOverLifeTimeModule(Vector3 minVelocityOverLifetime, Vector3 maxVelocityOverLifetime, ParticleSystem source)
        {
            ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = source.velocityOverLifetime;
            velocityOverLifetime.enabled = true;

            // Define min, max X velocity
            ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.TwoConstants,
                constantMin = minVelocityOverLifetime.x,
                constantMax = maxVelocityOverLifetime.x
            };
            velocityOverLifetime.x = minMaxCurveX;

            // Define min, max Y velocity
            ParticleSystem.MinMaxCurve minMaxCurveY = new()
            {
                mode = ParticleSystemCurveMode.TwoConstants,
                constantMin = minVelocityOverLifetime.y,
                constantMax = maxVelocityOverLifetime.y
            };
            velocityOverLifetime.y = minMaxCurveY;

            // Define min, max Z velocity
            ParticleSystem.MinMaxCurve minMaxCurveZ = new()
            {
                mode = ParticleSystemCurveMode.TwoConstants,
                constantMin = minVelocityOverLifetime.z,
                constantMax = maxVelocityOverLifetime.z
            };
            velocityOverLifetime.z = minMaxCurveZ;
        }

#endif
        #endregion
    }
}
