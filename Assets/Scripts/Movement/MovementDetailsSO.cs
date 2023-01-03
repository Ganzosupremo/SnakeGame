using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/Movement Details")]
public class MovementDetailsSO : ScriptableObject
{
    #region Header Movement Details
    [Space(10)]
    [Header("Movement Details")]
    #endregion

    #region Tooltip
    [Tooltip("Min move speed - The GetMoveSpeed Method calculates a random value between the min and max.")]
    #endregion
    public float minMoveSpeed = 8f;

    #region Tooltip
    [Tooltip("Max move speed - The GetMoveSpeed Method calculates a random value between the min and max.")]
    #endregion
    public float maxMoveSpeed = 8f;

    #region
    [Tooltip("This is the speed for the roll mechanich - it is used only on the player, not the enemies")]
    #endregion
    public float dashSpeed;

    #region
    [Tooltip("This is the distance for the roll mechanich - it is used only on the player, not the enemies")]
    #endregion
    public float dashDistance;

    #region Tooltip
    [Tooltip("This is the cooldown for the roll mechanich - it is used only on the player, not the enemies")]
    #endregion
    public float dashCooldownTime;

    /// <summary>
    /// Gets a random value between the min and max speed.
    /// </summary>
    /// <returns>The randomly selected speed</returns>
    public float GetMoveSpeed()
    {
        if (minMoveSpeed == maxMoveSpeed)
        {
            return minMoveSpeed;
        }
        else
        {
            return Random.Range(minMoveSpeed, maxMoveSpeed);
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false);

        if (dashDistance != 0 || dashSpeed != 0 || dashCooldownTime != 0)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(dashDistance), dashDistance, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(dashSpeed), dashSpeed, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(dashCooldownTime), dashCooldownTime, false);
        }
    }
#endif
    #endregion
}
