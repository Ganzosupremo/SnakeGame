using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using SnakeInput;
/// <summary>
/// This scripts controls all the input of the player using a unity built-in
/// Player Input
/// </summary>
[RequireComponent(typeof(Snake))]
[DisallowMultipleComponent]
public class SnakeControler : MonoBehaviour
{
    [SerializeField] MovementDetailsSO movementDetails;
    [SerializeField] private Transform weaponShootPosition;

    private Snake snake;
    private bool fireButtonPressedPreviousFrame = false;
    private int currentWeaponIndex = 1;
    private float moveSpeed;
    private Vector2 savedDirection;
    private Vector2 moveDirection;

    private Coroutine snakeDashCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private bool isSnakeDashing = false;
    private float dashCooldownTimer = 0f;

    private List<Transform> snakeBodyList = new();

    private SnakeControl snakeControl;
    private SnakeBody snakeBody;

    private void Awake()
    {
        snakeControl = new();
        snakeControl.Snake.Enable();
        
        snake = GetComponent<Snake>();
        //savedDirection = new Vector2(0.01f, 0f);
    }

    private void Start()
    {
        waitForFixedUpdate = new();
        savedDirection = new Vector2(1f, 0f);
        snakeBody = snake.snakeBody;

        SetInitialWeapon();

        SetAnimatorSpeed();
    }

    private void FixedUpdate()
    {
        OnMove();
    }

    private void Update()
    {
        OnDash();

        WeaponInputs();

        SnakeDashCooldownTimer();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food"))
        {
            snake.EatFood();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        StopDashCoroutine();
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        StopDashCoroutine();
    }

    /// <summary>
    /// Sets the initial weapon of the player
    /// </summary>
    private void SetInitialWeapon()
    {
        int index = 1;

        foreach (Weapon weapon in snake.weaponList)
        {
            if (weapon.weaponDetails == snake.snakeDetails.initialWeapon)
            {
                SetWeaponByIndex(index);
                break;
            }
            index++;
        }
    }

    private void SetAnimatorSpeed()
    {
        snake.animator.speed = moveSpeed / Settings.playerAnimationSpeed;
    }

    /// <summary>
    /// Moves the snake to the <see cref="moveDirection"/> position using the <seealso cref="MovementByVelocityEvent"/>.
    /// </summary>
    public void OnMove()
    {
        // If the snake is dashing, no other movement will be precessed
        if (isSnakeDashing) return;

        moveDirection = snakeControl.Snake.Move.ReadValue<Vector2>();

        // Check if the movement is allowed
        if (IsMovementAllowed(moveDirection))
        {
            savedDirection = moveDirection;
            moveSpeed = movementDetails.GetMoveSpeed();

            SetSnakeBodyTarget();

            snake.movementByVelocityEvent.MovementByVelocity(savedDirection, moveSpeed);
            RotateBody(savedDirection);
        }
    }

    private void WeaponInputs()
    {
        OnAim();

        OnReload();

        OnSwitchWeapon();
    }

    /// <summary>
    /// Aims the weapon at the position of the mouse,
    /// also handles the firing of the weapon.
    /// </summary>
    public void OnAim()
    {
        // If the snake is dashing, no other movement will be precessed
        if (isSnakeDashing) return;

        Vector3 weaponDirection;
        float weaponAngleDegrees, snakeAngleDegrees;
        AimDirection aimDirection;

        // Pass the values as 'out' so they are available again for additional processing in this method
        AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out snakeAngleDegrees, out aimDirection);

        FireWeaponInput(weaponDirection, weaponAngleDegrees, snakeAngleDegrees, aimDirection);
    }

    /// <summary>
    /// Reloads the weapon when the R key is pressed
    /// </summary>
    public void OnReload()
    {
        Weapon weapon = snake.activeWeapon.GetCurrentWeapon();

        if (weapon.isWeaponReloading) return;

        // If the remaining ammo is less than the weapon mag max capacity and we dont have infinity ammo, then just return
        if (weapon.weaponTotalAmmoCapacity < weapon.weaponDetails.clipMaxCapacity && !weapon.weaponDetails.hasInfiniteAmmo)
            return;

        // If the remaining ammo in the mag is equal to the mag max capacity, then return
        if (weapon.weaponClipRemaining == weapon.weaponDetails.clipMaxCapacity)
            return;

        if (snakeControl.Snake.Reload.WasPressedThisFrame())
            // Trigger the reload weapon event
            snake.reloadWeaponEvent.CallReloadEvent(snake.activeWeapon.GetCurrentWeapon(), 0);
    }

