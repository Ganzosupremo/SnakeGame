using System;
using UnityEngine;

namespace SnakeGame.VisualEffects
{
    public class AmmoHitEffect : MonoBehaviour
    {
        private ParticleSystem ammoHitEffectParticleSystem;

        private void Awake()
        {
            ammoHitEffectParticleSystem = GetComponent<ParticleSystem>();
        }

        /// <summary>
        /// Sets The Shoot Effect From the AmmoHitEffectSO
        /// </summary>
        public void InitialiseAmmoHitEffect(AmmoHitEffectSO ammoHitEffect)
        {
            // Set shoot effect color gradient
            SetAmmoHitEffectColorGradient(ammoHitEffect.colorGradient);

            SetShapeModule(ammoHitEffect.Type);

            // Set shoot effect particle system starting values
            SetAmmoHitEffectParticleStartingValues(ammoHitEffect.particleDuration, ammoHitEffect.startParticleSize, ammoHitEffect.startParticleSpeed,
                ammoHitEffect.startLifetime, ammoHitEffect.gravityEffect, ammoHitEffect.maxParticles);

            // Set shoot effect particle system particle burst particle number
            SetAmmoHitEffectParticleEmission(ammoHitEffect.emissionRate, ammoHitEffect.burstNumber);

            // Set shoot effect particle sprite
            SetAmmoHitEffectParticleSprite(ammoHitEffect.sprite);

            // Set shoot effect lifetime min and max velocities
            SetAmmoHitEffectVelocityOverLifeTime(ammoHitEffect.minVelocityOverLifetime, ammoHitEffect.maxVelocityOverLifetime);
        }

        /// <summary>
        /// Sets the shoot effect from the AmmoHitEffectSO and sets it's position to the specified position.
        /// </summary>
        /// <param name="ammoHitEffect"></param>
        /// <param name="spawnPosition"></param>
        public void InitialiseAmmoHitEffect(AmmoHitEffectSO ammoHitEffect, Vector3 spawnPosition)
        {
            SetAmmoHitEffectPosition(spawnPosition);

            // Set shoot effect color gradient
            SetAmmoHitEffectColorGradient(ammoHitEffect.colorGradient);

            SetShapeModule(ammoHitEffect.Type);

            // Set shoot effect particle system starting values
            SetAmmoHitEffectParticleStartingValues(ammoHitEffect.particleDuration, ammoHitEffect.startParticleSize, ammoHitEffect.startParticleSpeed,
                ammoHitEffect.startLifetime, ammoHitEffect.gravityEffect, ammoHitEffect.maxParticles);

            // Set shoot effect particle system particle burst particle number
            SetAmmoHitEffectParticleEmission(ammoHitEffect.emissionRate, ammoHitEffect.burstNumber);

            // Set shoot effect particle sprite
            SetAmmoHitEffectParticleSprite(ammoHitEffect.sprite);

            // Set shoot effect lifetime min and max velocities
            SetAmmoHitEffectVelocityOverLifeTime(ammoHitEffect.minVelocityOverLifetime, ammoHitEffect.maxVelocityOverLifetime);
        }

        private void SetAmmoHitEffectPosition(Vector3 spawnPosition)
        {
            transform.position = spawnPosition;
        }

        /// <summary>
        /// Set The Shoot Effect Particle System Color Gradient
        /// </summary>
        private void SetAmmoHitEffectColorGradient(Gradient colorGradient)
        {
            ParticleSystem.ColorOverLifetimeModule colorOverLifetime = ammoHitEffectParticleSystem.colorOverLifetime;
            colorOverLifetime.color = colorGradient;
        }

