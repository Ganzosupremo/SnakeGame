using UnityEngine;

[DisallowMultipleComponent]
public class WeaponShootEffect : MonoBehaviour
{
    private ParticleSystem shootEffectParticleSystem;
    private void Awake()
    {
        shootEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Sets The Shoot Effect Devired From The Passed In Shoot Effects And AimAngle
    /// </summary>
    public void SetShootEffect(WeaponShootEffectSO shootEffect, float aimAngle)
    {
        // Set shoot effect color gradient
        SetShootEffectColorGradient(shootEffect.colorGradient);

        // Set shoot effect particle system starting values
        SetShootEffectParticleStartingValues(shootEffect.particleDuration, shootEffect.startParticleSize, shootEffect.startParticleSpeed,
            shootEffect.startLifetime, shootEffect.gravityEffect, shootEffect.maxParticles);

        // Set shoot effect particle system particle burst particle number
        SetShootEffectParticleEmission(shootEffect.emissionRate, shootEffect.burstNumber);

        // Set emmitter rotation
        SetEmmitterRotation(aimAngle);

        // Set shoot effect particle sprite
        SetShootEffectParticleSprite(shootEffect.sprites);

        // Set shoot effect lifetime min and max velocities
        SetShootEffectVelocityOverLifeTime(shootEffect.minVelocityOverLifetime, shootEffect.maxVelocityOverLifetime);
    }

    /// <summary>
    /// Set The Shoot Effect Particle System Color Gradient
    /// </summary>
    private void SetShootEffectColorGradient(Gradient colorGradient)
    {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = shootEffectParticleSystem.colorOverLifetime;
        colorOverLifetime.color = colorGradient;
    }

    /// <summary>
    /// Set shoot effect particle system starting values
    /// </summary>
    private void SetShootEffectParticleStartingValues(float particleDuration, float startParticleSize, float startParticleSpeed, float startLifetime, float gravityEffect, int maxParticles)
    {
        ParticleSystem.MainModule mainModule = shootEffectParticleSystem.main;

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
    private void SetShootEffectParticleEmission(int emissionRate, int burstNumber)
    {
        ParticleSystem.EmissionModule emissionModule = shootEffectParticleSystem.emission;

        // Set particle burst number
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstNumber);
        emissionModule.SetBurst(0, burst);

        // Set particle emission rate
        emissionModule.rateOverTime = emissionRate;
    }

    /// <summary>
    /// Set the rotation of the emmitter to match the aim angle
    /// </summary>
    private void SetEmmitterRotation(float aimAngle)
    {
        transform.eulerAngles = new Vector3(0f, 0f, aimAngle);
    }

    /// <summary>
    /// Set shoot effect particle system sprite
    /// </summary>
    private void SetShootEffectParticleSprite(Sprite[] sprites)
    {
        // Set particle animation
        ParticleSystem.TextureSheetAnimationModule animationModule = shootEffectParticleSystem.textureSheetAnimation;
        animationModule.SetSprite(0, sprites[0]);
        
        // Set the multiple sprites that are in the sprites array
        for (int i = 0; i < sprites.Length; i++)
        {
            animationModule.AddSprite(sprites[i]);
            //animationModule.SetSprite(i, sprites[i]);
        }
    }

    /// <summary>
    /// Set the shoot effect velocity over lifetime
    /// </summary>
    private void SetShootEffectVelocityOverLifeTime(Vector3 minVelocityOverLifetime, Vector3 maxVelocityOverLifetime)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = shootEffectParticleSystem.velocityOverLifetime;

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
