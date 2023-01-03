using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponReloadedEvent : MonoBehaviour
{
    public event Action<WeaponReloadedEvent, WeaponReloadedEventArgs> OnReloaded;

    public void CallWeaponReloaded(Weapon weapon)
    {
        OnReloaded?.Invoke(this, new WeaponReloadedEventArgs()
        {
            weapon = weapon
        });
    }
}

public class WeaponReloadedEventArgs : EventArgs
{
    public Weapon weapon;
}