        private void SetShapeModule(EffectType effectType = EffectType.None)
        {
            ParticleSystem.ShapeModule shapeModule = ammoHitEffectParticleSystem.shape;

            switch (effectType)
            {
                case EffectType.Sphere_Burst:

                    shapeModule.shapeType = ParticleSystemShapeType.Sphere;
                    shapeModule.radius = 2f;
                    shapeModule.radiusThickness = 0.5f;
                    shapeModule.arc = 360f;
                    shapeModule.arcMode = ParticleSystemShapeMultiModeValue.BurstSpread;
                    shapeModule.arcSpread = 0f;

                    break;
                case EffectType.Circle_Explosion:

                    shapeModule.shapeType = ParticleSystemShapeType.Circle;
                    shapeModule.radius = 0.2f;
                    shapeModule.radiusThickness = 0.2f;
                    shapeModule.arc = 360f;
                    shapeModule.arcMode = ParticleSystemShapeMultiModeValue.BurstSpread;
                    shapeModule.arcSpread = 0.01f;

                    break;
                case EffectType.Cone_Upwards:

                    float xRotation = -90f;
                    gameObject.transform.localRotation = new Quaternion(xRotation, 0f, 0f, 0f);

                    shapeModule.shapeType = ParticleSystemShapeType.Cone;
                    shapeModule.radius = 4f;
                    shapeModule.radiusThickness = 0.4f;
                    shapeModule.arc = 180f;
                    shapeModule.arcMode = ParticleSystemShapeMultiModeValue.Loop;
                    shapeModule.arcSpread = 0f;

                    break;
                case EffectType.Circle_Whirpool:

                    shapeModule.shapeType = ParticleSystemShapeType.Circle;
                    shapeModule.angle = 25f;
                    shapeModule.radius = 1f;
                    shapeModule.radiusThickness = 0.5f;
                    shapeModule.arc = 360f;
                    shapeModule.arcMode = ParticleSystemShapeMultiModeValue.Loop;
                    shapeModule.arcSpread = 0f;
                    shapeModule.arcSpeed = 10f;

                    break;
                case EffectType.None:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Set shoot effect particle system starting values
        /// </summary>
        private void SetAmmoHitEffectParticleStartingValues(float particleDuration, float startParticleSize, float startParticleSpeed, float startLifetime, float gravityEffect, int maxParticles)
        {
            ParticleSystem.MainModule mainModule = ammoHitEffectParticleSystem.main;

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
        /// Set shoot effect particle system particle burst particle number
        /// </summary>
        private void SetAmmoHitEffectParticleEmission(int emissionRate, int burstNumber)
        {
            ParticleSystem.EmissionModule emissionModule = ammoHitEffectParticleSystem.emission;

            // Set particle burst number
            ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstNumber);
            emissionModule.SetBurst(0, burst);

            // Set particle emission rate
            emissionModule.rateOverTime = emissionRate;
        }

        /// <summary>
        /// Set shoot effect particle system sprite
        /// </summary>
        private void SetAmmoHitEffectParticleSprite(Sprite sprite)
        {
            // Set particle animation
            ParticleSystem.TextureSheetAnimationModule animationModule = ammoHitEffectParticleSystem.textureSheetAnimation;

            animationModule.SetSprite(0, sprite);
        }

        /// <summary>
        /// Set the shoot effect velocity over lifetime
        /// </summary>
        private void SetAmmoHitEffectVelocityOverLifeTime(Vector3 minVelocityOverLifetime, Vector3 maxVelocityOverLifetime)
        {
            ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = ammoHitEffectParticleSystem.velocityOverLifetime;

            // Define min, max X velocity
            ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.TwoConstants,
                constantMin = minVelocityOverLifetime.x,
                constantMax = maxVelocityOverLifetime.x
            };
            velocityOverLifetime.x = minMaxCurveX;

            // Define min, max Y velocity
            ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.TwoConstants,
                constantMin = minVelocityOverLifetime.y,
                constantMax = maxVelocityOverLifetime.y
            };
            velocityOverLifetime.y = minMaxCurveY;

            // Define min, max Z velocity
            ParticleSystem.MinMaxCurve minMaxCurveZ = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.TwoConstants,
                constantMin = minVelocityOverLifetime.z,
                constantMax = maxVelocityOverLifetime.z
            };
            velocityOverLifetime.z = minMaxCurveZ;
        }
    }
}