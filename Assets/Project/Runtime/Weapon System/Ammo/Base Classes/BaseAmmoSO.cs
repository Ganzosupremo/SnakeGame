using SnakeGame.AudioSystem;
using SnakeGame.GameUtilities;
using SnakeGame.VisualEffects;
using UnityEngine;

namespace SnakeGame.AbwehrSystem.Ammo
{
    /// <summary>
    /// The base Scriptable Object for the ammo that all other Scriptable Ammo Objects can derive from.
    /// Defines all the base parameters necessary to make the ammo work.
    /// </summary>
    public abstract class BaseAmmoSO : ScriptableObject
    {
        #region Header Base Ammo Details
        [Header("Base Ammo Details")]
        [Space(10)]
        #endregion
        public string ammoName;
        #region Tooltip
        [Tooltip("If this is set to true, the Increase and Decrease methods of this class," +
            " will work on percentages instead of flat numbers. " +
            "And also do other things on other classes.")]
        #endregion
        public bool isPlayerAmmo;

        #region Header Sprite, Prefab and Materials
        [Header("Ammo Sprite, Prefab and Materials")]
        [Space(10)]
        #endregion
        public Sprite ammoSprite;
        #region Tooltip
        [Tooltip("This can be populated with the prefab that will be used for the ammo, multiple prefabs can be used and " +
            "they will be randomly selected. The prefab can be an ammo pattern as long it is compatible with the IFireable interface.")]
        #endregion
        public GameObject[] ammoPrefabArray;
        #region Tooltip
        [Tooltip("The material used for the ammo")]
        #endregion
        public Material ammoMaterial;
        #region Tooltip
        [Tooltip("The ammo charge time - If the ammo should charge briefly before it can move. For example when creating an ammo pattern, the ammo should charge first before moving")]
        [Range(0f, 2f)]
        #endregion
        public float ammoChargeTime = 0.1f;
        #region Tooltip
        [Tooltip("If the ammo has a charge time, then specify the material that should be used to render the ammo while it's charging")]
        #endregion
        public Material ammoChargeMaterial;

        #region Ammo Hit Effect
        [Header("Collision Effects Settings")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("The SO that defines the parameters for the ammo hit effect prefab")]
        #endregion
        public AmmoHitEffectSO ammoHitEffect;

        public SoundEffectSO CollisionSoundEffect;

        #region Header Base Ammo Parameters
        [Header("Base Ammo Parameters")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("The damage for each individual ammo")]
        [Range(1, 1000)]
        #endregion
        public int ammoDamage = 1;
        #region Tooltip
        [Tooltip("As the damage of the weapon will increase in runtime, this variable sets the damage" +
            " to it's original value when exiting the app or restarting the game" +
            " ppopulate with the starting damage of each weapon.")]
        [Range(1, 1000)]
        #endregion
        public int originalAmmoDamage = 1;
        #region Tooltip
        [Tooltip("The max damage this weapon will do in this entire playtrough")]
        [Range(1, 1000)]
        #endregion
        public int maxAmmoDamage = 100;
        #region Tooltip
        [Tooltip("The min speed for each ammo - the speed will be a random value between the min and max speed")]
        [Range(1f, 100f)]
        #endregion
        public float minAmmoSpeed = 20f;
        #region Tooltip
        [Tooltip("The max speed for each ammo - the speed will be a random value between the min and max speed")]
        [Range(1f, 100f)]
        #endregion
        public float maxAmmoSpeed = 20f;
        #region Tooltip
        [Tooltip("The range of the ammo, or ammo patterns, in unity units")]
        [Range(1f, 200f)]
        #endregion
        public float ammoRange = 20f;
        #region Tooltip
        [Tooltip("The rotation speed for the ammo, it only works with ammo patterns - in degrees per second")]
        [Range(1f, 20f)]
        #endregion
        public float ammoRotationSpeed = 1f;

        #region Header Ammo Spread Details
        [Header("Base Ammo Spreed Details")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("This is the min spread angle for the ammo. A higher spread means less accuracy. A random value is calculated between the min and the max spread values")]
        [Range(0f, 50f)]
        #endregion
        public float ammoSpreadMin = 0f;
        #region Tooltip
        [Tooltip("This is the max spread angle for the ammo. A higher spread means less accuracy. A random value is calculated between the min and the max spread values")]
        [Range(0f, 50f)]
        #endregion
        public float ammoSpreadMax = 1f;
        
