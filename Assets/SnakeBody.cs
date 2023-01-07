using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class SnakeBody : MonoBehaviour
{
    private Vector3 nextbodyPos;
    private int waitPreviousParts;
    private Snake snake;

    private void Awake()
    {
        snake = GameManager.Instance.GetSnake();
    }

    private void Start()
    {
        nextbodyPos = transform.position;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextbodyPos, snake.snakeControler.GetMoveSpeed() * Time.deltaTime); ;
    }

    public void WaitHeadUpdateCicle(int value)
    {
        waitPreviousParts = value;
    }

    public void SetTargetPosition(Vector3 position)
    {
        if (waitPreviousParts > 0)
        {
            waitPreviousParts--;
            return;
        }

        nextbodyPos = position;
    }
}