    /// <summary>
    /// Makes the snake dash, while dashing the snake is invincible
    /// and moves faster.
    /// </summary>
    public void OnDash()
    {
        if (snakeControl.Snake.Dash.WasPressedThisFrame() && dashCooldownTimer <= 0f)
            SnakeDash((Vector3)savedDirection);
    }

    /// <summary>
    /// Switches The Active Weapon With The Mouse Wheel
    /// </summary>
    public void OnSwitchWeapon()
    {
        Vector2 switchWeaponInput = snakeControl.Snake.SwitchWeapons.ReadValue<Vector2>();

        if (switchWeaponInput.y < 0f || switchWeaponInput.x < -0.5f)
            SelectPreviousWeapon();
        if (switchWeaponInput.y > 0f || switchWeaponInput.x > 0.5f)
            SelectNextWeapon();

        if (snakeControl.Snake.SetWeaponAtFirstPos.WasPressedThisFrame())
            SetWeaponFirstInList();
    }

    /// <summary>
    /// Check if the current move direction is allowed
    /// </summary>
    /// <param name="moveDirection"></param>
    /// <returns>Returns true if the given move direction is allowed</returns>
    private bool IsMovementAllowed(Vector2 moveDirection)
    {
        // If the snake is moving to the right, only allow up or down turns
        if (savedDirection == Vector2.right)
        {
            return moveDirection == Vector2.up || moveDirection == Vector2.down;
        }
        // If the snake is moving to the left, only allow up or down turns
        else if (savedDirection == Vector2.left)
        {
            return moveDirection == Vector2.up || moveDirection == Vector2.down;
        }
        // If the snake is moving up, only allow left or right turns
        else if (savedDirection == Vector2.up)
        {
            return moveDirection == Vector2.left || moveDirection == Vector2.right;
        }
        // If the snake is moving down, only allow left or right turns
        else if (savedDirection == Vector2.down)
        {
            return moveDirection == Vector2.left || moveDirection == Vector2.right;
        }

        return false;
    }

    /// <summary>
    /// Rotate the sprite to face the current move direction.
    /// Problem - It seems that if we rotate the snake with any aimAngle
    /// that is calculated in the AimWeaponInput
    /// the movement of the snake breaks,
    /// I guess is because we rotate both objects at the same time or something like that
    /// I should look for another way for rotating the head of the snake and the weapon
    /// without both breaking.
    /// One solution could be animatiions or changing how the snake moves.
    /// </summary>
    private void RotateBody(Vector3 moveDirection)
    {
        float angle = HelperUtilities.GetAngleFromVector(moveDirection);
        transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    /// <summary>
    /// Calls the <seealso cref="AimWeaponEvent"/> and aims the weapon at cursor position.
    /// </summary>
    /// <param name="weaponDirection"></param>
    /// <param name="weaponAngleDegrees"></param>
    /// <param name="snakeAngleDegrees"></param>
    /// <param name="aimDirection"></param>
    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float snakeAngleDegrees, out AimDirection aimDirection)
    {
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();

        //Calculate Direction vector of the mouse cursor from the weapon shoot position
        weaponDirection = (Vector3)mouseWorldPosition - snake.activeWeapon.GetFirePosition();

        //Calculate direction vector of mouse cursor from the snake transform position
        Vector3 playerDirection = (Vector3)mouseWorldPosition - transform.position;

        //Get angle from the weapon and the cursor
        weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

        //Get the angle from the player and the cursor
        snakeAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);

        //Set the player aim direction
        aimDirection = HelperUtilities.GetAimDirection(snakeAngleDegrees);

