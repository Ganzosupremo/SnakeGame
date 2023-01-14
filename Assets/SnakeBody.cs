using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class SnakeBody : MonoBehaviour
{
    private MovementByVelocityEvent movementByVelocityEvent;
    private float moveSpeed;
    private Snake snake;

    private void Awake()
    {
        snake = GameManager.Instance.GetSnake();
        movementByVelocityEvent = snake.movementByVelocityEvent;
    }

    private void Start()
    {

    }

    private void FixedUpdate()
    {
        //for (int i = snake.snakeControler.GetSnakeSegmentList().Count - 1; i > 0; i--)
        //{
        //    snake.snakeControler.GetSnakeSegmentList()[i].position = snake.snakeControler.GetSnakeSegmentList()[i - 1].position;
        //    //snake.snakeControler.GetSnakeSegmentList()[i].position *= offset;
        //    //movementByVelocityEvent.MovementByVelocity(snake.snakeControler.GetSnakeSegmentList()[i].position, moveSpeed);
        //}
    }
}