        #region Header Ammo Spawn Details
        [Header("Ammo Spawn Settings")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("This is the min number of bullets that will spawn per shoot. A random value is calculated between the min and the max values")]
        [Range(1, 10)]
        #endregion
        public int MinBulletsPerShoot = 1;
        #region Tooltip
        [Tooltip("This is the max number of bullets that will spawn per shoot. A random value is calculated between the min and the max values." +
            " One is added in the FireWeapon class logic, so if the max ammo to spawn is 2, the final max ammo will be 3.")]
        [Range(1, 10)]
        #endregion
        public int MaxBulletsPerShoot = 1;
        #region Tooltip
        [Tooltip("The ammo spawn interval between them so they don't appear all at once. It is randomly calculated with the min and max values")]
        [Range(0f, 0.1f)]
        #endregion
        public float minSpawnInterval = 0f;
        #region Tooltip
        [Tooltip("The ammo spawn interval between them so they don't appear all at once. It is randomly calculated with the min and max values")]
        [Range(0f, 0.1f)]
        #endregion
        public float maxSpawnInterval = 0f;

        #region Header Ammo Trail Details
        [Header("Base Ammo Trail Details")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("Select if the ammo should have a trail, if not deselect")]
        #endregion
        public bool hasAmmoTrail = false;
        #region Tooltip
        [Tooltip("The ammo trail lifetime in seconds")]
        #endregion
        public float ammoTrailLifetime = 3f;
        #region Tooltip
        [Tooltip("The ammo trail material")]
        #endregion
        public Material ammoTrailMaterial;
        #region Tooltip
        [Tooltip("The starting width for the trail")]
        [Range(0f, 2f)]
        #endregion
        public float ammoTrailStartWidth;
        #region Tooltip
        [Tooltip("The ending width for the trail")]
        [Range(0f, 2f)]
        #endregion
        public float ammoTrailEndWidth;

        protected virtual void OnEnable()
        {
            ammoDamage = originalAmmoDamage;
        }

        protected virtual void OnDisable()
        {
            ammoDamage = originalAmmoDamage;
        }

        /// <summary>
        /// Sets the spread of the current ammo.
        /// A random value will be choosen between the min and max values.
        /// </summary>
        /// <param name="minSpread"></param>
        /// <param name="maxSpread"></param>
        public void SetAmmoSpread(float minSpread, float maxSpread)
        {
            ammoSpreadMin = minSpread;
            ammoSpreadMax = maxSpread;
        }

        /// <summary>
        /// Sets the interval with which, if set to more than one, multiple bullets will spawn.
        /// A random value will be choosen between the min and max values.
        /// </summary>
        /// <param name="minSpawnInterval"></param>
        /// <param name="maxSpawnInterval"></param>
        public void SetAmmoSpawnInterval(float minSpawnInterval, float maxSpawnInterval)
        {
            this.minSpawnInterval = minSpawnInterval;
            this.maxSpawnInterval = maxSpawnInterval;
        }

        /// <summary>
        /// Sets the amount of bullets that will spawn per shoot.
        /// A random value will be choosen between the min and max values.
        /// </summary>
        /// <param name="minBullets"></param>
        /// <param name="maxBullets"></param>
        public void SetBulletsToSpawn(int minBullets, int maxBullets)
        {
            MinBulletsPerShoot = minBullets;
            MaxBulletsPerShoot = maxBullets;
        }

        /// <summary>
        /// This method should only be used when increasing the damage of the enemy weapons.
        /// </summary>
        /// <param name="damageIncrease">The new damage in percent</param>
        public void IncreaseDamage(int damageIncrease)
        {
            if (!isPlayerAmmo)
            {
                ammoDamage = originalAmmoDamage;
                ammoDamage += damageIncrease;
            }
        }

