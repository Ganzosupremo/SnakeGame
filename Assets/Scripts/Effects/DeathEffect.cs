using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame.VisualEffects
{
    public class DeathEffect : MonoBehaviour
    {
        private ParticleSystem _DeathEffectParticleSystem;

        private void Awake()
        {
            _DeathEffectParticleSystem = GetComponent<ParticleSystem>();
        }

        /// <summary>
        /// Sets The Effect Devired From The Passed In Variables
        /// </summary>
        public void InitialiseDeathEffect(DeathEffectSO deathEffect)
        {
            // Set shoot effect color gradient
            SetDeathEffectColorGradient(deathEffect.ColorGradient);

            float gravityScale = Random.Range(deathEffect.MinGravityEffect, deathEffect.MaxGravityEffect);

            // Set shoot effect particle system starting values
            SetMainModule(deathEffect.ParticleDuration, deathEffect.StartParticleSize, deathEffect.StartParticleSpeed,
                deathEffect.StartLifetime, gravityScale, deathEffect.MaxParticles);

            SetShapeModule(deathEffect, deathEffect.Type);
            
            // Set shoot effect particle system particle burst particle number
            SetEmissionModule(deathEffect.EmissionRate, deathEffect.BurstNumber);

            // Set shoot effect particle sprite
            SetAnimationModule(deathEffect.SpriteArray);

            // Set the lifetime min and max velocities
            SetVelocityOverLifeTimeModule(deathEffect.MinVelocityOverLifetime, deathEffect.MaxVelocityOverLifetime);
        }

        /// <summary>
        /// Set The Particle System Color Gradient
        /// </summary>
        private void SetDeathEffectColorGradient(Gradient colorGradient)
        {
            ParticleSystem.ColorOverLifetimeModule colorOverLifetime = _DeathEffectParticleSystem.colorOverLifetime;
            colorOverLifetime.color = colorGradient;
        }

        /// <summary>
        /// Set the particle system starting values
        /// </summary>
        private void SetMainModule(float particleDuration, float startParticleSize, float startParticleSpeed, float startLifetime, float gravityEffect, int maxParticles)
        {
            ParticleSystem.MainModule mainModule = _DeathEffectParticleSystem.main;

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

        /// <summary>
        /// Set the particle system's particle burst and number of particles
        /// </summary>
        private void SetEmissionModule(int emissionRate, int burstNumber)
        {
            ParticleSystem.EmissionModule emissionModule = _DeathEffectParticleSystem.emission;

            // Set particle burst number
            ParticleSystem.Burst burst = new(0f, burstNumber);
            emissionModule.SetBurst(0, burst);

            // Set particle emission rate
            emissionModule.rateOverTime = emissionRate;
        }

        private void SetShapeModule(DeathEffectSO deathEffect, EffectType effectType = EffectType.None)
        {
            ParticleSystem.ShapeModule shapeModule = _DeathEffectParticleSystem.shape;
            gameObject.transform.localRotation = Quaternion.identity;

            switch (effectType)
            {
                case EffectType.Sphere_Burst:
                    
                    shapeModule.shapeType = ParticleSystemShapeType.Sphere;
                    shapeModule.radius = deathEffect.Radius;
                    shapeModule.radiusThickness = deathEffect.RadiusThickness;
                    shapeModule.arc = deathEffect.Arc;
                    shapeModule.arcMode = deathEffect.Mode;
                    shapeModule.arcSpread = deathEffect.Spread;
                    shapeModule.arcSpeed = deathEffect.Speed;

                    break;
                case EffectType.Circle_Explosion:

                    shapeModule.shapeType = ParticleSystemShapeType.Circle;
                    shapeModule.radius = deathEffect.Radius;
                    shapeModule.radiusThickness = deathEffect.RadiusThickness;
                    shapeModule.arc = deathEffect.Arc;
                    shapeModule.arcMode = deathEffect.Mode;
                    shapeModule.arcSpeed = deathEffect.Speed;
                    shapeModule.arcSpread = deathEffect.Spread;

                    break;
                case EffectType.Cone_Upwards:

                    float xRotation = -90f;
                    gameObject.transform.localRotation = new Quaternion(xRotation, 0f, 0f, 0f);

                    shapeModule.shapeType = ParticleSystemShapeType.Cone;
                    shapeModule.angle = deathEffect.Angle;
                    shapeModule.radius = deathEffect.Radius;
                    shapeModule.radiusThickness = deathEffect.RadiusThickness;
                    shapeModule.arc = deathEffect.Arc;
                    shapeModule.arcMode = deathEffect.Mode;
                    shapeModule.arcSpeed = deathEffect.Speed;
                    shapeModule.arcSpread = deathEffect.Spread;

                    break;
                case EffectType.Circle_Whirpool:

                    shapeModule.shapeType = ParticleSystemShapeType.Circle;
                    shapeModule.radius = deathEffect.Radius;
                    shapeModule.radiusThickness = deathEffect.RadiusThickness;
                    shapeModule.arc = deathEffect.Arc;
                    shapeModule.arcMode = deathEffect.Mode;
                    shapeModule.arcSpeed = deathEffect.Speed;
                    shapeModule.arcSpread = deathEffect.Spread;

                    break;
                case EffectType.None:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Set the particle system sprite
        /// </summary>
        private void SetAnimationModule(params Sprite[] sprites)
        {
            ParticleSystem.TextureSheetAnimationModule animationModule = _DeathEffectParticleSystem.textureSheetAnimation;
            animationModule.SetSprite(0, sprites[0]);

            foreach (Sprite sprite in sprites)
            {
                animationModule.AddSprite(sprite);
            }
        }

        /// <summary>
        /// Set the velocity over lifetime
        /// </summary>
        private void SetVelocityOverLifeTimeModule(Vector3 minVelocityOverLifetime, Vector3 maxVelocityOverLifetime)
        {
            ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = _DeathEffectParticleSystem.velocityOverLifetime;

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
    }
}
