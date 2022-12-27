using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeGame : SingletonMonoBehaviour<SnakeGame>
{
    // The grid dimensions
    public int GridWidth { get; set; }
    public int GridHeight {get; set; }

    // The size of each grid cell
    public float cellSize = 1.0f;

    // The prefab for the snake body segments
    public GameObject snakeBodyPrefab;

    // The prefab for the food
    public GameObject foodPrefab;
    
    // The prefab for the walls
    public GameObject wallPrefab;

    // The snake's body segments
    List<GameObject> snakeBody = new List<GameObject>();
    Food food;

    // The current direction of the snake
    Vector2 direction = Vector2.right; // 0 = right, 1 = down, 2 = left, 3 = up

    // The position of the food
    int foodX = 0;
    int foodY = 0;

    // The snake's speed
    public float speed = 0.5f;

    // A timer to control the game update frequency
    float timer = 0.0f;

    protected override void Awake()
    {
        base.Awake();
        GridWidth = 50;
        GridHeight = 50;
        food = foodPrefab.GetComponent<Food>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set up the initial position of the snake
        int startX = GridWidth / 3;
        int startY = GridHeight / 3;
        snakeBody.Add(CreateSnakeBody(startX, startY));
        snakeBody.Add(CreateSnakeBody(startX, startY - 1));
        snakeBody.Add(CreateSnakeBody(startX, startY - 2));


        // Set up the walls
        for (int x = 0; x < GridWidth; x++)
        {
            Instantiate(wallPrefab, new Vector2(x, -1), Quaternion.identity);
            Instantiate(wallPrefab, new Vector2(x, GridHeight), Quaternion.identity);
        }
        for (int y = 0; y < GridHeight; y++)
        {
            Instantiate(wallPrefab, new Vector2(-1, y), Quaternion.identity);
            Instantiate(wallPrefab, new Vector2(GridWidth, y), Quaternion.identity);
        }

        // Generate the initial food position
        food.GenerateFood();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food"))
        {
            EatFood();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Increment the timer
        timer += Time.deltaTime;

        if (timer >= speed)
        {
            timer = 0f;

            MovementInput();

            UpdateSnake();
        }
    }

    private void MovementInput()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        Vector2 movementVector = new Vector2(horizontalMovement, verticalMovement);

        //direction = movementVector * Time.deltaTime;

        // Check for input
        if (movementVector == Vector2.left)
        {
            direction = Vector2.left;
        }
        else if (movementVector == Vector2.right)
        {
            direction = Vector2.right;
        }
        else if (movementVector == Vector2.up)
        {
            direction = Vector2.up;
        }
        else if (movementVector == Vector2.down)
        {
            direction = Vector2.down;
        }
    }

    private void UpdateSnake()
    {
        // Update the snake's position
        Vector2 newPos = snakeBody[0].transform.position + (Vector3)direction;
        int newX = (int)newPos.x;
        int newY = (int)newPos.y;

        // Check for collision with walls
        if (newX < 0 || newX >= GridWidth || newY < 0 || newY >= GridHeight)
        {
            Debug.Log("Game Over!");
            enabled = false;
        }

        // Update the snake
        for (int i = snakeBody.Count - 1; i > 0; i--)
        {
            snakeBody[i].transform.position = snakeBody[i - 1].transform.position;
        }
        snakeBody[0].transform.position = newPos;

        EatFood(newX, newY);
    }

    private void EatFood(int newX, int newY)
    {
        // Check for food
        if (newX == foodX && newY == foodY)
        {
            // Eat the food
            GameObject newSegment = CreateSnakeBody(snakeBody[snakeBody.Count - 1].transform.position.x, snakeBody[snakeBody.Count - 1].transform.position.y);
            snakeBody.Add(newSegment);
            food.GenerateFood();
        }
    }

    private void EatFood()
    {
        // Eat the food
        GameObject newSegment = CreateSnakeBody(snakeBody[snakeBody.Count - 1].transform.position.x, snakeBody[snakeBody.Count - 1].transform.position.y);
        snakeBody.Add(newSegment);
        food.GenerateFood();
    }

    //private void GenerateFood()
    //{
    //    foodX = Random.Range(0, GridWidth);
    //    foodY = Random.Range(0, GridHeight);
    //    Instantiate(foodPrefab, new Vector2(foodX, foodY), Quaternion.identity);
    //}

    // Creates a new snake body segment at the given position
    private GameObject CreateSnakeBody(float x, float y)
    {
        return Instantiate(snakeBodyPrefab, new Vector2(x, y), Quaternion.identity);
    }
}
