using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SnakeGame.VisualEffects;
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

    public List<SnakeBody> SnakeBodyList { get; private set; } = new();
    public List<Transform> SnakeSegmentsList { get; private set; } = new();
    public bool IsSnakeColliding { get; private set; }

    private int snakeSegmentCount;
    private MaterializeEffect materializeEffect;
    private SnakeControler snakeControler;

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
    }

    private void Start()
    {
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
            IncreaseWeaponDamage(20);
            //IsSnakeColliding = true;
        }
        else if (other.CompareTag(Settings.goldenFood))
        {
            GrowSnake();
            IncreaseWeaponDamage(40);
        }
        else if (other.CompareTag(Settings.voidFood))
        {
            GrowSnake();
            IncreaseWeaponDamage(60);
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

        SnakeSegmentsList.Add(this.transform);
        snakeSegmentCount = 0;

        IsSnakeColliding = false;

        SetPlayerHealth();

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
        EnableSnake(true);
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

    /// <summary>
    /// Gets the snake controler script,
    /// in this script all the input is handled.
    /// </summary>
    /// <returns></returns>
    public SnakeControler GetSnakeControler()
    {
        return snakeControler;
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
        snakeBody = (SnakeBody)PoolManager.Instance.ReuseComponent(GameResources.Instance.snakeBodyPrefab.gameObject,
            this.transform.position, Quaternion.identity);
        snakeBody.gameObject.SetActive(true);
        SnakeBodyList.Add(snakeBody);
        
        Transform segment = snakeBody.transform;
        segment.position = SnakeSegmentsList[snakeSegmentCount].position;
        SnakeSegmentsList.Add(segment);
        snakeSegmentCount++;

        health.IncreaseHealth(1);
    }

    public void UpdateSnakeSegments()
    {
        for (int i = SnakeSegmentsList.Count - 1; i > 0; i--)
        {
            SnakeSegmentsList[i].position = SnakeSegmentsList[i - 1].position;
        }
    }

    private void SubstractSegmentFromSnake()
    {
        if (SnakeSegmentsList.Count > 0 && snakeSegmentCount > 0)
        {
            if (!health.IsDamageable) return;

            //snakeSegmentsList[snakeSegmentCount].TryGetComponent(out SnakeBody snakeBody);

            snakeBody.gameObject.SetActive(false);
            SnakeSegmentsList.RemoveAt(snakeSegmentCount);
            if (SnakeBodyList.Count > 0)
                SnakeBodyList.Remove(snakeBody);

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
        for (int i = 1; i < SnakeSegmentsList.Count; i++)
        {
            SnakeSegmentsList[i].gameObject.SetActive(false);
        }

        SnakeSegmentsList.Clear();
        SnakeBodyList.Clear();
        SnakeSegmentsList.Add(this.transform);

        if (dead) return;

        // We add more segments to the snake on the beginning
        for (int i = 1; i < health.CurrentHealth; i++)
        {
            snakeBody = (SnakeBody)PoolManager.Instance.ReuseComponent(GameResources.Instance.snakeBodyPrefab.gameObject,
            this.transform.position, Quaternion.identity);
            snakeBody.gameObject.SetActive(true);
            SnakeBodyList.Add(snakeBody);

            Transform segment = snakeBody.transform;
            segment.position = SnakeSegmentsList[snakeSegmentCount].position;
            SnakeSegmentsList.Add(segment);
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
    private void IncreaseWeaponDamage(int percentageToincrease)
    {
        Weapon currentWeapon = activeWeapon.GetCurrentWeapon();
        currentWeapon.weaponDetails.weaponCurrentAmmo.IncreaseDamage(percentageToincrease);
    }

    private void EnableSnake(bool isActive)
    {
        snakeControler.IsSnakeEnabled = isActive;
        snakeControler.enabled = isActive;
        fireWeapon.enabled = isActive;
    }
}