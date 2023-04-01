using SnakeGame.Enemies;
using SnakeGame.PlayerSystem;
using System;

namespace SnakeGame
{
    public static class DifficultyManager
    {
        public static event Action<Difficulty> OnDifficultyChanged;
        
        private static Difficulty m_difficulty;

        [Obsolete]
        public static void ChangeDifficulty(Difficulty selectedDifficulty)
        {
            #region A Fricking Big Chunk of Code
            m_difficulty = selectedDifficulty;
            switch (selectedDifficulty)
            {
                case Difficulty.Noob:

                    foreach (EnemyDetailsSO enemy in GameResources.Instance.enemyDetailsList)
                    {
                        enemy.enemyChaseDistance = 30f;

                        // Configure this values only if the enemy has a weapon
                        SetEnemyWeapon(enemy, 3f, 8f, 0.5f, 1f, true, true ,0);
                        IncreaseEnemyMoveSpeed(enemy);

                        for (int i = 0; i < enemy.enemyHealthDetailsArray.Length; i++)
                        {
                            // If the difficulty is change, reset the health to the original value
                            enemy.enemyHealthDetailsArray[i].healthAmount = enemy.enemyHealthDetailsArray[i].defaultHealthAmount;
                        }
                        SetEnemyImmunityTime(enemy);
                    }
                    // Set the player health depending on the difficulty selected
                    foreach (SnakeDetailsSO snake in GameResources.Instance.snakeDetailsList)
                    {
                        snake.snakeInitialHealth = 10;
                    }

                    break;

                case Difficulty.Easy:
                    foreach (var enemy in GameResources.Instance.enemyDetailsList)
                    {
                        enemy.enemyChaseDistance = 30f;

                        // Configure this values only if the enemy has a weapon
                        SetEnemyWeapon(enemy, 2.5f, 6f, 1f, 2f, true, true, 0);
                        IncreaseEnemyMoveSpeed(enemy, 0.5f);

                        // Add extra health
                        SetEnemyHealth(enemy, 50);
                        SetEnemyImmunityTime(enemy);
                    }

                    foreach (SnakeDetailsSO snake in GameResources.Instance.snakeDetailsList)
                    {
                        snake.snakeInitialHealth = 8;
                    }

                    break;

                case Difficulty.Medium:
                    foreach (var enemy in GameResources.Instance.enemyDetailsList)
                    {
                        enemy.enemyChaseDistance = 30f;

                        // Configure this values only if the enemy has a weapon
                        SetEnemyWeapon(enemy, 2f, 4f, 1.5f, 2.5f, true, true, 0);
                        IncreaseEnemyMoveSpeed(enemy, 0.8f);

                        // Add extra health
                        SetEnemyHealth(enemy, 100);
                        SetEnemyImmunityTime(enemy);
                    }

                    foreach (SnakeDetailsSO snake in GameResources.Instance.snakeDetailsList)
                    {
                        snake.snakeInitialHealth = 6;
                    }

                    break;

                case Difficulty.Hard:
                    foreach (var enemy in GameResources.Instance.enemyDetailsList)
                    {
                        enemy.enemyChaseDistance = 50f;

                        // Configure this values only if the enemy has a weapon
                        SetEnemyWeapon(enemy, 1.5f, 3.5f, 2f, 2.8f, true, true, 0);
                        IncreaseEnemyMoveSpeed(enemy, 1f);

                        // Add extra health
                        SetEnemyHealth(enemy, 150);
                        SetEnemyImmunityTime(enemy);
                    }

                    foreach (SnakeDetailsSO snake in GameResources.Instance.snakeDetailsList)
                    {
                        snake.snakeInitialHealth = 5;
                    }

                    break;

                case Difficulty.DarkSouls:
                    foreach (var enemy in GameResources.Instance.enemyDetailsList)
                    {
                        enemy.enemyChaseDistance = 100f;

                        // Configure this values only if the enemy has a weapon
                        SetEnemyWeapon(enemy, 1f, 2f, 2.5f, 3.5f, false, true, 1);
                        IncreaseEnemyMoveSpeed(enemy, 1.2f);

                        // Add extra health
                        SetEnemyHealth(enemy, 250);
                        SetEnemyImmunityTime(enemy, true, 0.25f);
                    }

                    foreach (SnakeDetailsSO snake in GameResources.Instance.snakeDetailsList)
                    {
                        snake.snakeInitialHealth = 4;
                    }

                    break;

                case Difficulty.EmotionalDamage:
                    foreach (var enemy in GameResources.Instance.enemyDetailsList)
                    {
                        enemy.enemyChaseDistance = 100f;

                        // Configure this values only if the enemy has a weapon
                        SetEnemyWeapon(enemy, 0.5f, 1.5f, 3f, 4.5f, false, true, 2);
                        IncreaseEnemyMoveSpeed(enemy, 1.5f);

                        // Add extra health
                        SetEnemyHealth(enemy, 500);
                        SetEnemyImmunityTime(enemy, true, 0.5f);
                    }

                    foreach (SnakeDetailsSO snake in GameResources.Instance.snakeDetailsList)
                    {
                        snake.snakeInitialHealth = 3;
                    }

                    break;

                default:
                    break;
            }
            #endregion
        }

