using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class defines <seealso cref="OnWeaponAim"/> event to aim the weapon at target position
/// </summary>
public class AimWeaponEvent : MonoBehaviour
{
    /// <summary>
    /// Event to aim the weapon at target position
    /// </summary>
    public event Action<AimWeaponEvent, AimWeaponEventArgs> OnWeaponAim;

    /// <summary>
    /// This method is called when a subscriber wants to trigger the <seealso cref="OnWeaponAim"/> event.
    /// </summary>
    /// <param name="aimDirection"></param>
    /// <param name="aimAngle"></param>
    /// <param name="weaponAimAngle"></param>
    /// <param name="weaponAimDirectionVector"></param>
    public void CallAimWeaponEvent(AimDirection aimDirection, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        OnWeaponAim?.Invoke(this, new AimWeaponEventArgs
        {
            aimDirection = aimDirection,
            aimAngle = aimAngle,
            weaponAimAngle = weaponAimAngle,
            weaponAimDirectionVector = weaponAimDirectionVector
        });
    }
}

public class AimWeaponEventArgs : EventArgs
{
    public AimDirection aimDirection;
    public float aimAngle;
    public float weaponAimAngle;
    public Vector3 weaponAimDirectionVector;
}
