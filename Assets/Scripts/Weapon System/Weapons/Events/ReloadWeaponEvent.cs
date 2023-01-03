using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ReloadWeaponEvent : MonoBehaviour
{
    public event Action<ReloadWeaponEvent, ReloadWeaponEventArgs> OnReload;

    public void CallReloadEvent(Weapon weapon, int reloadPercent)
    {
        OnReload?.Invoke(this, new ReloadWeaponEventArgs()
        {
            weapon = weapon,
            reloadPercent = reloadPercent
        });
    }
}

public class ReloadWeaponEventArgs : EventArgs
{
    public Weapon weapon;
    public int reloadPercent;
}
