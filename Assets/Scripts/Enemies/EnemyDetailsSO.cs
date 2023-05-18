using SnakeGame.Debuging;
using SnakeGame.GameUtilities;
using System;
using System.Runtime.CompilerServices;
using System.Timers;
using UnityEngine;

namespace SnakeGame.Enemies
{
    [CreateAssetMenu(fileName = "Enemy_", menuName = "Scriptable Objects/Enemy/Enemy Details")]
    public class EnemyDetailsSO : UniversalEnemy
    {
        private static float m_DifficultyAdjustmentTimeSeconds = 60f;
        private static float m_HealthIncreasePercentage = 10f;

        public static float DifficultyAdjustemtTimeSeconds { get => m_DifficultyAdjustmentTimeSeconds; set => m_DifficultyAdjustmentTimeSeconds = value; }
        public static float HealthIncreasePercentage { get => m_HealthIncreasePercentage; set => m_HealthIncreasePercentage = value; }

        private void OnEnable()
        {
            DifficultyManager.OnDifficultyChanged += DifficultyManager_OnDifficultyChanged;
            SetDefaultWeaponValues();
        }

        private void SetDefaultWeaponValues()
        {
            // Set the default weapon values and save them for later
            m_DefaultFireMinDelay = firingMinDelay;
            m_DefaultFireMaxDelay = firingMaxDelay;
            m_DefaultFireMinDuration = firingMinDuration;
            m_DefaultFireMaxDuration = firingMaxDuration;
            m_DefaultRequireLineOfSight = lineOfSightRequired;
        }

        private void OnDisable()
        {
            DifficultyManager.OnDifficultyChanged -= DifficultyManager_OnDifficultyChanged;

            // Reset the health amount to it's original value
            ResetEnemyHealthToDefault();
            ResetWeaponValues();
        }

        private void ResetWeaponValues()
        {
            // Reset the overrided weapon values to default

            firingMinDelay = m_DefaultFireMinDelay;
            firingMaxDelay = m_DefaultFireMaxDelay;
            firingMinDuration = m_DefaultFireMinDuration;
            firingMaxDuration = m_DefaultFireMaxDuration;
            lineOfSightRequired = m_DefaultRequireLineOfSight;
        }

