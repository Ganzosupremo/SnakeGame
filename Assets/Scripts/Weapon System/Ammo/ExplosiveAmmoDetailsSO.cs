using SnakeGame.AbwehrSystem.Ammo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame
{
    [CreateAssetMenu(fileName = "ExplosiveAmmoDetails_", menuName = "Scriptable Objects/Weapon System/Explosive Ammo Details")]
    public class ExplosiveAmmoDetailsSO : BaseAmmoSO
    {
        [Header("Explosion Ammo Settings")]
        [Space(5)]
        public float ExplosionRadius = 1f;
        public LayerMask ExplosionMask;

    }
}
