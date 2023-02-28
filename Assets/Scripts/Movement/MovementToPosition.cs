using System;
using UnityEngine;

/// <summary>
/// This is one subscriber of <seealso cref="MovementToPositionEvent"/>.
/// </summary>
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(Rigidbody2D))]
public class MovementToPosition : MonoBehaviour
{
    private Rigidbody2D m_Rb;
    private MovementToPositionEvent m_MovementToPositionEvent;

    private void Awake()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_MovementToPositionEvent = GetComponent<MovementToPositionEvent>();
    }

    private void OnEnable()
    {
        m_MovementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
    }

    private void OnDisable()
    {
        m_MovementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
    }

    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionEventArgs movementToPositionEventArgs)
    {
        MoveRigidBody(movementToPositionEventArgs.movePosition, movementToPositionEventArgs.currentPosition, movementToPositionEventArgs.velocity);
    }

    private void MoveRigidBody(Vector3 movePosition, Vector3 currentPosition, float velocity)
    {
        Vector2 unitVector = Vector3.Normalize(movePosition - currentPosition);

        m_Rb.MovePosition(m_Rb.position + (Time.fixedDeltaTime * velocity * unitVector));
    }
}
