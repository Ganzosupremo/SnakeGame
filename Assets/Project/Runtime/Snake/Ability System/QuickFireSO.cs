using SnakeGame.AudioSystem;
using SnakeGame.HealthSystem;
using SnakeGame.UI;
using System;
using UnityEngine;

namespace SnakeGame.PlayerSystem.AbilitySystem
{
    [CreateAssetMenu(fileName = "QuickFireAbility_", menuName = "Scriptable Objects/Player/Quick Fire Ability")]
    public class QuickFireSO : UniversalAbility
    {
        #region Tooltip
        [Tooltip("The new current weapon's fire rate while using this ability." +
            " Low fire rate value, means more bullets per second.")]
        [Range(0.01f, 1f)]
        #endregion
        public float FireRate;
        public bool HasInfinityAmmo = true;
        public bool HasInfinityClipCapacity = true;

        private float originalFireRate;
        private bool hasInfiniteAmmo;
        private bool hasInfiniteClipCapacity;

        public override void Activate(GameObject parent)
        {
            InitIfNeeded(parent);
            
            PlaySoundEffect();

            SaveOriginalValues(Snake);

            Health health = parent.GetComponent<Health>();
            health.TakeDamage(AbilityCost);

            if (!parent.activeSelf) return;

            Snake.activeWeapon.GetCurrentWeapon().weaponDetails.SetFireRate(FireRate);
            Snake.activeWeapon.GetCurrentWeapon().weaponDetails.SetInfinity(HasInfinityAmmo, HasInfinityClipCapacity);
        }

        private void SaveOriginalValues(Snake snake)
        {
            originalFireRate = snake.activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
            hasInfiniteAmmo = snake.activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteAmmo;
            hasInfiniteClipCapacity = snake.activeWeapon.GetCurrentWeapon().weaponDetails.hasInfinityClipCapacity;
        }

        public override void Cooldown(GameObject parent)
        {
            InitIfNeeded(parent);

            Snake.activeWeapon.GetCurrentWeapon().weaponDetails.SetFireRate(originalFireRate);
            Snake.activeWeapon.GetCurrentWeapon().weaponDetails.SetInfinity(hasInfiniteAmmo, hasInfiniteClipCapacity);
        }
    }
}
