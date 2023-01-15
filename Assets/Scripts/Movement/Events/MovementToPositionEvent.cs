using System;
using UnityEngine;

/// <summary>
/// This event is used when the player dashes
/// </summary>
[DisallowMultipleComponent]
public class MovementToPositionEvent : MonoBehaviour
{
    public event Action<MovementToPositionEvent, MovementToPositionEventArgs> OnMovementToPosition;

    public void CallMovementToPosition(Vector3 moveToPosition, Vector3 currentPosition, Vector3 moveDirection, float velocity, bool isDashing = false)
    {
        OnMovementToPosition?.Invoke(this, new MovementToPositionEventArgs()
        {
            movePosition = moveToPosition,
            currentPosition = currentPosition,
            moveDirection = moveDirection,
            velocity = velocity,
            isDashing = isDashing
        });
    }
}

public class MovementToPositionEventArgs : EventArgs
{
    public Vector3 movePosition;
    public Vector3 currentPosition;
    public Vector3 moveDirection;
    public float velocity;
    public bool isDashing;
}
