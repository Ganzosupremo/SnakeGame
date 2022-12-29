using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class acts like the controller for the snake 
/// </summary>

[RequireComponent(typeof(SnakeControler))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent (typeof(MovementByVelocityEvent))]
public class Snake : MonoBehaviour
{
    private SnakeControler snakeControler;
    private MovementByVelocityEvent movementByVelocityEvent;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        snakeControler = GetComponent<SnakeControler>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // This is a test to see when the snake collides with the walls can be respawned again in the spawn positions
    // of the room templates and lose a specific amount of health

    private void OnCollisionEnter2D(Collision2D other)
    {
        StartCoroutine(DisableObject());
    }

    private IEnumerator DisableObject()
    {
        yield return null;
        spriteRenderer.color = Color.blue;
        yield return new WaitForSeconds(3f);
        spriteRenderer.color = Color.white;
    }
}
