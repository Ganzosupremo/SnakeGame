using SnakeGame.Debuging;
using SnakeGame.GameUtilities;
using SnakeGame.TimeSystem;
using UnityEngine;

namespace SnakeGame.Enemies
{
    [CreateAssetMenu(fileName = "Enemy_", menuName = "Scriptable Objects/Enemy/Enemy Details")]
    public class EnemyDetailsSO : UniversalEnemy
    {
        public static float HealthIncreasePercentage { get => m_HealthIncreasePercentage; set => m_HealthIncreasePercentage = value; }
        private static float m_HealthIncreasePercentage = 1f;

        private DiffStatus m_CurrentStatus;

        private void OnEnable()
        {
            DifficultyManager.OnDifficultyChanged += OnUIDifficultyChanged;
            Timer.OnStatusChanged += OnTimeElapsed;
            m_CurrentStatus = DiffStatus.Easy;
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
            DifficultyManager.OnDifficultyChanged -= OnUIDifficultyChanged;
            Timer.OnStatusChanged -= OnTimeElapsed;

            // Reset the health amount to it's original value
            ResetEnemyHealthToDefault();
            SetEnemySpeedToDefault();
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

        private void OnUIDifficultyChanged(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.Noob:

                    ReconfigureWeapon(0f, 0.1f, 0.1f, 0.1f);
                    IncreaseEnemyMoveSpeed();
                    ResetEnemyHealthToDefault();
                    SetEnemySpeedToDefault();
                    SetEnemyImmunityTime();

                    break;
                case Difficulty.Easy:

                    ReconfigureWeapon(0.2f, 0.6f, 1f, 2f);
                    SetEnemyImmunityTime();

                    break;
                case Difficulty.Medium:

                    ReconfigureWeapon(0.3f, 0.8f, 1.5f, 2.5f);
                    SetEnemyImmunityTime();

                    break;
                case Difficulty.Hard:

                    ReconfigureWeapon(0.5f, 1f, 2f, 2.8f);
                    SetEnemyImmunityTime();

                    break;
                case Difficulty.DarkSouls:

                    ReconfigureWeapon(0.6f, 1.2f, 2.5f, 3.5f, false, true, 1);
                    SetEnemyImmunityTime(true, 0.25f);

                    break;
                case Difficulty.EmotionalDamage:

                    ReconfigureWeapon(0.8f, 1.4f, 3f, 4.5f, false, true, 2);
                    SetEnemyImmunityTime(true, 0.5f);

                    break;
                default:
                    break;
            }
        }

        private void OnTimeElapsed(TimerEventArgs args)
        {
            // To avoid increase the health on every in-game minute.
            //if (m_CurrentStatus == args.Status) return;
            m_CurrentStatus = args.Status;

            ReconfigureEnemyHealth(m_HealthIncreasePercentage, false);
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
        public void ReconfigureEnemyHealth(float healthIncrease, bool shouldResetHealth = true)
        {
            if (shouldResetHealth)
                ResetEnemyHealthToDefault();

            for (int i = 0; i < enemyHealthDetailsArray.Length; i++)
            {
                int percent = enemyHealthDetailsArray[i].GetHealthPercentage(healthIncrease);
                enemyHealthDetailsArray[i].IncreaseHealth(percent);
            }
        }

        /// <summary>
        /// Resets the changed health to the default value
        /// </summary>
        private void ResetEnemyHealthToDefault()
        {
            for (int i = 0; i < enemyHealthDetailsArray.Length; i++)
            {
                // If the difficulty is changed, reset the health to the original value
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