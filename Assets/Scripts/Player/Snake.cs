using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using SnakeGame;

/// <summary>
/// This class acts like an interface for other scripts
/// </summary>
#region Required Components
[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
//[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SnakeControler))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(Destroy))]
[RequireComponent(typeof(DestroyEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(MaterializeEffect))]
#endregion
public class Snake : MonoBehaviour
{
    #region Components
    [HideInInspector] public Animator animator;
    [HideInInspector] public SnakeDetailsSO snakeDetails;
    [HideInInspector] public SnakeControler snakeControler;
    [HideInInspector] public Health health;
    [HideInInspector] public HealthEvent healthEvent;
    [HideInInspector] public Destroy destroy;
    [HideInInspector] public DestroyEvent destroyEvent;
    [HideInInspector] public MovementByVelocityEvent movementByVelocityEvent;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public Idle idle;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public SetActiveWeaponEvent setActiveWeaponEvent;
    [HideInInspector] public ActiveWeapon activeWeapon;
    [HideInInspector] public FireWeaponEvent fireWeaponEvent;
    [HideInInspector] public FireWeapon fireWeapon;
    [HideInInspector] public WeaponFiredEvent weaponFiredEvent;
    [HideInInspector] public ReloadWeaponEvent reloadWeaponEvent;
    [HideInInspector] public ReloadWeapon reloadWeapon;
    [HideInInspector] public WeaponReloadedEvent weaponReloadedEvent;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public List<Weapon> weaponList = new();
    [HideInInspector] public SnakeBody snakeBody;
    #endregion

    private List<Transform> snakeSegmentsList = new();
    private int snakeSegmentCount;
    private MaterializeEffect materializeEffect;

    public bool IsSnakeColliding { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        healthEvent = GetComponent<HealthEvent>();
        destroy = GetComponent<Destroy>();
        destroyEvent = GetComponent<DestroyEvent>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        snakeControler = GetComponent<SnakeControler>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        idle = GetComponent<Idle>();
        idleEvent = GetComponent<IdleEvent>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        fireWeapon = GetComponent<FireWeapon>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        reloadWeapon = GetComponent<ReloadWeapon>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        materializeEffect = GetComponent<MaterializeEffect>();

        //ResetSnakeSegments(false);
    }

    private void Start()
    {
        //snakeSegmentsList.Add(this.transform);
        //snakeSegmentCount = 0;
        ResetSnakeSegments(false);
    }

    private void OnEnable()
    {
        healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        Debug.Log("Health remaining: " + healthEventArgs.healthAmount);

        if (healthEventArgs.healthAmount <= 0f)
        {
            destroyEvent.CallOnDestroy(true, 0);
            ResetSnakeSegments(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Settings.food))
        {
            GrowSnake();
            //IsSnakeColliding = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag(Settings.CollisionTilemapTag) || other.collider.CompareTag(Settings.enemyTag))
            SubstractSegmentFromSnake();
    }

    /// <summary>
    /// Initialises the snake
    /// </summary>
    /// <param name="snakeDetails">The snake details to initialize</param>
    public void Initialize(SnakeDetailsSO snakeDetails)
    {
        this.snakeDetails = snakeDetails;

        snakeSegmentsList.Add(this.transform);
        snakeSegmentCount = 0;

        IsSnakeColliding = false;

        SetPlayerHealth();

        //ResetSnakeSegments(false);

        // Initialise the initial weapon for the player
        CreatePlayerInitialWeapon();

        StartCoroutine(MaterializeSnake());
    }

    /// <summary>
    /// Sets The Player Initial Weapon 
    /// </summary>
    private void CreatePlayerInitialWeapon()
    {
        weaponList.Clear();

        foreach (WeaponDetailsSO weaponDetails in snakeDetails.initialWeaponList)
        {
            //Add the weapon to the player to use
            AddWeaponToPlayer(weaponDetails);
        }
    }

    private IEnumerator MaterializeSnake()
    {
        // Disable snake while the effect is in progress
        EnableSnake(false);
        yield return StartCoroutine(materializeEffect.MaterializeRoutine(snakeDetails.materializeShader,
            snakeDetails.materializeColor, snakeDetails.materializeTime, snakeDetails.defaultLitMaterial, spriteRenderer));
        // Enable the snake again
        EnableSnake(false);
    }

    private void SetPlayerHealth()
    {
        health.SetStartingHealth(snakeDetails.snakeInitialHealth);
    }

    /// <summary>
    /// Gets the player position
    /// </summary>
    /// <returns>Returns the current player position</returns>
    public Vector3 GetSnakePosition()
    {
        return transform.position;
    }

    public Weapon AddWeaponToPlayer(WeaponDetailsSO weaponDetails)
    {
        Weapon weapon = new()
        {
            weaponDetails = weaponDetails,
            weaponReloadTimer = 0f,
            weaponClipRemaining = weaponDetails.clipMaxCapacity,
            weaponTotalAmmoCapacity = weaponDetails.totalAmmoCapacity,
            isWeaponReloading = false
        };
        // Adds the weapon to the weapon list
        weaponList.Add(weapon);

        // Set the position of the newly added weapon in the list
        weapon.weaponListPosition = weaponList.Count;
        
        // Activate the newly added weapon
        setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);
        return weapon;
    }

    private void GrowSnake()
    {
        // TODO - Add More health and weapon damage when the snake grows
        snakeBody = (SnakeBody)PoolManager.Instance.ReuseComponent(GameResources.Instance.snakeBodyPrefab.gameObject,
            this.transform.position, Quaternion.identity);
        snakeBody.gameObject.SetActive(true);
        
        Transform segment = snakeBody.transform;
        segment.position = snakeSegmentsList[snakeSegmentCount].position;
        snakeSegmentsList.Add(segment);
        snakeSegmentCount++;

        health.IncreaseHealth(1);

        IncreaseWeaponDamage();
    }

    public void UpdateSnakeSegments()
    {
        for (int i = snakeSegmentsList.Count - 1; i > 0; i--)
        {
            snakeSegmentsList[i].position = snakeSegmentsList[i - 1].position;
        }
    }

    private void SubstractSegmentFromSnake()
    {
        if (snakeSegmentsList.Count > 0 && snakeSegmentCount > 0)
        {
            if (!health.IsDamageable) return;

            //snakeSegmentsList[snakeSegmentCount].TryGetComponent(out SnakeBody snakeBody);

            snakeBody.gameObject.SetActive(false);
            snakeSegmentsList.RemoveAt(snakeSegmentCount);
            snakeSegmentCount--;

            health.TakeDamage(1);
            // Reduce the multiplier when the player gets hit,
            // because the player at some point will run out of ammo
            // we do it here, so the multiplier can still be updated
            StaticEventHandler.CallMultiplierEvent(false);
            DecreaseWeaponDamage();
        }
        else
        {
            destroyEvent.CallOnDestroy(true, 0);
        }
    }

    private void ResetSnakeSegments(bool dead)
    {
        for (int i = 1; i < snakeSegmentsList.Count; i++)
        {
            snakeSegmentsList[i].gameObject.SetActive(false);
        }

        snakeSegmentsList.Clear();
        snakeSegmentsList.Add(this.transform);

        if (dead) return;

        // We add more segments to the snake on the beginning
        for (int i = 1; i < health.CurrentHealth; i++)
        {
            snakeBody = (SnakeBody)PoolManager.Instance.ReuseComponent(GameResources.Instance.snakeBodyPrefab.gameObject,
            this.transform.position, Quaternion.identity);
            snakeBody.gameObject.SetActive(true);

            Transform segment = snakeBody.transform;
            segment.position = snakeSegmentsList[snakeSegmentCount].position;
            snakeSegmentsList.Add(segment);
            snakeSegmentCount++;
        }
    }

    /// <summary>
    /// Decreases the damage of the weapon when collided with something.
    /// </summary>
    private void DecreaseWeaponDamage()
    {
        Weapon currentWeapon = activeWeapon.GetCurrentWeapon();
        currentWeapon.weaponDetails.weaponCurrentAmmo.DecreaseDamage(20);
    }

    /// <summary>
    /// Increase the damage of the weapon when a food was eated.
    /// </summary>
    private void IncreaseWeaponDamage()
    {
        Weapon currentWeapon = activeWeapon.GetCurrentWeapon();
        currentWeapon.weaponDetails.weaponCurrentAmmo.IncreaseDamage(20);
    }

    private void EnableSnake(bool isActive)
    {
        snakeControler.enabled = isActive;
        fireWeapon.enabled = isActive;
    }
}
