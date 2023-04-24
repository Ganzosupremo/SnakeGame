using SnakeGame.AbwehrSystem;
using SnakeGame.AudioSystem;
using SnakeGame.GameUtilities;
using UnityEngine;

namespace SnakeGame.Enemies
{
    public class UniversalEnemy : ScriptableObject
    {
        #region Header Base Enemy Details
        [Header("Base Enemy Details")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("The name for the enemy")]
        #endregion
        public string enemyName;

        #region Tooltip
        [Tooltip("The prefab for the enemy")]
        #endregion
        public GameObject enemyPrefab;

        public bool IsBoss = false;
        
        #region Tooltip
        [Tooltip("The Scriptable Object containing the movement settings")]
        #endregion
        public MovementDetailsSO MovementDetails;

        public SoundEffectSO hitSoundEffect;
        public SoundEffectSO deathSoundEffect;

        [ColorUsage(true, true)]
        public Color enemyColor;

        #region Tooltip
        [Tooltip("Distance from which the enemy will chase the player")]
        #endregion
        public float enemyChaseDistance = 50f;

        #region Header Enemy Materialize Settings
        [Header("Settings For Enemy Materialize")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("The time the enemy will take for the enemy to materialize")]
        #endregion
        public float enemyMaterializeTime;

        #region Tooltip
        [Tooltip("The shader that'll be used when the enemy materializes")]
        #endregion
        public Shader enemyMaterializeShader;

        #region Tooltip
        [Tooltip("The color of the materialize effect - it's and HDR, so the intensity can make the color glow")]
        #endregion
        [ColorUsage(true, true)]
        public Color enemyMaterializeColor;

        #region Header Enemy Materials
        [Header("Materials For The Enemy")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("This is the sprite default-lit material")]
        #endregion
        public Material standardEnemyMaterial;

        #region Header Enemy's Weapon Settings
        [Header("Enemy Weapon Settings")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("The weapon for the enemy - leave blank if the enemy won't have a weapon")]
        #endregion
        public WeaponDetailsSO enemyWeapon;

        #region Tooltip
        [Tooltip("The minimum delay in seconds btw shoots for the enemy - the end value will be a random value btw min and max")]
        #endregion
        public float firingMinDelay = 0.1f;
        #region Tooltip
        [Tooltip("The maximum delay in seconds btw shoots for the enemy - the end value will be a random value btw min and max")]
        #endregion
        public float firingMaxDelay = 1f;

        #region Tooltip
        [Tooltip("The minimum duration in seconds that the enemy is going to fire for - the end value will be a random value btw min and max")]
        #endregion
        public float firingMinDuration = 1f;
        #region Tooltip
        [Tooltip("The maximum duration in seconds that the enemy is going to fire for - the end value will be a random value btw min and max")]
        #endregion
        public float firingMaxDuration = 2f;

        #region Tooltip
        [Tooltip("Check this if the enemy will need a sight of the player before shooting - if it's not selected the enemy" +
            " will shoot regardless of obstacles, but whenever the player is in range")]
        #endregion
        public bool lineOfSightRequired;

        #region Header Enemy Health
        [Header("Enemy Health Parameters")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("The Enemy Health for each individual level")]
        #endregion
        public ItemHealthDetails[] enemyHealthDetailsArray;

        #region Tooltip
        [Tooltip("Select if the enemy will have immunity after hit - if so, select the immunity time under this field")]
        #endregion
        public bool immuneAfterHit = false;

        #region Tooltip
        [Tooltip("Immunity time in seconds after hit")]
        #endregion
        public float immunityTime;

        #region Tooltip
        [Tooltip("The time the enemy will stop moving after hit - Not yet implemented")]
        #endregion
        public float startDazeTime;

        #region Tooltip
        [Tooltip("Select if the enemy will have a health bar above itself")]
        #endregion
        public bool isHealthBarDisplayed = false;

        internal float m_DefaultFireMinDelay;
        internal float m_DefaultFireMaxDelay;
        internal float m_DefaultFireMinDuration;
        internal float m_DefaultFireMaxDuration;
        internal bool m_DefaultRequireLineOfSight = true;


        protected virtual void PowerUpEnemy(GameObject parent) { }

        protected virtual void DowngradeEnemy(GameObject parent) { }


        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValue(this, nameof(MovementDetails), MovementDetails);
            HelperUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);
            HelperUtilities.ValidateCheckNullValue(this, nameof(hitSoundEffect), hitSoundEffect);
            HelperUtilities.ValidateCheckNullValue(this, nameof(deathSoundEffect), deathSoundEffect);
            HelperUtilities.ValidateCheckNullValue(this, nameof(enemyMaterializeShader), enemyMaterializeShader);
        }
#endif
        #endregion
    }
}
