using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
/// <summary>
/// This scripts controls all the input of the player
/// </summary>
[RequireComponent(typeof(Snake))]
public class SnakeControler : MonoBehaviour
{
    [SerializeField] MovementDetailsSO movementDetails;

    private float moveSpeed;
    private MovementByVelocityEvent movementByVelocityEvent;
    private Vector2 savedDirection;
    private Vector2 moveDirection;

    private void Awake()
    {
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        moveSpeed = movementDetails.GetMoveSpeed();
        savedDirection = new Vector2(1f, 0f);
    }

    /// <summary>
    /// Moves the snake to the <see cref="moveDirection"/> position using the <seealso cref="MovementByVelocityEvent"/>.
    /// </summary>
    /// <param name="movement"></param>
    public void OnMove(CallbackContext movement)
    {
        if (movement.performed)
        {
            moveDirection = movement.ReadValue<Vector2>();

            // Check if the move direction is allowed
            if (IsMovementAllowed(moveDirection))
            {
                savedDirection = moveDirection;
            }

            movementByVelocityEvent.MovementByVelocity(savedDirection, moveSpeed);
        }
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
}
