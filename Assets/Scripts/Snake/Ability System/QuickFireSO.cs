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
            Snake snake = parent.GetComponent<Snake>();
            SaveOriginalValues(snake);
            Health health = parent.GetComponent<Health>();
            health.TakeDamage(1);

            if (!parent.activeSelf) return;

            snake.activeWeapon.GetCurrentWeapon().weaponDetails.SetFireRate(FireRate);
            snake.activeWeapon.GetCurrentWeapon().weaponDetails.SetInfinity(HasInfinityAmmo, HasInfinityClipCapacity);
        }

        private void SaveOriginalValues(Snake snake)
        {
            originalFireRate = snake.activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
            hasInfiniteAmmo = snake.activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteAmmo;
            hasInfiniteClipCapacity = snake.activeWeapon.GetCurrentWeapon().weaponDetails.hasInfinityClipCapacity;
        }

        public override void Cooldown(GameObject parent)
        {
            Snake snake = parent.GetComponent<Snake>();

            snake.activeWeapon.GetCurrentWeapon().weaponDetails.SetFireRate(originalFireRate);
            snake.activeWeapon.GetCurrentWeapon().weaponDetails.SetInfinity(hasInfiniteAmmo, hasInfiniteClipCapacity);
        }
    }
}
