using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame.AbwehrSystem.Ammo
{
    [CreateAssetMenu(fileName = "LaserAmmoDetails_", menuName = "Scriptable Objects/Weapon System/Laser Ammo Details")]
    public class LaserAmmoSO : BaseAmmoSO
    {
        [Space(5)]
        [Header("Laser Ammo Setttings")]
        [Tooltip("Defines the layers this ammo should hit.")]
        public LayerMask HitMask;

        public bool HasLineRenderer;

        public Material LineRendererMaterial;

        [Range(0.1f, 2f)]
        public float LineRendererStartWidth;
        [Range(0.1f, 2f)]
        public float LineRendererEndWidth;

        public Gradient LineGradient;
    }
}
