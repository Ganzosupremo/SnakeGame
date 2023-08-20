using SnakeGame.AudioSystem;
using SnakeGame.HealthSystem;
using System;
using UnityEngine;

namespace SnakeGame.PlayerSystem.AbilitySystem
{
    [CreateAssetMenu(fileName = "MultipleBullets_", menuName = "Scriptable Objects/Player/Multiple Bullets Ability")]
    public class MultipleBulletsSO : UniversalAbility
    {
        #region Tooltip
        [Tooltip("The min bullets that will spawn at the same time." +
            " A random value will be choosen between the min and max values")]
        [Range(1, 20)]
        #endregion
        public int MinBullets;
        #region Tooltip
        [Tooltip("The max bullets that will spawn at the same time." +
            " A random value will be choosen between the min and max values.")]
        [Range(1, 20)]
        #endregion
        public int MaxBullets;

        [Range(0f, 50f)]
        public float MinSpread;
        [Range(0f, 50f)]
        public float MaxSpread;

        #region Tooltip
        [Tooltip("The min spawn bullet interval, so they don't collide with each other." +
            " A random value will be choosen between the min and max values.")]
        [Range(0.01f, 0.1f)]
        #endregion
        public float SpawnIntervalMin;
        #region Tooltip
        [Tooltip("The max spawn bullet interval, so they don't collide with each other." +
            " A random value will be choosen between the min and max values.")]
        [Range(0.01f, 0.1f)]
        #endregion
        public float SpawnIntervalMax;


        private int originaMinBullets;
        private int originaMaxBullets;
        private float originalMinSpread;
        private float originalMaxSpread;
        private float originalSpawnIntervalMax;
        private float originalSpawnIntervalMin;

        public override void Activate(GameObject parent)
        {
            InitIfNeeded(parent);
            
            PlaySoundEffect();

            StoreOriginalValues(Snake);

            Health health = parent.GetComponent<Health>();
            health.TakeDamage(AbilityCost);

            if (!parent.activeSelf) return;

            Snake.activeWeapon.GetCurrentAmmo().SetBulletsToSpawn(MinBullets, MaxBullets);
            Snake.activeWeapon.GetCurrentAmmo().SetAmmoSpread(MinSpread, MaxSpread);
            Snake.activeWeapon.GetCurrentAmmo().SetAmmoSpawnInterval(SpawnIntervalMin, SpawnIntervalMax);
        }

        public override void Cooldown(GameObject parent)
        {
            InitIfNeeded(parent);

            Snake.activeWeapon.GetCurrentAmmo().SetBulletsToSpawn(originaMinBullets, originaMaxBullets);
            Snake.activeWeapon.GetCurrentAmmo().SetAmmoSpread(originalMinSpread, originalMaxSpread);
            Snake.activeWeapon.GetCurrentAmmo().SetAmmoSpawnInterval(originalSpawnIntervalMin, originalSpawnIntervalMax);
        }
        
        /// <summary>
        /// Store the original values of the current weapon ammo before changing them.
        /// </summary>
        /// <param name="snake"></param>
        private void StoreOriginalValues(Snake snake)
        {
            originaMinBullets = snake.activeWeapon.GetCurrentAmmo().MinBulletsPerShoot;
            originaMaxBullets = snake.activeWeapon.GetCurrentAmmo().MaxBulletsPerShoot;
            originalMinSpread = snake.activeWeapon.GetCurrentAmmo().ammoSpreadMin;
            originalMaxSpread = snake.activeWeapon.GetCurrentAmmo().ammoSpreadMax;
            originalSpawnIntervalMin = snake.activeWeapon.GetCurrentAmmo().minSpawnInterval;
            originalSpawnIntervalMax = snake.activeWeapon.GetCurrentAmmo().maxSpawnInterval;
        }
    }
}
