using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementByVelocityEvent))]
public class MovementByVelocity : MonoBehaviour
{
    private Rigidbody2D m_RB;
    private MovementByVelocityEvent m_MovementByVelocityEvent;

    private void Awake()
    {
        m_RB = GetComponent<Rigidbody2D>();
        m_MovementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
    }

    private void OnEnable()
    {
        m_MovementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
    }

    private void OnDisable()
    {
        m_MovementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
    }

    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent movementByVelocityEvent, MovementByVelocityEventArgs movementByVelocityEventArgs)
    {
        MoveRigidBody(movementByVelocityEventArgs.moveDirection, movementByVelocityEventArgs.velocity);
    }

    private void MoveRigidBody(Vector2 moveDirection, float velocity)
    {
        m_RB.velocity = moveDirection * velocity;
    }
}