        private void DifficultyManager_OnDifficultyChanged(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.Noob:

                    ReconfigureWeapon(0f, 0.1f, 0.1f, 0.1f);
                    IncreaseEnemyMoveSpeed();
                    ResetEnemyHealthToDefault();
                    SetEnemyImmunityTime();

                    break;
                case Difficulty.Easy:

                    ReconfigureWeapon(0.2f, 0.6f, 1f, 2f);
                    IncreaseEnemyMoveSpeed(0.5f);
                    ReconfigureEnemyHealth(10);
                    SetEnemyImmunityTime();

                    break;
                case Difficulty.Medium:

                    ReconfigureWeapon(0.3f, 0.8f, 1.5f, 2.5f);
                    IncreaseEnemyMoveSpeed(0.8f);
                    ReconfigureEnemyHealth(20);
                    SetEnemyImmunityTime();

                    break;
                case Difficulty.Hard:

                    ReconfigureWeapon(0.5f, 1f, 2f, 2.8f);
                    IncreaseEnemyMoveSpeed(1f);
                    ReconfigureEnemyHealth(40);
                    SetEnemyImmunityTime();

                    break;
                case Difficulty.DarkSouls:

                    ReconfigureWeapon(0.6f, 1.2f, 2.5f, 3.5f, false, true, 1);
                    IncreaseEnemyMoveSpeed(1.2f);

                    // Add extra health
                    ReconfigureEnemyHealth(50);
                    SetEnemyImmunityTime(true, 0.25f);

                    break;
                case Difficulty.EmotionalDamage:

                    ReconfigureWeapon(0.8f, 1.4f, 3f, 4.5f, false, true, 2);
                    IncreaseEnemyMoveSpeed(1.5f);
                    ReconfigureEnemyHealth(60);
                    SetEnemyImmunityTime(true, 0.5f);

                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sets the immunity fields of the enemy with the specified values
        /// </summary>
        /// <param name="isImmune"></param>
        /// <param name="timeOfImmunity"></param>
        private void SetEnemyImmunityTime(bool isImmune = false, float timeOfImmunity = 0f)
        {
            immuneAfterHit = isImmune;
            immunityTime = timeOfImmunity;
        }

        /// <summary>
        /// Increases move speed by the specified moveSpeed.
        /// </summary>
        /// <param name="moveSpeed"></param>
        private void IncreaseEnemyMoveSpeed(float moveSpeed = 0f)
        {
            SetEnemySpeedToDefault();
            
            // Increase enemy move speed
            MovementDetails.minMoveSpeed += moveSpeed;
            MovementDetails.maxMoveSpeed += moveSpeed;
        }

        /// <summary>
        /// Resets the enemy speed to the default values
        /// </summary>
        private void SetEnemySpeedToDefault()
        {
            MovementDetails.minMoveSpeed = MovementDetails.DefaultMinMoveSpeed;
            MovementDetails.maxMoveSpeed = MovementDetails.DefaultMaxMoveSpeed;
        }

        /// <summary>
        /// Adds the specified extra health on top of the enemy health
        /// </summary>
        /// <param name="healthIncrease">The percentage to increase the health.</param>
        private void ReconfigureEnemyHealth(float healthIncrease)
        {
            ResetEnemyHealthToDefault();

            for (int i = 0; i < enemyHealthDetailsArray.Length; i++)
            {
                int percent = enemyHealthDetailsArray[i].GetHealthPercentage(healthIncrease);
                for (int y = 0; y < enemyHealthDetailsArray.Length; y++)
                {
                    enemyHealthDetailsArray[i].IncreaseHealth(percent);
                }
            }
            //for (int i = 0; i < enemyHealthDetailsArray.Length; i++)
            //{
            //    // And now the health can be increased
            //}
        }

        /// <summary>
        /// Resets the changed health to the default value
        /// </summary>
        private void ResetEnemyHealthToDefault()
        {
            for (int i = 0; i < enemyHealthDetailsArray.Length; i++)
            {
                // If the difficulty is change, reset the health to the original value
                enemyHealthDetailsArray[i].healthAmount = enemyHealthDetailsArray[i].defaultHealthAmount;
            }
        }

        /// <summary>
        /// Reconfigures the enemy weapon values with the new specified values.
        /// Adding the new values on top of the default ones.
        /// </summary>
        /// <param name="minFireDelay">The fire min delay, this value will be substracting from the default value.</param>
        /// <param name="maxFireDelay">The fire max delay, this value will be substracting from the default value.</param>
        /// <param name="minFireDuration"></param>
        /// <param name="maxFireDuration"></param>
        /// <param name="requieresLineOfSight">True if the enemy needs to have a sight of the player before firing.</param>
        /// <param name="increaseDamage">Always set to true so the damage reset whenever the difficulty is changed.</param>
        /// <param name="damageToIncrease"></param>
        private void ReconfigureWeapon(float minFireDelay = 0.5f, float maxFireDelay = 1f, float minFireDuration = 1f, float maxFireDuration = 2f, 
            bool requieresLineOfSight = true, bool increaseDamage = false, int damageToIncrease = 0)
        {
            if (enemyWeapon != null && !IsBoss)
            {
                ResetWeaponValues();

                firingMinDelay -= minFireDelay;
                firingMaxDelay -= maxFireDelay;

                if (firingMinDelay <= 0f && firingMaxDelay <= 1f)
                {
                    firingMinDelay = 0.1f;
                    firingMaxDelay = 0.5f;
                }

                firingMinDuration += minFireDuration;
                firingMaxDuration += maxFireDuration;

                lineOfSightRequired = requieresLineOfSight;

                if (increaseDamage)
                    enemyWeapon.weaponCurrentAmmo.IncreaseDamage(damageToIncrease);
            }
        }

        public static void StartTimer()
        {
            m_DifficultyAdjustmentTimeSeconds -= Time.deltaTime;

            if (m_DifficultyAdjustmentTimeSeconds <= 0f)
            {
                
            }
        }




        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
            HelperUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);
            HelperUtilities.ValidateCheckNullValue(this, nameof(enemyMaterializeShader), enemyMaterializeShader);
            HelperUtilities.ValidateCheckNullValue(this, nameof(standardEnemyMaterial), standardEnemyMaterial);
            HelperUtilities.ValidateCheckNullValue(this, nameof(deathSoundEffect), deathSoundEffect);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(enemyChaseDistance), enemyChaseDistance, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(enemyMaterializeTime), enemyMaterializeTime, true);

            HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingMinDelay), firingMinDelay,
                nameof(firingMaxDelay), firingMaxDelay, false);
            HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingMinDuration), firingMinDuration,
                nameof(firingMaxDuration), firingMaxDuration, false);

            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyHealthDetailsArray), enemyHealthDetailsArray);
            if (immuneAfterHit)
            {
                HelperUtilities.ValidateCheckPositiveValue(this, nameof(immunityTime), immunityTime, false);
            }
        }
#endif
        #endregion
    }
}