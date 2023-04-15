using System.Collections;
using UnityEngine;
using SnakeGame.AudioSystem;
using Cysharp.Threading.Tasks;
using System.Threading;

[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class ReloadWeapon : MonoBehaviour
{
    private ReloadWeaponEvent reloadWeaponEvent;
    private WeaponReloadedEvent weaponReloadedEvent;
    private SetActiveWeaponEvent activeWeaponEvent;
    private Coroutine reloadWeaponCoroutine;

    private void Awake()
    {
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        activeWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    private void OnEnable()
    {
        //subscribe to the reload weapon event
        reloadWeaponEvent.OnReload += ReloadWeaponEvent_OnReload;

        //suscribe to the active weapon event
        activeWeaponEvent.OnSetActiveWeapon += ActiveWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable()
    {
        //unsubscribe to the reload weapon event
        reloadWeaponEvent.OnReload -= ReloadWeaponEvent_OnReload;

        //unsuscribe to the active weapon event
        activeWeaponEvent.OnSetActiveWeapon -= ActiveWeaponEvent_OnSetActiveWeapon;
    }

    /// <summary>
    /// Handles The Reload Weapon Event
    /// </summary>
    private void ReloadWeaponEvent_OnReload(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        StartReloadingWeapon(reloadWeaponEventArgs);
    }

    private void StartReloadingWeapon(ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        var CancellationTokenSource = new CancellationTokenSource();

        ReloadWeaponAsync(reloadWeaponEventArgs.weapon, reloadWeaponEventArgs.reloadPercent, CancellationTokenSource);

        //if (reloadWeaponCoroutine != null)
        //{
        //    StopCoroutine(reloadWeaponCoroutine);
        //}

        //reloadWeaponCoroutine = StartCoroutine(ReloadingWeaponCoroutine(reloadWeaponEventArgs.weapon, reloadWeaponEventArgs.reloadPercent));

    }

    private IEnumerator ReloadingWeaponCoroutine(Weapon weapon, int reloadPercent)
    {
        // Play the reload sound when the weapon is reloaded
        if (weapon.weaponDetails.reloadSound != null)
            SoundEffectManager.CallOnSoundEffectSelectedEvent(weapon.weaponDetails.reloadSound);

        weapon.isWeaponReloading = true;

        //Update the reload timer
        while (weapon.weaponReloadTimer < weapon.weaponDetails.weaponReloadTime)
        {
            weapon.weaponReloadTimer += Time.deltaTime;
            yield return null;
        }

        //If the total ammo should be increased
        if (reloadPercent != 0)
        {
            int ammoIncrease = Mathf.RoundToInt((weapon.weaponDetails.totalAmmoCapacity * reloadPercent) / 100);

            int totalAmmo = weapon.weaponTotalAmmoCapacity + ammoIncrease;

            if (totalAmmo > weapon.weaponDetails.totalAmmoCapacity)
                weapon.weaponTotalAmmoCapacity = weapon.weaponDetails.totalAmmoCapacity;
            else
                weapon.weaponTotalAmmoCapacity = totalAmmo;
        }

        //If the weapon has infinity ammo, then just refill the mag
        if (weapon.weaponDetails.hasInfiniteAmmo)
        {
            weapon.weaponClipRemaining = weapon.weaponDetails.clipMaxCapacity;
        }
        // else if not infinite ammo then if remaining ammo is greater than the amount required to
        // refill the clip, then fully refill the clip
        else if (weapon.weaponTotalAmmoCapacity >= weapon.weaponDetails.clipMaxCapacity)
        {
            weapon.weaponClipRemaining = weapon.weaponDetails.clipMaxCapacity;
        }
        // else set the clip to the remaining ammo
        else
        {
            weapon.weaponClipRemaining = weapon.weaponTotalAmmoCapacity;
        }

        // Reset weapon reload timer
        weapon.weaponReloadTimer = 0f;

        // Set weapon as not reloading
        weapon.isWeaponReloading = false;

        // Call weapon reloaded event
        weaponReloadedEvent.CallWeaponReloaded(weapon);
    }

    private async void ReloadWeaponAsync(Weapon weapon, int reloadPercent, CancellationTokenSource cancellationToken)
    {
        // Play the reload sound when the weapon is reloaded
        if (weapon.weaponDetails.reloadSound != null)
            SoundEffectManager.CallOnSoundEffectSelectedEvent(weapon.weaponDetails.reloadSound);

        weapon.isWeaponReloading = true;

        //Update the reload timer
        while (weapon.weaponReloadTimer < weapon.weaponDetails.weaponReloadTime)
        {
            weapon.weaponReloadTimer += Time.deltaTime;
            await UniTask.WaitForEndOfFrame(this, cancellationToken: cancellationToken.Token);
        }

        //If the total ammo should be increased
        if (reloadPercent != 0)
        {
            int ammoIncrease = Mathf.RoundToInt((weapon.weaponDetails.totalAmmoCapacity * reloadPercent) / 100);

            int totalAmmo = weapon.weaponTotalAmmoCapacity + ammoIncrease;

            if (totalAmmo > weapon.weaponDetails.totalAmmoCapacity)
                weapon.weaponTotalAmmoCapacity = weapon.weaponDetails.totalAmmoCapacity;
            else
                weapon.weaponTotalAmmoCapacity = totalAmmo;
        }

        //If the weapon has infinity ammo, then just refill the mag
        if (weapon.weaponDetails.hasInfiniteAmmo)
        {
            weapon.weaponClipRemaining = weapon.weaponDetails.clipMaxCapacity;
        }
        // else if not infinite ammo then if remaining ammo is greater than the amount required to
        // refill the clip, then fully refill the clip
        else if (weapon.weaponTotalAmmoCapacity >= weapon.weaponDetails.clipMaxCapacity)
        {
            weapon.weaponClipRemaining = weapon.weaponDetails.clipMaxCapacity;
        }
        // else set the clip to the remaining ammo
        else
        {
            weapon.weaponClipRemaining = weapon.weaponTotalAmmoCapacity;
        }

        // Reset weapon reload timer
        weapon.weaponReloadTimer = 0f;

        // Set weapon as not reloading
        weapon.isWeaponReloading = false;

        // Call weapon reloaded event
        weaponReloadedEvent.CallWeaponReloaded(weapon);
    }

    /// <summary>
    /// Check if the <see cref="reloadWeaponCoroutine"/> is currently running, if so stop it
    /// and then start the coroutine again.
    /// If a weapon needs reloading and the player start reloading the weapon but then switches to another weapon
    /// before the previous weapon finished reloading, the coroutine will start again,
    /// so this method prevents that.
    /// This prevents the meme, "is faster to switch to another weapon than reloading."
    /// I could let the meme live, but I'll see.
    /// </summary>
    private void ActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        if (setActiveWeaponEventArgs.weapon.isWeaponReloading)
        {
            var cancellationToken = new CancellationTokenSource();

            ReloadWeaponAsync(setActiveWeaponEventArgs.weapon, 0, cancellationToken);

            //if (reloadWeaponCoroutine != null)
            //{
            //    StopCoroutine(reloadWeaponCoroutine);
            //}
            //reloadWeaponCoroutine = StartCoroutine(ReloadingWeaponCoroutine(setActiveWeaponEventArgs.weapon, 0));
        }
    }
}
