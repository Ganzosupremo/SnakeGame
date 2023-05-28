using SnakeGame.AbwehrSystem;
using SnakeGame.Debuging;
using SnakeGame.HealthSystem;
using SnakeGame.PlayerSystem;
using System;
using UnityEngine;

namespace SnakeGame.ObsoleteClasses
{
    /// <summary>
    /// This class handles the snake's special abilities system
    /// </summary>
    [RequireComponent(typeof(Snake))]
    [RequireComponent(typeof(Health))]
    [DisallowMultipleComponent]
    [Obsolete("This class won't be used in the future for managing the special abilities," +
        " use the SnakeAbilityManager instead.")]
    public class SnakeSpecialAbility : MonoBehaviour
    {
        private Snake snake;
        private Health health;
        private SetActiveWeaponEvent setActiveWeaponEvent;
        private Abilities ability = Abilities.None;
        private SpecialAbilitySO specialAbilitySO;

        private int originalBulletsAmountMin;
        private int originalBulletsAmountMax;

        private float minOriginalMoveSpeed;
        private float maxOriginalMoveSpeed;

        private float originalFireRate;
        private bool hasInfiniteAmmo;
        private bool hasInfiniteClipCapacity;

        private void Awake()
        {
            snake = GetComponent<Snake>();
            health = GetComponent<Health>();
            setActiveWeaponEvent= GetComponent<SetActiveWeaponEvent>();
        }

        private void Start()
        {
            //originalBulletsAmountMin = snake.activeWeapon.GetCurrentAmmo().ammoSpawnAmountMin;
            //originalBulletsAmountMax = snake.activeWeapon.GetCurrentAmmo().ammoSpawnAmountMax;

            minOriginalMoveSpeed = snake.GetSnakeControler().GetMoveSpeed();
            maxOriginalMoveSpeed = snake.GetSnakeControler().GetMoveSpeed();

            //originalFireRate = snake.activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
            //hasInfiniteAmmo = snake.activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteAmmo;
            //hasInfiniteClipCapacity = snake.activeWeapon.GetCurrentWeapon().weaponDetails.hasInfinityClipCapacity;
        }

        private void OnEnable()
        {
            setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
        }

        private void OnDisable()
        {
            setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
        }

        private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
        {
            originalBulletsAmountMin = setActiveWeaponEventArgs.weapon.weaponDetails.weaponCurrentAmmo.MinBulletsPerShoot;
            originalBulletsAmountMax = setActiveWeaponEventArgs.weapon.weaponDetails.weaponCurrentAmmo.MaxBulletsPerShoot;

            originalFireRate = setActiveWeaponEventArgs.weapon.weaponDetails.weaponFireRate;
            hasInfiniteAmmo = setActiveWeaponEventArgs.weapon.weaponDetails.hasInfiniteAmmo;
            hasInfiniteClipCapacity = setActiveWeaponEventArgs.weapon.weaponDetails.hasInfinityClipCapacity;
        }

        /// <summary>
        /// Set the current ability, this method should be called first before activating
        ///  an ability.
        /// </summary>
        /// <param name="ability"></param>
        public void SetCurrentAbility(Abilities ability, SpecialAbilitySO specialAbilitySO)
        {
            this.ability = ability;
            this.specialAbilitySO = specialAbilitySO;
        }

        /// <summary>
        /// Activates the current snake's ability
        /// Call first the SetCurrentAbility method, otherwise this method won't work
        /// </summary>
        public void ActivateAbility()
        {
            switch (ability)
            {
                case Abilities.None:
                    this.LogWarning("No ability was set, you need to set an ability first by " +
                        "calling the SetCurrentAbility method.");
                    break;
                case Abilities.SlowDownTime:
                    Time.timeScale = specialAbilitySO.slowdownTimeValue;
                    break;
                case Abilities.MultipleBullets:
                    // Define a new spawn interval, this is used so the bullets don't collide with each other on spawn.
                    snake.activeWeapon.GetCurrentAmmo().minSpawnInterval = specialAbilitySO.spawnIntervalMin;
                    snake.activeWeapon.GetCurrentAmmo().maxSpawnInterval = specialAbilitySO.spawnIntervalMax;

                    // Set the new amount of bullets to spawn per shoot.
                    snake.activeWeapon.GetCurrentAmmo().MinBulletsPerShoot = specialAbilitySO.minBullets;
                    snake.activeWeapon.GetCurrentAmmo().MaxBulletsPerShoot = specialAbilitySO.maxBullets;
                    break;
                case Abilities.Flash:
                    //mainGameProfile.
                    snake.GetSnakeControler().SetMovementVelocity(specialAbilitySO.minVelocity, specialAbilitySO.maxVelocity);
                    health.SetIsDamageable(specialAbilitySO.isInvincible);
                    break;
                case Abilities.QuickFire:
                    snake.activeWeapon.GetCurrentWeapon().weaponDetails.SetFireRate(specialAbilitySO.weaponFireRate);
                    snake.activeWeapon.GetCurrentWeapon().weaponDetails.SetInfinity(specialAbilitySO.hasInfinityAmmo, specialAbilitySO.hasInfinityClipCapacity);
                    break;
                default:
                    break;
            }
        }
        
        /// <summary>
        /// Resets all the modified fileds by the ability back to normal.
        /// </summary>
        public void ResetToNormal()
        {
            switch (ability)
            {
                case Abilities.None:
                    break;
                case Abilities.SlowDownTime:
                    Time.timeScale = 1f;
                    break;
                case Abilities.MultipleBullets:
                    snake.activeWeapon.GetCurrentAmmo().MinBulletsPerShoot = originalBulletsAmountMin;
                    snake.activeWeapon.GetCurrentAmmo().MaxBulletsPerShoot = originalBulletsAmountMax;
                    break;
                case Abilities.Flash:
                    snake.GetSnakeControler().GetMovementDetails().minMoveSpeed = minOriginalMoveSpeed;
                    snake.GetSnakeControler().GetMovementDetails().maxMoveSpeed = maxOriginalMoveSpeed;
                    health.SetIsDamageable(!specialAbilitySO.isInvincible);
                    break;
                case Abilities.QuickFire:
                    snake.activeWeapon.GetCurrentWeapon().weaponDetails.SetFireRate(originalFireRate);
                    snake.activeWeapon.GetCurrentWeapon().weaponDetails.SetInfinity(hasInfiniteAmmo, hasInfiniteClipCapacity);
                    break;
                default:
                    break;
            }
        }
    }
}