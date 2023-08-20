using SnakeGame.GameUtilities;
using System;
using UnityEngine;

namespace SnakeGame.ObsoleteClasses
{
    [CreateAssetMenu(fileName = "SpecialAbility_", menuName = "Scriptable Objects/Player/Legacy/Special Ability")]
    [Obsolete("Don't use this scriptable object for special abilities anymore." +
        " Create another class and let that class inherit from UniversalAbility instead.")]
    public class SpecialAbilitySO : ScriptableObject
    {
        public Abilities ability;

        #region Tooltip
        [Tooltip("The cooldown for the abilities, since a snake type can only have one ability," +
            " this cooldown is shared between all abilities.")]
        [Range(1f, 10f)]
        #endregion
        public float abilityCooldownTimer;

        #region Tooltip
        [Tooltip("The duration of the selected ability, just as with the cooldown," +
            " this duration is shared between all the abilites, 'cause one ability per snake type.")]
        [Range(1f, 10f)]
        #endregion
        public float abilityDuration;

        #region Header Slow Down Time Ability
        [Header("Slow Down Time Ability")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("This value will be used to set the TimeScale to.")]
        [Range(0f, 1f)]
        #endregion
        public float slowdownTimeValue;

        #region Header Multiple Bullets Ability
        [Header("Multiple Bullets Ability")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("The min bullets that will spawn at the same time." +
            " A random value will be choosen between the min and max values")]
        [Range(1, 20)]
        #endregion
        public int minBullets;
        #region Tooltip
        [Tooltip("The max bullets that will spawn at the same time." +
            " A random value will be choosen between the min and max values.")]
        [Range(1, 20)]
        #endregion
        public int maxBullets;
        #region Tooltip
        [Tooltip("The min spawn bullet interval, so they don't collide with each other." +
            " A random value will be choosen between the min and max values.")]
        [Range(0.01f, 0.1f)]
        #endregion
        public float spawnIntervalMin;
        #region Tooltip
        [Tooltip("The max spawn bullet interval, so they don't collide with each other." +
            " A random value will be choosen between the min and max values.")]
        [Range(0.01f, 0.1f)]
        #endregion
        public float spawnIntervalMax;

        #region Header Flash Ability
        [Header("Flash Ability")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("The min velocity that the player will get while using this ability." +
            " The min and max values on the player should be the same.")]
        [Range(1, 20)]
        #endregion
        public float minVelocity;
        #region Tooltip
        [Tooltip("The max velocity that the player will get while using this ability." +
            " The min and max values on the player should be the same.")]
        [Range(1, 20)]
        #endregion
        public float maxVelocity;
        #region Tooltip
        [Tooltip("True if the player will be invincible while using this ability.")]
        #endregion
        public bool isInvincible = true;

        #region Header QuickFire Ability
        [Header("QuickFire Ability")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("The new current weapon's fire rate while using this ability." +
            " Low fire rate value, means more bullets per second.")]
        [Range(0f, 3f)]
        #endregion
        public float weaponFireRate;
        public bool hasInfinityAmmo;
        public bool hasInfinityClipCapacity;

        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(abilityDuration), abilityDuration, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(abilityCooldownTimer), abilityCooldownTimer, false);
            switch (ability)
            {
                case Abilities.None:
                    Debug.Log("No ability selected.");
                    break;
                case Abilities.SlowDownTime:
                    HelperUtilities.ValidateCheckPositiveValue(this, nameof(slowdownTimeValue), slowdownTimeValue, false);
                    break;
                case Abilities.MultipleBullets:
                    HelperUtilities.ValidateCheckPositiveRange(this, nameof(minBullets), minBullets, nameof(maxBullets), maxBullets, false);
                    break;
                case Abilities.Flash:
                    HelperUtilities.ValidateCheckPositiveRange(this, nameof(minVelocity), minVelocity, nameof(maxVelocity), maxVelocity, false);
                    break;
                case Abilities.QuickFire:
                    HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponFireRate), weaponFireRate, true);
                    break;
                default:
                    break;
            }
        }
#endif
        #endregion
    }
}