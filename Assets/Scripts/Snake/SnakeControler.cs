using Cysharp.Threading.Tasks;
using SnakeGame.AbwehrSystem;
using SnakeGame.GameUtilities;
using SnakeGame.ProceduralGenerationSystem;
using SnakeGame.UI;
using SnakeInput;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace SnakeGame.PlayerSystem
{
    /// <summary>
    /// This script controls all the input of the player.
    /// </summary>
    [RequireComponent(typeof(Snake))]
    [DisallowMultipleComponent]
    public class SnakeControler : MonoBehaviour
    {
        public bool IsSnakeEnabled { get; set; } = false;
        
        [SerializeField] private MovementDetailsSO movementDetails;
        [SerializeField] private Transform weaponShootPosition;

        private Snake snake;
        private SnakeControl snakeInputActions;

        private bool fireButtonPressedPreviousFrame = false;
        private int currentWeaponIndex = 1;
        private float moveSpeed;
        private Vector2 savedDirection;
        private Vector2 moveDirection;

        private Coroutine snakeDashCoroutine;
        private WaitForFixedUpdate waitForFixedUpdate;
        private bool isSnakeDashing = false;
        private float dashCooldownTimer = 0f;

        [Obsolete]
        private float specialAbilityCooldownTimer = 0f;
        [Obsolete]
        private Coroutine specialAbilityCoroutine;

        private bool isSpecialAbilityActive = false;
        private CancellationTokenSource _CancellationTokenSource = new();

        private void Awake()
        {
            snakeInputActions = new();
            snakeInputActions.Snake.Enable();

            snake = GetComponent<Snake>();
        }

        private void Start()
        {
            waitForFixedUpdate = new();
            savedDirection = new Vector2(0f, 0f);

            SetInitialWeapon();
            SetAnimatorSpeed();
        }

        private void OnDisable()
        {
            _CancellationTokenSource.Cancel();
        }

        private void OnDestroy()
        {
            _CancellationTokenSource.Dispose();
        }

        private void FixedUpdate()
        {
            if (!IsSnakeEnabled) return;

            OnMove();
        }

        private void Update()
        {
            if (!IsSnakeEnabled) return;

            WeaponInputs();

            SnakeDashCooldownTimer();

            //if (snakeInputActions.Snake.test.IsPressed())
            //{
            //    snake.health.IncreaseHealth(3);
            //}
        }

        private async void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag(Settings.CollisionTilemapTag) ||
                other.collider.CompareTag(Settings.EnemyTag))
                await TeleportSnake();
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.collider.CompareTag(Settings.CollisionTilemapTag) ||
                other.collider.CompareTag(Settings.EnemyTag))
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
        private void OnMove()
        {
            // If the snake is dashing, no other movement will be processed
            // can't dash for the moment but just in case I decide to put the dash
            // feature again
            if (isSnakeDashing) return;

            moveDirection = snakeInputActions.Snake.Move.ReadValue<Vector2>();
            // Check if there is movement
            if (moveDirection != Vector2.zero)
            {
                // Update the segments before the snake moves, because
                // the segments will stop moving if the under if-statement is true. and looks weird.
                snake.UpdateSnakeSegments();
                // Can't move in diagonal
                if (moveDirection.x != 0f && moveDirection.y != 0f) return;

                moveSpeed = movementDetails.GetMoveSpeed();

                snake.movementByVelocityEvent.MovementByVelocity(moveDirection, moveSpeed);
                RotateBody(moveDirection);
            }
            else
            {
                snake.idleEvent.CallIdleEvent();
                snake.UpdateSnakeSegments();
            }
        }

        [Obsolete("Was used to activate a special ability when the required input was pressed." +
            " Now the SnakeAbilityManager is used for that.")]
        private void OnSpecialAbility()
        {
            if (!gameObject.activeSelf) return;

            if (snakeInputActions.Snake.SpecialAbility.IsPressed() && 
                specialAbilityCooldownTimer <= 0f &&
                !isSpecialAbilityActive)
            {
                // The cost for using special abilities
                snake.health.TakeDamage(1);
                SnakeAbility(2f);
            }
        }

        [Obsolete("Also obsolete for the same reason as the above method.")]
        private void SnakeAbility(float duration)
        {
            if (specialAbilityCoroutine != null)
                StopCoroutine(specialAbilityCoroutine);

            specialAbilityCoroutine = StartCoroutine(SnakeAbilityRoutine(duration, 
                Abilities.None, null));
        }

        [Obsolete("Obsolete just the same as the above method.")]
        private IEnumerator SnakeAbilityRoutine(float duration, Abilities ability, UnityEngine.Object specialAbility)
        {
            while (duration >= 0f)
            {
                isSpecialAbilityActive = true;

                //snake.snakeSpecialAbility.SetCurrentAbility(ability, specialAbility);
                //snake.snakeSpecialAbility.ActivateAbility();

                duration -= Time.unscaledDeltaTime;
                StartCoroutine(SpecialAbilityUI.Instance.UpdateSpecialAbilityBar(duration / 5f));
                yield return waitForFixedUpdate;
            }

            isSpecialAbilityActive = false;
            //snake.snakeSpecialAbility.ResetToNormal();
            specialAbilityCooldownTimer = 5f;
            SpecialAbilityUI.Instance.ResetSpecialAbilityBar();
            SpecialAbilityUI.Instance.ResetSpecialAbilityCooldownBar();
        }

        private void WeaponInputs()
        {
            OnAim();

            OnSwitchWeapon();

            OnReload();
        }

        /// <summary>
        /// Aims the weapon at the position of the mouse,
        /// also handles the firing of the weapon.
        /// </summary>
        private void OnAim()
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

            //if (weapon.isWeaponReloading) return;

            // If the remaining ammo is less than the weapon mag max capacity and we dont have infinity ammo, then just return
            if (weapon.weaponTotalAmmoCapacity < weapon.weaponDetails.clipMaxCapacity && !weapon.weaponDetails.hasInfiniteAmmo)
                return;

            // If the remaining ammo in the mag is equal to the mag max capacity, then return
            if (weapon.weaponClipRemaining == weapon.weaponDetails.clipMaxCapacity)
                return;

            if (weapon.isWeaponReloading) return;

            if (snakeInputActions.Snake.Reload.WasPressedThisFrame())
                // Trigger the reload weapon event
                snake.reloadWeaponEvent.CallReloadEvent(snake.activeWeapon.GetCurrentWeapon(), 0);
        }

        /// <summary>
        /// Makes the snake dash, while dashing the snake is invincible
        /// and moves faster.
        /// Current Situation: The snake segments don't update while the snake is dashing.
        /// And it's kinda buggy, so maybe the dash mechanic will go away.
        /// </summary>
        private void OnDash(CallbackContext context)
        {
            if (/*snakeControl.Snake.Dash.WasPressedThisFrame() &&*/ dashCooldownTimer <= 0f && context.performed && moveDirection != Vector2.zero)
                SnakeDash((Vector3)savedDirection);
        }

        /// <summary>
        /// Switches The Active Weapon With The Mouse Wheel
        /// </summary>
        private void OnSwitchWeapon()
        {
            if (isSpecialAbilityActive) return;

            Vector2 switchWeaponInput = snakeInputActions.Snake.SwitchWeapons.ReadValue<Vector2>();

            if (switchWeaponInput.y < 0f || switchWeaponInput.x < -0.5f)
                SelectPreviousWeapon();
            if (switchWeaponInput.y > 0f || switchWeaponInput.x > 0.5f)
                SelectNextWeapon();

            if (snakeInputActions.Snake.SetWeaponAtFirstPos.WasPressedThisFrame())
                SetWeaponFirstInList();
        }

        /// <summary>
        /// Check if the current move direction is allowed - Deprecated
        /// </summary>
        /// <param name="moveDirection"></param>
        /// <returns>Returns true if the given move direction is allowed</returns>
        [Obsolete()]
        private bool CheckMovementDirection(Vector2 moveDirection)
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
            else if (savedDirection == Vector2.zero)
            {
                return true;
            }
            return false;
            //if (moveDirection == Vector2.up)
            //    moveDirection = Vector2.up;
            //else if (moveDirection == Vector2.down)
            //    moveDirection = Vector2.down;
            //else if (moveDirection == Vector2.left)
            //    moveDirection = Vector2.left;
            //else if (moveDirection == Vector2.right)
            //    moveDirection = Vector2.right;
        }

        /// <summary>
        /// Rotate the sprite to face the current move direction.
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
            weaponDirection = mouseWorldPosition - snake.activeWeapon.GetFirePosition();

            //Calculate direction vector of mouse cursor from the snake transform position
            Vector3 playerDirection = mouseWorldPosition - transform.position;

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
            //if (snakeDashCoroutine != null)
            //{
            //    StopCoroutine(snakeDashCoroutine);
            //}
            snakeDashCoroutine = StartCoroutine(SnakeDashRoutine(savedDirection));
        }

        private IEnumerator SnakeDashRoutine(Vector3 savedDirection)
        {
            float minDistance = 0.4f;

            isSnakeDashing = true;

            Vector3 targetPosition = snake.transform.position + savedDirection * movementDetails.dashDistance;

            // Loop until the target position is reached
            while (Vector3.Distance(snake.transform.position, targetPosition) > minDistance)
            {
                snake.movementToPositionEvent.CallMovementToPosition(targetPosition, snake.transform.position, savedDirection, movementDetails.dashSpeed, isSnakeDashing);
                snake.UpdateSnakeSegments();

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

        [Obsolete("The ability cooldown is now keep on the SnakeAbilityManager.")]
        private void SnakeSpecialAbilityCooldownTimer()
        {
            if (specialAbilityCooldownTimer >= 0f)
            {
                specialAbilityCooldownTimer -= Time.unscaledDeltaTime;
                StartCoroutine(SpecialAbilityUI.Instance.UpdateSpecialAbilityCooldownBar
                    (specialAbilityCooldownTimer));
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

        /// <summary>
        /// Teleports the snake to the center of the room again, when the snake collides
        /// with the walls.
        /// </summary>
        private async UniTask TeleportSnake(CancellationToken cancellationToken = default)
        {
            if (!gameObject.activeSelf || !gameObject.activeInHierarchy || cancellationToken.IsCancellationRequested) return;
            
            try
            {
                await GameManager.Instance.FadeScreen(0f, 1f, 0f, Color.black);

                Room currentRoom = GameManager.Instance.GetCurrentRoom();

                snake.TakeOneDamage();

                snake.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f,
                    (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);

                // Get the nearest spawn point position of the room, so the snake doesn't spawn on the walls and gets hit again
                snake.gameObject.transform.position = HelperUtilities.GetNearestSpawnPointPosition(snake.gameObject.transform.position);

                await GameManager.Instance.FadeScreen(1f, 0f, 1f, Color.black);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        private void SetWeaponByIndex(int index)
        {
            if (index - 1 < snake.weaponList.Count)
            {
                currentWeaponIndex = index;
                snake.setActiveWeaponEvent.CallSetActiveWeaponEvent(snake.weaponList[index - 1]);

                GameCursor.SetCrosshairImage(snake.activeWeapon.GetCurrentWeapon().weaponDetails.weaponCrosshair);
            }
        }

        private void FireWeaponInput(Vector3 weaponDirection, float weaponAngleDegrees, float snakeAngleDegrees, AimDirection aimDirection)
        {
            if (snakeInputActions.Snake.Fire.IsPressed())
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

        /// <summary>
        /// Sets new values for the move speed
        /// </summary>
        /// <param name="minVel"></param>
        /// <param name="maxVel"></param>
        public void SetMovementVelocity(float minVel, float maxVel)
        {
            movementDetails.minMoveSpeed = minVel;
            movementDetails.maxMoveSpeed = maxVel;
        }

        /// <summary>
        /// Sets the <seealso cref="isSpecialAbilityActive"/> bool, 
        /// if true the player won't be able to change the current weapon until
        /// the bool is set to false again.
        /// </summary>
        /// <param name="value"></param>
        public void SetSpecialAbilityBool(bool value)
        {
            isSpecialAbilityActive = value;
        }

        public void EnableSnake()
        {
            IsSnakeEnabled = true;
        }

        public void DisableSnake()
        {
            IsSnakeEnabled = false;
        }

        /// <summary>
        /// Gets the unity's new input system input actions
        /// </summary>
        /// <returns></returns>
        public SnakeControl GetInputActions()
        {
            return snakeInputActions;
        }

        /// <summary>
        /// Gets the player move speed.
        /// </summary>
        /// <returns>Returns the min move speed, since the min and max values
        ///  on the player should be the same.</returns>
        public float GetMoveSpeed()
        {
            return movementDetails.GetMoveSpeed();
        }

        public MovementDetailsSO GetMovementDetails()
        {
            return movementDetails;
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
}