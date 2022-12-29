using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovementByVelocityEvent : MonoBehaviour
{
    public event Action<MovementByVelocityEvent, MovementByVelocityEventArgs> OnMovementByVelocity;

    public void MovementByVelocity(Vector2 moveDirection, float velocity)
    {
        OnMovementByVelocity?.Invoke(this, new MovementByVelocityEventArgs()
        {
            moveDirection = moveDirection,
            velocity = velocity
        });
    }
}

public class MovementByVelocityEventArgs : EventArgs
{
    public Vector2 moveDirection;
    public float velocity;
}
