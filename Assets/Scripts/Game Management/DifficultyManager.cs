using UnityEngine;
using UnityEngine.UI;

namespace SnakeGame
{
    public class DifficultyManager : SingletonMonoBehaviour<DifficultyManager>
    {
        public void ChangeDifficulty(Difficulty selectedDifficulty)
        {
            #region A Fricking Big Chunk of Code
            switch (selectedDifficulty)
            {
                case Difficulty.Noob:

                    foreach (EnemyDetailsSO enemy in GameResources.Instance.enemyDetailsList)
                    {
                        enemy.enemyChaseDistance = 10f;

                        // Configure this values only if the enemy has a weapon
                        if (enemy.enemyWeapon != null)
                        {
                            enemy.firingMinDelay = 3f;
                            enemy.firingMaxDelay = 5f;

                            enemy.firingMinDuration = 0.1f;
                            enemy.firingMaxDuration = 0.2f;
                        }

                        for (int i = 0; i < enemy.enemyHealthDetailsArray.Length; i++)
                        {
                            // If the difficulty is change, reset the health to the original value
                            enemy.enemyHealthDetailsArray[i].healthAmount = enemy.enemyHealthDetailsArray[i].originalHealthAmount;
                        }

                        enemy.immuneAfterHit = false;
                    }
                    // Set the player health depending on the difficulty selected
                    foreach (SnakeDetailsSO snake in GameResources.Instance.snakeDetailsList)
                    {
                        snake.snakeInitialHealth = 10;
                    }

                    //GameResources.Instance.SetLightIntensity(1f);
                    break;

                case Difficulty.Easy:
                    foreach (var enemy in GameResources.Instance.enemyDetailsList)
                    {
                        enemy.enemyChaseDistance = 20f;

                        // Configure this values only if the enemy has a weapon
                        if (enemy.enemyWeapon != null)
                        {
                            enemy.firingMinDelay = 2f;
                            enemy.firingMaxDelay = 4f;

                            enemy.firingMinDuration = 0.2f;
                            enemy.firingMaxDuration = 0.4f;
                        }

                        int extraHealth = 50;
                        for (int i = 0; i < enemy.enemyHealthDetailsArray.Length; i++)
                        {
                            // If the difficulty is change, reset the health to the original value
                            enemy.enemyHealthDetailsArray[i].healthAmount = enemy.enemyHealthDetailsArray[i].originalHealthAmount;
                            // And now the health can be increased
                            enemy.enemyHealthDetailsArray[i].healthAmount += extraHealth;
                        }

                        enemy.immuneAfterHit = false;
                    }

                    foreach (SnakeDetailsSO snake in GameResources.Instance.snakeDetailsList)
                    {
                        snake.snakeInitialHealth = 8;
                    }
                    
                    //GameResources.Instance.SetLightIntensity(0.9f);
                    break;

                case Difficulty.Medium:
                    foreach (var enemy in GameResources.Instance.enemyDetailsList)
                    {
                        enemy.enemyChaseDistance = 30f;

                        // Configure this values only if the enemy has a weapon
                        if (enemy.enemyWeapon != null)
                        {
                            enemy.firingMinDelay = 1.5f;
                            enemy.firingMaxDelay = 3.5f;

                            enemy.firingMinDuration = 0.4f;
                            enemy.firingMaxDuration = 0.8f;
                        }

                        int extraHealth = 100;
                        for (int i = 0; i < enemy.enemyHealthDetailsArray.Length; i++)
                        {
                            // If the difficulty is change, reset the health to the original value
                            enemy.enemyHealthDetailsArray[i].healthAmount = enemy.enemyHealthDetailsArray[i].originalHealthAmount;
                            // And now the health can be increased
                            enemy.enemyHealthDetailsArray[i].healthAmount += extraHealth;
                        }

                        enemy.immuneAfterHit = false;
                    }

                    foreach (SnakeDetailsSO snake in GameResources.Instance.snakeDetailsList)
                    {
                        snake.snakeInitialHealth = 6;
                    }
                    //GameResources.Instance.currentSnake.snakeDetails.snakeInitialHealth = 6;
                    //GameResources.Instance.SetLightIntensity(0.75f);
                    break;

                case Difficulty.Hard:
                    foreach (var enemy in GameResources.Instance.enemyDetailsList)
                    {
                        enemy.enemyChaseDistance = 50f;

                        // Configure this values only if the enemy has a weapon
                        if (enemy.enemyWeapon != null)
                        {
                            enemy.firingMinDelay = 0.85f;
                            enemy.firingMaxDelay = 2f;

                            enemy.firingMinDuration = 1f;
                            enemy.firingMaxDuration = 1.8f;
                            enemy.lineOfSightRequired = false;
                        }

                        int extraHealth = 150;
                        for (int i = 0; i < enemy.enemyHealthDetailsArray.Length; i++)
                        {
                            // If the difficulty is change, reset the health to the original value
                            enemy.enemyHealthDetailsArray[i].healthAmount = enemy.enemyHealthDetailsArray[i].originalHealthAmount;
                            // And now the health can be increased
                            enemy.enemyHealthDetailsArray[i].healthAmount += extraHealth;
                        }

                        enemy.immuneAfterHit = false;
                    }

                    foreach (SnakeDetailsSO snake in GameResources.Instance.snakeDetailsList)
                    {
                        snake.snakeInitialHealth = 4;
                    }
                    //GameResources.Instance.currentSnake.snakeDetails.snakeInitialHealth = 4;
                    //GameResources.Instance.SetLightIntensity(0.55f);
                    break;

                case Difficulty.DarkSouls:
                    foreach (var enemy in GameResources.Instance.enemyDetailsList)
                    {
                        enemy.enemyChaseDistance = 100f;

                        // Configure this values only if the enemy has a weapon
                        if (enemy.enemyWeapon != null)
                        {
                            enemy.firingMinDelay = 0.5f;
                            enemy.firingMaxDelay = 1f;

                            enemy.firingMinDuration = 2f;
                            enemy.firingMaxDuration = 4f;
                            enemy.lineOfSightRequired = false;

                            enemy.enemyWeapon.weaponCurrentAmmo.IncreaseDamage(1);
                        }

                        int extraHealth = 250;
                        for (int i = 0; i < enemy.enemyHealthDetailsArray.Length; i++)
                        {
                            // If the difficulty is change, reset the health to the original value
                            enemy.enemyHealthDetailsArray[i].healthAmount = enemy.enemyHealthDetailsArray[i].originalHealthAmount;
                            // And now the health can be increased
                            enemy.enemyHealthDetailsArray[i].healthAmount += extraHealth;
                        }

                        enemy.immuneAfterHit = true;
                        enemy.immunityTime = 2f;
                    }

                    foreach (SnakeDetailsSO snake in GameResources.Instance.snakeDetailsList)
                    {
                        snake.snakeInitialHealth = 3;
                    }
                    //GameResources.Instance.currentSnake.snakeDetails.snakeInitialHealth = 3;
                    //GameResources.Instance.SetLightIntensity(0.4f);
                    break;

                case Difficulty.EmotionalDamage:
                    foreach (var enemy in GameResources.Instance.enemyDetailsList)
                    {
                        enemy.enemyChaseDistance = 100f;

                        // Configure this values only if the enemy has a weapon
                        if (enemy.enemyWeapon != null)
                        {
                            enemy.firingMinDelay = 0.1f;
                            enemy.firingMaxDelay = 0.2f;

                            enemy.firingMinDuration = 5f;
                            enemy.firingMaxDuration = 8f;
                            enemy.lineOfSightRequired = false;

                            enemy.enemyWeapon.weaponCurrentAmmo.IncreaseDamage(2);
                        }

                        int extraHealth = 500;
                        for (int i = 0; i < enemy.enemyHealthDetailsArray.Length; i++)
                        {
                            // If the difficulty is change, reset the health to the original value
                            enemy.enemyHealthDetailsArray[i].healthAmount = enemy.enemyHealthDetailsArray[i].originalHealthAmount;
                            // And now the health can be increased
                            enemy.enemyHealthDetailsArray[i].healthAmount += extraHealth;
                        }

                        enemy.immuneAfterHit = true;
                        enemy.immunityTime = 4f;
                    }

                    foreach (SnakeDetailsSO snake in GameResources.Instance.snakeDetailsList)
                    {
                        snake.snakeInitialHealth = 1;
                    }
                    //GameResources.Instance.currentSnake.snakeDetails.snakeInitialHealth = 1;
                    //GameResources.Instance.SetLightIntensity(0.3f);
                    break;

                default:
                    break;
            }
            #endregion
        }
    }
}