        private static void SetEnemySpeedToDefault(EnemyDetailsSO enemy)
        {
            enemy.MovementDetails.minMoveSpeed = enemy.MovementDetails.DefaultMinMoveSpeed;
            enemy.MovementDetails.maxMoveSpeed = enemy.MovementDetails.DefaultMaxMoveSpeed;
        }

        private static void IncreaseEnemyMoveSpeed(EnemyDetailsSO enemy, float moveSpeed = 0f)
        {
            SetEnemySpeedToDefault(enemy);

            enemy.MovementDetails.minMoveSpeed = enemy.MovementDetails.DefaultMinMoveSpeed;
            enemy.MovementDetails.maxMoveSpeed = enemy.MovementDetails.DefaultMaxMoveSpeed;
            // Increase enemy move speed
            enemy.MovementDetails.minMoveSpeed += moveSpeed;
            enemy.MovementDetails.maxMoveSpeed += moveSpeed;
        }

        private static void SetEnemyImmunityTime(EnemyDetailsSO enemy, bool isImmune = false, float immunityTime = 0f)
        {
            enemy.immuneAfterHit = isImmune;
            enemy.immunityTime = immunityTime;
        }

        private static void SetEnemyHealth(EnemyDetailsSO enemy, int extraHealth)
        {
            for (int i = 0; i < enemy.enemyHealthDetailsArray.Length; i++)
            {
                // If the difficulty is changed, reset the health to the original value
                enemy.enemyHealthDetailsArray[i].healthAmount = enemy.enemyHealthDetailsArray[i].defaultHealthAmount;
                // And now the health can be increased
                enemy.enemyHealthDetailsArray[i].healthAmount += extraHealth;
            }
        }

        /// <summary>
        /// Reconfigures the enemy weapon with the new specified values
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="fireMinDelay">The min time the enemy will take to fire again.</param>
        /// <param name="fireMaxDelay">The max time the enemy will take to fire again.</param>
        /// <param name="fireMinDuration">The min duration of fire.</param>
        /// <param name="fireMaxDuration">The max duration of fire.</param>
        /// <param name="lineOfSightRequired">True if the enemy requires a sight if the player before firing.</param>
        /// <param name="increaseDamage">True if the damage if the enemy weapon will increase.</param>
        /// <param name="newDamage">The damage to increase the enemy weapon to. NOTE: The damage should increase with flat numbers and not percentages.</param>
        private static void SetEnemyWeapon(EnemyDetailsSO enemy, float fireMinDelay = 0.1f, float fireMaxDelay = 0.5f, float fireMinDuration = 2f, float fireMaxDuration = 4f,
            bool lineOfSightRequired = true, bool increaseDamage = false, int newDamage = 0)
        {
            if (enemy.enemyWeapon != null)
            {
                enemy.firingMinDelay = fireMinDelay;
                enemy.firingMaxDelay = fireMaxDelay;

                enemy.firingMinDuration = fireMinDuration;
                enemy.firingMaxDuration = fireMaxDuration;
                enemy.lineOfSightRequired = lineOfSightRequired;

                if (increaseDamage)
                    enemy.enemyWeapon.weaponCurrentAmmo.IncreaseDamage(newDamage);
            }
        }

        public static void CallOnDifficultyChangedEvent(Difficulty difficulty)
        {
            OnDifficultyChanged?.Invoke(difficulty);
        }
    }
}
