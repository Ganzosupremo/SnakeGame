using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class Ammo : MonoBehaviour, IFireable
{
    #region Tooltip
    [Tooltip("Populate with the child component TrailRenderer, that is found in the ammo prefab")]
    #endregion
    [SerializeField] private TrailRenderer trailRenderer;

    private float ammoRange = 0;
    private float ammoSpeed;
    private Vector3 aimDirectionVector;
    private float fireDirectionAngle;
    private SpriteRenderer spriteRenderer;
    private AmmoDetailsSO ammoDetails;
    private float ammoChargeTimer;
    private bool hasAmmoMaterialSet = false;
    private bool overrideAmmoMovement;
    private bool isColliding = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (ammoChargeTimer > 0)
        {
            ammoChargeTimer -= Time.deltaTime;
            return;
        }
        else if (!hasAmmoMaterialSet)
        {
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            hasAmmoMaterialSet = true;
        }

        //Don't move the ammo if the movement has been overriden, meaning this ammo is part of an ammo pattern
        if (!overrideAmmoMovement)
        {
            //Calculate distance vector to move the bullet
            Vector3 distanceVector = ammoSpeed * Time.deltaTime * aimDirectionVector;

            transform.position += distanceVector;

            //Disable after the max range has been reached
            ammoRange -= distanceVector.magnitude;

            if (ammoRange < 0)
            {
                //if (ammoDetails.isPlayerAmmo)
                //    //Call the multiply score event
                //    StaticEventHandler.CallMultiplierEvent(false);

                DisableAmmo();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isColliding) return;

        DisableAmmo();
    }

    /// <summary>
    /// This Method Initialises The Ammo So It Can Be Fired, Using The Specified Variables.
    /// If This Ammo Is Part Of An Ammo Pattern, The Ammo Movement Can Be Overriden By Setting The Bool overrideAmmoMovement To True
    /// </summary>
    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
    {
        #region Ammo
        this.ammoDetails = ammoDetails;

        isColliding = false;

        //Sets the fire direction
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        spriteRenderer.sprite = ammoDetails.ammoSprite;

        if (ammoDetails.ammoChargeTime > 0)
        {
            ammoChargeTimer = ammoDetails.ammoChargeTime;
            SetAmmoMaterial(ammoDetails.ammoChargeMaterial);
            hasAmmoMaterialSet = false;
        }
        else
        {
            ammoChargeTimer = 0f;
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            hasAmmoMaterialSet = true;
        }
        // Set the ammo Range
        ammoRange = ammoDetails.ammoRange;
        // Set ammo speed
        this.ammoSpeed = ammoSpeed;
        // Override the ammo movement
        this.overrideAmmoMovement = overrideAmmoMovement;

        gameObject.SetActive(true);
        #endregion

        #region Ammo Trail
        if (ammoDetails.hasAmmoTrail)
        {
            trailRenderer.gameObject.SetActive(true);
            trailRenderer.emitting = true;
            trailRenderer.material = ammoDetails.ammoTrailMaterial;
            trailRenderer.startWidth = ammoDetails.ammoTrailStartWidth;
            trailRenderer.endWidth = ammoDetails.ammoTrailEndWidth;
            trailRenderer.time = ammoDetails.ammoTrailLifetime;
        }
        else
        {
            trailRenderer.emitting = false;
            trailRenderer.gameObject.SetActive(false);
        }
        #endregion
    }

    /// <summary>
    /// Set The Ammo Fire Direction Based On The Input Angle And Direction Adjusted By The Random Spread.
    /// </summary>
    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        float spreadRandomAngle = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

        //Get a random toggle between 1 or -1
        int spreadRandomToggle = Random.Range(0, 2) * 2 - 1;

        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
        {
            fireDirectionAngle = aimAngle;
        }
        else
        {
            fireDirectionAngle = weaponAimAngle;
        }
        
        // Adjust the bullet fire angle with the random spread
        fireDirectionAngle += spreadRandomToggle * spreadRandomAngle;

        //Set the bullet rotation if any
        transform.eulerAngles = new Vector3(0f, 0f, fireDirectionAngle);

        //Set the bullet fire direction
        aimDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
    }

    private void DisableAmmo()
    {
        gameObject.SetActive(false);
    }

    private void SetAmmoMaterial(Material ammoMaterial)
    {
        spriteRenderer.material = ammoMaterial;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(trailRenderer), trailRenderer);
    }
#endif
    #endregion
}