        //Trigger the weapon aim event
        snake.aimWeaponEvent.CallAimWeaponEvent(aimDirection, snakeAngleDegrees, weaponAngleDegrees, weaponDirection);
    }

    private void SnakeDash(Vector3 savedDirection)
    {
        if (snakeDashCoroutine != null)
        {
            StopCoroutine(snakeDashCoroutine);
        }
        snakeDashCoroutine = StartCoroutine(SnakeDashRoutine(savedDirection));
    }

    private IEnumerator SnakeDashRoutine(Vector3 savedDirection)
    {
        float minDistance = 0.2f;

        isSnakeDashing = true;

        Vector3 targetPosition = snake.transform.position + savedDirection * movementDetails.dashDistance;

        // Loop until the target position is reached
        while (Vector3.Distance(snake.transform.position, targetPosition) > minDistance)
        {
            snake.movementToPositionEvent.CallMovementToPosition(targetPosition, snake.transform.position, savedDirection, movementDetails.dashSpeed, isSnakeDashing);

            // Wait for the fixed update
            yield return waitForFixedUpdate;
        }

        isSnakeDashing = false;

        dashCooldownTimer = movementDetails.dashCooldownTime;

        snake.transform.position = targetPosition;
    }

    /// <summary>
    /// Reduces the <see cref="dashCooldownTimer"/>
    /// </summary>
    private void SnakeDashCooldownTimer()
    {
        if (dashCooldownTimer >= 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Stops the <see cref="snakeDashCoroutine"/> if a wall was hitted
    /// </summary>
    private void StopDashCoroutine()
    {
        if (snakeDashCoroutine != null)
        {
            StopCoroutine(snakeDashCoroutine);
            isSnakeDashing = false;
        }
    }

    private void SetWeaponByIndex(int index)
    {
        if (index - 1 < snake.weaponList.Count)
        {
            currentWeaponIndex = index;
            snake.setActiveWeaponEvent.CallSetActiveWeaponEvent(snake.weaponList[index - 1]);

            GameCursor.Instance.ChangeCrosshair(snake.activeWeapon.GetCurrentWeapon().weaponDetails.weaponCrosshair);
        }
    }

    private void FireWeaponInput(Vector3 weaponDirection,float weaponAngleDegrees, float snakeAngleDegrees, AimDirection aimDirection)
    {
        if (snakeControl.Snake.Fire.IsPressed())
        {
            // Trigger the fire weapon event
            snake.fireWeaponEvent.CallOnFireEvent(true, 
                fireButtonPressedPreviousFrame, 
                aimDirection, 
                snakeAngleDegrees, 
                weaponAngleDegrees, 
                weaponDirection);

            fireButtonPressedPreviousFrame = true;
        }
        else
            fireButtonPressedPreviousFrame = false;
    }

    /// <summary>
    /// Selects the next weapon in the weapon list
    /// </summary>
    private void SelectNextWeapon()
    {
        currentWeaponIndex++;

        if (currentWeaponIndex > snake.weaponList.Count)
            currentWeaponIndex = 1;

        SetWeaponByIndex(currentWeaponIndex);
    }

    /// <summary>
    /// Selects the previous weapon in the weapon list
    /// </summary>
    private void SelectPreviousWeapon()
    {
        currentWeaponIndex--;

        if (currentWeaponIndex < 1)
            currentWeaponIndex = snake.weaponList.Count;

        SetWeaponByIndex(currentWeaponIndex);
    }

    /// <summary>
    /// Sets the current weapon at the first position in the weapon list
    /// </summary>
    private void SetWeaponFirstInList()
    {
        List<Weapon> tempList = new();
        
        // Add current weapon to be the first in the temporary list
        Weapon weapon = snake.weaponList[currentWeaponIndex - 1];
        weapon.weaponListPosition = 1;
        tempList.Add(weapon);


        // Loop in the existing weapon list - skips the current weapon
        int index = 2;

        foreach (Weapon weapon1 in snake.weaponList)
        {
            if (weapon1 == weapon) continue;

            tempList.Add(weapon1);
            weapon1.weaponListPosition = index;
            index++;
        }

        // Assign the temporary list to the player weapon list
        snake.weaponList = tempList;
        currentWeaponIndex = 1;
        SetWeaponByIndex(currentWeaponIndex);
    }
    

    private void SetSnakeBodyTarget()
    {
        if (snakeBodyList.Count > 0)
        {
            snakeBodyList[0].GetComponent<SnakeBody>().SetTargetPosition(transform.position);
        }
        for (int i = snakeBodyList.Count - 1; i > 0; i--)
        {
            snakeBodyList[i].GetComponent<SnakeBody>().SetTargetPosition(snakeBodyList[i - 1].position);
        }

        snake.SetChildList(snakeBodyList);
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }
    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }
#endif
    #endregion
}
