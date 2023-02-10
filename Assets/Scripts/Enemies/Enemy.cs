using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

#region Required Components
[RequireComponent(typeof(EnemyMovementAI))]
[RequireComponent(typeof(EnemyWeaponAI))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(DestroyEvent))]
[RequireComponent(typeof(Destroy))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
//[RequireComponent(typeof(AnimateEnemy))]
[RequireComponent(typeof(MaterializeEffect))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(SortingGroup))]
//[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(DealDamageOnContact))]
[RequireComponent(typeof(ReceiveDamageOnContact))]
#endregion
public class Enemy : MonoBehaviour
{
    [HideInInspector] public EnemyDetailsSO enemyDetails;
    [HideInInspector] public SpriteRenderer[] spriteRendererArray;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public FireWeaponEvent fireWeaponEvent;
    [HideInInspector] public FireWeapon fireWeapon;

    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public EnemyMovementAI enemyMovementAI;

    [Tooltip("The transform that will rotate with the aimAngle.")]
    [SerializeField] private Transform enemyRotateTransform;
    [SerializeField] private SpriteRenderer enemyWeaponSprite;

    private MaterializeEffect materializeEffect;
    private CircleCollider2D triggerCollider;
    private CircleCollider2D solidCollider;


    private SetActiveWeaponEvent setActiveWeaponEvent;
    private HealthEvent healthEvent;
    private Health health;

    private void Awake()
    {
        enemyMovementAI = GetComponent<EnemyMovementAI>();
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();

        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        fireWeapon = GetComponent<FireWeapon>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        idleEvent = GetComponent<IdleEvent>();

        materializeEffect = GetComponent<MaterializeEffect>();
        solidCollider = GetComponentInChildren<CircleCollider2D>();
        triggerCollider = GetComponent<CircleCollider2D>();
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
        //aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
    }


    private void OnDisable()
    {
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
        //aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (healthEventArgs.healthAmount <= 0f)
        {
            EnemyDestroyed();
        }
    }

    //private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    //{
    //    RotateEnemy(aimWeaponEventArgs.aimAngle);
    //}

    ///// <summary>
    ///// Rotates the enemy to face the fire direction
    ///// </summary>
    ///// <param name="aimAngle"></param>
    //private void RotateEnemy(float aimAngle)
    //{
    //    enemyRotateTransform.eulerAngles = new(0f, 0f, aimAngle);
    //}

    public void InitializeEnemy(EnemyDetailsSO enemyDetails, int enemySpawnNumber, GameLevelSO gameLevel)
    {
        this.enemyDetails = enemyDetails;

        SetEnemyMovementUpdateFrame(enemySpawnNumber);

        SetEnemyStartingHealth(gameLevel);

        SetEnemyStartingWeapon();

        // Calls the materialize effect class
        StartCoroutine(MaterializeEnemy());

        //SetPolygonColliderShape();
    }

    /// <summary>
    /// Set the enemy movement update frame
    /// </summary>
    private void SetEnemyMovementUpdateFrame(int enemySpawnNumber)
    {
        //Set the frame on which the enemy will process it's updates
        enemyMovementAI.UpdateFramesNumber(enemySpawnNumber % Settings.targetFramesToSpreadPathfindingOver);
    }

    /// <summary>
    /// Sets the starting health for the enemy on this specific game level
    /// </summary>
    /// <param name="gameLevel"></param>
    private void SetEnemyStartingHealth(GameLevelSO gameLevel)
    {
        foreach (ItemHealthDetails itemHealth in enemyDetails.enemyHealthDetailsArray)
        {
            if (itemHealth.gameLevel == gameLevel)
            {
                health.SetStartingHealth(itemHealth.healthAmount);
                return;
            }
        }

        health.SetStartingHealth(Settings.defaultEnemyHealth);
    }

    /// <summary>
    /// Set the enemy starting weapon with the weapon details SO
    /// </summary>
    private void SetEnemyStartingWeapon()
    {
        //Proceed if the enemy has a weapon
        if (enemyDetails.enemyWeapon != null)
        {
            Weapon weapon = new()
            {
                weaponDetails = enemyDetails.enemyWeapon,
                weaponReloadTimer = 0f,
                weaponClipRemaining = enemyDetails.enemyWeapon.clipMaxCapacity,
                weaponTotalAmmoCapacity = enemyDetails.enemyWeapon.totalAmmoCapacity,
                isWeaponReloading = false
            };

            //Set the weapon for the enemy
            setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);
        }
    }

    //private void SetPolygonColliderShape()
    //{
    //    if (solidCollider != null)
    //    {
    //        List<Vector2> polygonVertices = new();
    //        enemyWeaponSprite.sprite.GetPhysicsShape(0, polygonVertices);

    //        solidCollider.points = polygonVertices.ToArray();
    //    }
    //}

    private IEnumerator MaterializeEnemy()
    {
        // Disables the enemy while it's been materialized
        EnableEnemy(false);

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(enemyDetails.enemyMaterializeShader, enemyDetails.enemyMaterializeColor,
            enemyDetails.enemyMaterializeTime, enemyDetails.standardEnemyMaterial, spriteRendererArray));

        // Enables the enemy again, after it has been materialzed
        EnableEnemy(true);
    }

    private void EnableEnemy(bool isEnabled)
    {
        // Enable/Disable the colliders
        triggerCollider.enabled = isEnabled;
        solidCollider.enabled = isEnabled;

        // Enable/Disable the enemy movement AI
        enemyMovementAI.enabled = isEnabled;

        // Enable/Disable the fire weapon
        fireWeapon.enabled = isEnabled;
    }

    private void EnemyDestroyed()
    {
        DestroyEvent destroyEvent = GetComponent<DestroyEvent>();
        destroyEvent.CallOnDestroy(false, 0);
    }
}
