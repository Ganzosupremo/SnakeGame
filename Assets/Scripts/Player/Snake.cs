using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

/// <summary>
/// This class acts like an interface for other scripts
/// </summary>
#region Required Components
[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SnakeControler))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MovementToPosition))]
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
#endregion
public class Snake : MonoBehaviour
{
    [HideInInspector] public Animator animator;
    [HideInInspector] public SnakeDetailsSO snakeDetails;
    [HideInInspector] public SnakeControler snakeControler;
    [HideInInspector] public Health health;
    [HideInInspector] public MovementByVelocityEvent movementByVelocityEvent;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
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

    private List<Transform> snakeSegmentsList = new();
    private int snakeSegmentCount;

    public bool IsSnakeColliding { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        snakeControler = GetComponent<SnakeControler>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        fireWeapon = GetComponent<FireWeapon>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        reloadWeapon = GetComponent<ReloadWeapon>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        snakeSegmentsList.Add(this.transform);
        snakeSegmentCount = 0;
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
        if (other.collider.CompareTag(Settings.CollisionTilemapTag))
            SubstractSegmentFromSnake();
    }

    /// <summary>
    /// Initialises the snake
    /// </summary>
    /// <param name="snakeDetails">The snake details to initialize</param>
    public void Initialize(SnakeDetailsSO snakeDetails)
    {
        this.snakeDetails = snakeDetails;

        IsSnakeColliding = false;

        SetPlayerHealth();

        //Initialise the initial weapon for the player
        CreatePlayerInitialWeapon();
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

    private void SetPlayerHealth()
    {
        health.SetStartingHealth(snakeDetails.snakeInitialHealth);
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

        SnakeBody snakeBody = (SnakeBody)PoolManager.Instance.ReuseComponent(GameResources.Instance.snakeBodyPrefab.gameObject,
            this.transform.position, Quaternion.identity);
        snakeBody.gameObject.SetActive(true);
        
        Transform segment = snakeBody.transform;
        segment.position = snakeSegmentsList[snakeSegmentCount].position;
        snakeSegmentsList.Add(segment);
        snakeSegmentCount++;

        IncreaseWeaponDamage();
    }


    private void SubstractSegmentFromSnake()
    {
        if (snakeSegmentsList.Count > 0 && snakeSegmentCount > 0)
        {
            bool bodyFound = snakeSegmentsList[snakeSegmentCount].TryGetComponent(out SnakeBody body);
            if (bodyFound)
            {
                body.gameObject.SetActive(false);
                snakeSegmentsList.RemoveAt(snakeSegmentCount);
                snakeSegmentCount--;

                DecreaseWeaponDamage();
            }
        }
        else
        {
            Debug.Log("No more lives left");
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

    public void UpdateSnakeSegments()
    {
        for (int i = snakeSegmentsList.Count - 1; i > 0; i--)
        {
            snakeSegmentsList[i].position = snakeSegmentsList[i - 1].position;
        }
    }
    // TODO After training, change the update of the snake segments to this script
    // and just call this new method from the snake controler
    // add an int to keep track of how many segment the snake currently has
    // so when the snake collides, the last segment can be removed from the list
    // and deactivate it, so it returns to the pool.
}
