using UnityEngine;
using SnakeGame.SoundsSystem;

namespace SnakeGame.Enemies
{
    [CreateAssetMenu(fileName = "Enemy_", menuName = "Scriptable Objects/Enemy/Enemy Details")]
    public class EnemyDetailsSO : UniversalEnemy
    {
        private void OnEnable()
        {
            // Reset the health amount to it's original value
            for (int i = 0; i < enemyHealthDetailsArray.Length; i++)
            {
                enemyHealthDetailsArray[i].healthAmount = enemyHealthDetailsArray[i].defaultHealthAmount;
            }
        }

        private void OnDisable()
        {
            // Reset the health amount to it's original value
            for (int i = 0; i < enemyHealthDetailsArray.Length; i++)
            {
                enemyHealthDetailsArray[i].healthAmount = enemyHealthDetailsArray[i].defaultHealthAmount;
            }
        }

        [ContextMenu("Difficulty - Noob")]
        private void Noob()
        {
            enemyChaseDistance = 10;

            firingMinDelay = 3f;
            firingMaxDelay = 5f;

            firingMinDuration = 0.1f;
            firingMaxDuration = 0.2f;

            for (int i = 0; i < enemyHealthDetailsArray.Length; i++)
            {
                enemyHealthDetailsArray[i].healthAmount = 50;
            }
        }

        [ContextMenu("Easy Difficulty")]
        private void Easy()
        {
            enemyChaseDistance = 20;

            firingMinDelay = 2f;
            firingMaxDelay = 4f;

            firingMinDuration = 0.2f;
            firingMaxDuration = 0.4f;

            for (int i = 0; i < enemyHealthDetailsArray.Length; i++)
            {
                enemyHealthDetailsArray[i].healthAmount = 100;
            }
        }

        protected override void PowerUpEnemy(GameObject parent)
        {
            
        }

        protected override void DowngradeEnemy(GameObject parent)
        {
            
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