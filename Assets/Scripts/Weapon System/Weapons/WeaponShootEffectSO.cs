using SnakeGame.GameUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponShootEffect_", menuName = "Scriptable Objects/Weapon System/Shoot Effect")]
public class WeaponShootEffectSO : ScriptableObject
{
    #region Header Shoot Effect Details
    [Header("Shoot Effect Settings")]
    [Space(10)]
    #endregion

    #region Tooltip
    [Tooltip("The color gradient for the shoot effect.  This gradient show the color of particles during their lifetime - from left to right ")]
    #endregion Tooltip
    public Gradient colorGradient;

    #region Tooltip
    [Tooltip("The length of time the particle system is emitting particles")]
    [Range(0f, 10f)]
    #endregion Tooltip
    public float particleDuration = 0.5f;

    #region Tooltip
    [Tooltip("The start particle size for the particle effect")]
    [Range(0f, 10f)]
    #endregion Tooltip
    public float startParticleSize = 0.2f;

    #region Tooltip
    [Tooltip("The start particle speed for the particle effect")]
    [Range(0f, 20f)]
    #endregion Tooltip
    public float startParticleSpeed = 3f;

    #region Tooltip
    [Tooltip("The particle lifetime for the particle effect")]
    [Range(0f, 20f)]
    #endregion Tooltip
    public float startLifetime = 0.5f;

    #region Tooltip
    [Tooltip("The maximum number of particles to be emitted")]
    [Range(0, 500)]
    #endregion Tooltip
    public int maxParticles = 100;

    #region Tooltip
    [Tooltip("The number of particles emitted per second. If zero it will just be the burst number")]
    [Range(0, 500)]
    #endregion Tooltip
    public int emissionRate = 100;

    #region Tooltip
    [Tooltip("How many particles should be emmitted in the particle effect burst")]
    [Range(0, 500)]
    #endregion Tooltip
    public int burstNumber = 30;

    #region Tooltip
    [Tooltip("The gravity on the particles - a small negative number will make them float up")]
    [Range(-10f, 10f)]
    #endregion
    public float gravityEffect = -0.5f;

    #region Tooltip
    [Header("Particle Sprites")]
    [Tooltip("The sprite array for the particle effect. If none is specified then the default particle sprite will be used")]
    [Space(10)]
    #endregion Tooltip
    public Sprite[] sprites;

    #region Tooltip
    [Space(5)]
    [Tooltip("The min velocity for the particle over its lifetime. A random value between min and max will be generated.")]
    #endregion Tooltip
    public Vector3 minVelocityOverLifetime;

    #region Tooltip
    [Tooltip("The max velocity for the particle over its lifetime. A random value between min and max will be generated.")]
    #endregion Tooltip
    public Vector3 maxVelocityOverLifetime;

    #region Tooltip
    [Tooltip("weaponShootEffectPrefab contains the particle system for the shoot effect - and is configured by the weaponShootEffect SO")]
    #endregion Tooltip
    public GameObject shootEffectPrefab;

    #region Camera Shake
    [Header("Camera Shake")]
    [Space(10)]
    #endregion

    #region Tooltip
    [Tooltip("Enables or Disables the camera shake for this weapon")]
    #endregion Tooltip
    public bool enableShake = false;

    #region Tooltip
    [Tooltip("The amplitud of the noise")]
    [Range(0f, 10f)]
    #endregion Tooltip
    public float amplitud = 1f;

    #region Tooltip
    [Tooltip("The camera shake intensity")]
    [Range(0f, 10f)]
    #endregion Tooltip
    public float intensity = 1f;

    #region Tooltip
    [Tooltip("The time the camera will shake - lower values are better")]
    [Range(0f, 5f)]
    #endregion Tooltip
    public float time = 0.1f;


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
        HelperUtilities.ValidateCheckNullValue(this, nameof(shootEffectPrefab), shootEffectPrefab);
    }

#endif
    #endregion Validation
}