        /// <summary>
        /// Increase the damage of the weapon.
        /// The increase must be in decimals. For example if you want to increase
        /// the damage by 20%, 20% as a decimal is 0.2, or 50% as decimal is 0.5
        /// ALWAYS PASS THE VALUE AS DECIMAL.
        /// </summary>
        /// <param name="increase"></param>
        public void IncreaseDamage(float increase)
        {
            if (isPlayerAmmo)
            {
                float damageIncrease = ammoDamage * increase;
                ammoDamage += Mathf.RoundToInt(damageIncrease);

                if (ammoDamage > maxAmmoDamage)
                {
                    ammoDamage = maxAmmoDamage;
                    GameManager.Instance.CallShowMesageRoutine($"Max Damage!", 1.5f);
                    return;
                }
                GameManager.Instance.CallShowMesageRoutine($"Damage increased by {damageIncrease} points.", 1.5f);
            }
        }

        /// <summary>
        /// Decreases the weapon damage.
        /// If this is player ammo, the decrease should be in percentage,
        /// if it's not, i.e in enemy weapons, the decrease should be in flat numbers.
        /// With player ammo, the damage cannot go below the original ammo damage,
        /// and with enemies, the damage cannot go below zero.
        /// </summary>
        /// <param name="decreaseDamageBy">The damage to decrease in percentage</param>
        public void DecreaseDamage(int damageDecrease)
        {
            if (isPlayerAmmo)
            {
                int decreaseDamage = ammoDamage * damageDecrease / 100;
                ammoDamage -= decreaseDamage;

                if (ammoDamage < originalAmmoDamage)
                {
                    ammoDamage = originalAmmoDamage;
                    return;
                }
                GameManager.Instance.CallShowMesageRoutine($"Damage Decreased!", 1.5f);
            }
            // Is enemy ammo
            else
            {
                ammoDamage -= damageDecrease;
                if (ammoDamage < 0)
                    ammoDamage = 1;
            }
        }

        public void DecreaseDamage(float decrease)
        {
            if (isPlayerAmmo)
            {
                float damageIncrease = ammoDamage * decrease;
                ammoDamage -= Mathf.RoundToInt(damageIncrease);

                if (ammoDamage < originalAmmoDamage)
                {
                    ammoDamage = originalAmmoDamage;
                    GameManager.Instance.CallShowMesageRoutine($"Lowest Damage Possible, Stop Getting Hit!", 1.2f);
                    return;
                }

                GameManager.Instance.CallShowMesageRoutine($"Damage Decreased!", 1.5f);
            }
        }

        #region Validation
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            HelperUtilities.ValidateCheckEmptyString(this, nameof(ammoName), ammoName);
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoSprite), ammoSprite);
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoPrefabArray), ammoPrefabArray);
            HelperUtilities.ValidateCheckPositiveRange(this, nameof(minAmmoSpeed), minAmmoSpeed, nameof(maxAmmoSpeed), maxAmmoSpeed, false);

            if (ammoChargeTime > 0)
                HelperUtilities.ValidateCheckNullValue(this, nameof(ammoChargeMaterial), ammoChargeMaterial);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoDamage), ammoDamage, false);
            HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpreadMin), ammoSpreadMin, nameof(ammoSpreadMax), ammoSpreadMax, true);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoRange), ammoRange, false);
            HelperUtilities.ValidateCheckPositiveRange(this, nameof(minAmmoSpeed), minAmmoSpeed, nameof(maxAmmoSpeed), maxAmmoSpeed, false);
            HelperUtilities.ValidateCheckPositiveRange(this, nameof(MinBulletsPerShoot), MinBulletsPerShoot, nameof(MaxBulletsPerShoot), MaxBulletsPerShoot, false);
            HelperUtilities.ValidateCheckPositiveRange(this, nameof(minSpawnInterval), minSpawnInterval, nameof(maxSpawnInterval), maxSpawnInterval, true);

            if (hasAmmoTrail)
            {
                HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailLifetime), ammoTrailLifetime, false);
                HelperUtilities.ValidateCheckNullValue(this, nameof(ammoTrailMaterial), ammoTrailMaterial);
                HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailStartWidth), ammoTrailStartWidth, false);
                HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailEndWidth), ammoTrailEndWidth, false);
            }
        }
#endif
        #endregion
    }
}
