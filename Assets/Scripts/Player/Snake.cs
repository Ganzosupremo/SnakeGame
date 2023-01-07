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
    [HideInInspector] public SnakeBody snakeBody;

    public List<Weapon> weaponList = new();
    public GameObject foodPrefab;
    public GameObject snakeBodyPrefab;

    private List<Transform> childList = new();

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
        snakeBody = snakeBodyPrefab.GetComponent<SnakeBody>();
    }
    
    public void SpawnFood()
    {
        // Know in which room the food should spawn
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Vector3 spawnPosition = new(Random.Range(currentRoom.tilemapLowerBounds.x, currentRoom.tilemapUpperBounds.x),
            Random.Range(currentRoom.tilemapLowerBounds.y, currentRoom.tilemapUpperBounds.y), 0f);

        // Make sure the food spawns within the room
        Food food = (Food)PoolManager.Instance.ReuseComponent(foodPrefab, HelperUtilities.GetNearestSpawnPointPosition(spawnPosition),
            Quaternion.identity);

        food.gameObject.SetActive(true);
    }

    /// <summary>
    /// Initialises the snake
    /// </summary>
    /// <param name="snakeDetails">The snake details to initialize</param>
    public void Initialize(SnakeDetailsSO snakeDetails)
    {
        this.snakeDetails = snakeDetails;


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

    public void EatFood()
    {
        foodPrefab.SetActive(false);
        var body = Instantiate(snakeBodyPrefab, transform.position, Quaternion.identity);
        body.GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(ActivateBodyCollider(body.transform));
        body.GetComponent<SnakeBody>().WaitHeadUpdateCicle(childList.Count);
        childList.Add(body.transform);
        SpawnFood();
    }

    private IEnumerator ActivateBodyCollider(Transform body)
    {
        yield return new WaitForSeconds(0.8f);
        body.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void SetChildList(List<Transform> childList)
    {
        this.childList = childList;
    }
}
