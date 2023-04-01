using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using SnakeGame;
using SnakeGame.Interfaces;
using SnakeGame.AbwehrSystem.Ammo;
using SnakeGame.AudioSystem;
using SnakeGame.VisualEffects;

[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(WeaponFiredEvent))]
[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
    private float firePrechargeTimer = 0f;
    private float fireRateCooldownTimer = 0f;
    private ActiveWeapon activeWeapon;
    private FireWeaponEvent fireWeaponEvent;
    private ReloadWeaponEvent reloadWeaponEvent;
    private WeaponFiredEvent weaponFiredEvent;

    private void Awake()
    {
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
    }

    private void Update()
    {
        fireRateCooldownTimer -= Time.deltaTime;
    }

    private void OnEnable()
    {
        fireWeaponEvent.OnFire += FireWeaponEvent_OnFire;
    }

    private void OnDisable()
    {
        fireWeaponEvent.OnFire -= FireWeaponEvent_OnFire;
    }

    /// <summary>
    /// Handles The Firing Weapon Event
    /// </summary>
    private void FireWeaponEvent_OnFire(FireWeaponEvent fireWeaponEvent, FireWeaponEventArgs fireWeaponEventArgs)
    {
        WeaponFire(fireWeaponEventArgs);
    }

    /// <summary>
    /// Fires the weapon
    /// </summary>
    /// <param name="fireWeaponEventArgs"></param>
    private void WeaponFire(FireWeaponEventArgs fireWeaponEventArgs)
    {
        WeaponPrecharge(fireWeaponEventArgs);

        if (WeaponReadyToFire())
        {
            FireAmmo(fireWeaponEventArgs.aimAngle, fireWeaponEventArgs.weaponAimAngle, fireWeaponEventArgs.weaponAimDirectionVector);

            // TODO - Shake the camera when fired
            //MyCameraShake.Instance.ShakeCamera(firingWeaponEventArgs.weaponShootEffectSO.intensity,
            //    firingWeaponEventArgs.weaponShootEffectSO.time,
            //    firingWeaponEventArgs.weaponShootEffectSO.enableShake);

            ResetCooldownTimer();

            ResetPrechargeTimer();
        }
    }

    /// <summary>
    /// Sets Up The Bullet/Ammo Using A GameObject From The Object Pool
    /// </summary>
    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        AmmoDetailsSO currentAmmo = activeWeapon.GetCurrentAmmo();

        if (currentAmmo != null)
        {
            StartCoroutine(FireAmmoRoutine(currentAmmo, aimAngle, weaponAimAngle, weaponAimDirectionVector));
        }
    }

    /// <summary>
    /// Coroutine that enables the firing of the weapon, the Object Pool is used in this method
    /// </summary>
    /// <param name="currentAmmo">The current ammo for the weapon</param>
    /// <param name="aimAngle">The aim angle for the weapon</param>
    /// <param name="weaponAimAngle">The aim angle used to rotate the weapon</param>
    /// <param name="weaponAimDirectionVector">The vector aim angle for initializing the ammo</param>
    /// <returns></returns>
    private IEnumerator FireAmmoRoutine(AmmoDetailsSO currentAmmo, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        int ammoCounter = 0;

        //Get a random number of bullets that are gonna be fired, per shoot
        int bulletsPerShoot = Random.Range(currentAmmo.MinBulletsPerShoot, currentAmmo.MaxBulletsPerShoot + 1);

        //Get a random interval that the bullets are gonna spawn with
        float ammoSpawnInterval;

        if (bulletsPerShoot > 1)
        {
            ammoSpawnInterval = Random.Range(currentAmmo.minSpawnInterval, currentAmmo.maxSpawnInterval);
        }
        else
        {
            ammoSpawnInterval = 0f;
        }

        //Loop for the number of bullets that are gonna be fired
        while (ammoCounter < bulletsPerShoot)
        {
            ammoCounter++;

            // Get a random ammo prefab from the array in the Ammo Details SO
            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];

            // Get a random speed btw the min and max values in the Ammo Details SO
            float ammoSpeed = Random.Range(currentAmmo.minAmmoSpeed, currentAmmo.maxAmmoSpeed);

            // Get the component with the IFireable interface
            IFireable ammo = (IFireable)PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon.GetFirePosition(), Quaternion.identity);

            // Initialise the ammo
            ammo.InitialiseAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);

            // Wait for the ammo per shoot timegap
            yield return new WaitForSeconds(ammoSpawnInterval);
        }

        //Reduce the ammo mag capacity if it doesn't have infinity mag capacity
        if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfinityClipCapacity)
        {
            activeWeapon.GetCurrentWeapon().weaponClipRemaining--;
            activeWeapon.GetCurrentWeapon().weaponTotalAmmoCapacity--;
        }

        //Call the fired weapon event
        weaponFiredEvent.CallWeaponFiredEvent(activeWeapon.GetCurrentWeapon());

        // Display the weapon shoot effect
        WeaponShootEffects(aimAngle);

        // Play the sound effects specified for this weapon
        WeaponSoundEffects();
    }

    private bool WeaponReadyToFire()
    {
        //If there's no more remaining ammo and we dont have infinite ammo, then retur false
        if (activeWeapon.GetCurrentWeapon().weaponTotalAmmoCapacity <= 0 && !activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteAmmo)
            return false;

        //If the weapon is reloading then return false
        if (activeWeapon.GetCurrentWeapon().isWeaponReloading)
            return false;

        //If the weapon fire rate timer is greater than zero or if the weapon precharge timer is greater than zero, return false
        if (fireRateCooldownTimer > 0f || firePrechargeTimer > 0)
            return false;

        //If the ammo remaining in the mag is zero and the weapon doesn't have infinity mag capacity, then return false
        if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfinityClipCapacity && activeWeapon.GetCurrentWeapon().weaponClipRemaining <= 0)
        {
            reloadWeaponEvent.CallReloadEvent(activeWeapon.GetCurrentWeapon(), 0);
            return false;
        }

        //Weapon is finally ready to fire, return true
        return true;
    }

    private void WeaponPrecharge(FireWeaponEventArgs fireWeaponEventArgs)
    {
        if (fireWeaponEventArgs.firedPreviousFrame)
        {
            firePrechargeTimer -= Time.deltaTime;
        }
        else
        {
            ResetPrechargeTimer();
        }
    }

    private void ResetCooldownTimer()
    {
        fireRateCooldownTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
    }

    private void ResetPrechargeTimer()
    {
        firePrechargeTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponPrechargeTime;
    }

    private void WeaponShootEffects(float aimAngle)
    {
        if (activeWeapon.GetCurrentWeapon().weaponDetails.shootEffect != null 
            && activeWeapon.GetCurrentWeapon().weaponDetails.shootEffect.shootEffectPrefab != null)
        {
            // Get weapon shoot effect gameobject from the pool with particle system component
            WeaponShootEffect weaponShootEffect = (WeaponShootEffect)PoolManager.Instance.ReuseComponent(activeWeapon.GetCurrentWeapon().weaponDetails.shootEffect.shootEffectPrefab,
                activeWeapon.GetWeaponFireEffectPosition(), Quaternion.identity);

            weaponShootEffect.SetShootEffect(activeWeapon.GetCurrentWeapon().weaponDetails.shootEffect, aimAngle);

            // Set gameobject active (the particle system is set to automatically disable the
            // gameobject once finished)
            weaponShootEffect.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Play The Sound Effects For This Weapon
    /// </summary>
    private void WeaponSoundEffects()
    {
        if (activeWeapon.GetCurrentWeapon().weaponDetails.fireSound != null)
        {
            SoundEffectManager.CallOnSoundEffectSelectedEvent(activeWeapon.GetCurrentWeapon().weaponDetails.fireSound);
        }
    }
}
