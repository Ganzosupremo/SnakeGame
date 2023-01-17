using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [HideInInspector] public GameState currentGameState;
    [HideInInspector] public GameState previousGameState;

    #region Header Dungeon Levels
    [Space(10)]
    [Header("The Levels Of The Dungeon")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with all the game level for this game")]
    #endregion
    [SerializeField] private List<GameLevelSO> gameLevelList;

    #region Tooltip
    [Tooltip("Populate with the starting dungeon level for testing, first dungeon level = 0")]
    #endregion
    [SerializeField] private int currentDungeonLevelListIndex = 0;

    [SerializeField] private float foodSpawnTimer = 10f;

    private Room currentRoom;
    private Room previousRoom;
    private SnakeDetailsSO snakeDetails;
    private Snake snake;

    private float timer;
    //private int foodSpawnedSoFar = 0;

    protected override void Awake()
    {
        base.Awake();

        snakeDetails = GameResources.Instance.currentPlayer.snakeDetails;

        timer = foodSpawnTimer;

        //testList = snake.snakeControler.GetSnakeSegmentList();

        InstantiatePlayer();
    }

    /// <summary>
    /// Instantiates the snake in the scene at position
    /// </summary>
    private void InstantiatePlayer()
    {
        GameObject snakeGameObject = Instantiate(snakeDetails.snakePrefab);

        snake = snakeGameObject.GetComponent<Snake>();

        snake.Initialize(snakeDetails);
    }

    private void Start()
    {
        previousGameState = GameState.Started;
        currentGameState = GameState.Started;
    }

    private void Update()
    {
        HandleGameStates();

        if (Input.GetKeyDown(KeyCode.P))
        {
            currentGameState = GameState.Started;
        }

        //foodSpawnTimer -= Time.deltaTime;
        //if (foodSpawnTimer <= 0f)
        //{
        //    SpawnFood();
        //}
    }

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);
        //SpawnFood();
    }

    /// <summary>
    /// Handles the state of the current playtrought using the enum <see cref="GameState"/>. 
    /// </summary>
    private void HandleGameStates()
    {
        switch (currentGameState)
        {
            case GameState.Started:
                PlayDungeonLevel(currentDungeonLevelListIndex);
                currentGameState = GameState.Playing;
                break;
            case GameState.Playing:
                break;
            case GameState.EngagingEnemies:
                break;
            case GameState.BossStage:
                break;
            case GameState.EngagingBoss:
                break;
            case GameState.LevelCompleted:
                break;
            case GameState.GameWon:
                break;
            case GameState.GameLost:
                break;
            case GameState.Paused:
                break;
            case GameState.Restarted:
                break;
            default:
                break;
        }
    }

    private void PlayDungeonLevel(int currentDungeonLevelListIndex)
    {
        bool dungeonBuiltSuccesfully = DungeonBuilder.Instance.GenerateDungeon(gameLevelList[currentDungeonLevelListIndex]);

        if (!dungeonBuiltSuccesfully)
        {
            Debug.LogError("Couldn't build dungeon from the specified node graphs");
        }

        // Trigger the room changed event for the first time
        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        //InstantiatePlayer();

        // Set the snake position roughly in the middle of the room
        snake.gameObject.transform.position = new Vector3((currentRoom.worldLowerBounds.x + currentRoom.worldUpperBounds.x) / 2f,
            (currentRoom.worldLowerBounds.y + currentRoom.worldUpperBounds.y) / 2f, 0f);

        // Get the nearest spawn point position of the room, so the snake doesn't spawn in walls or something
        snake.gameObject.transform.position = HelperUtilities.GetNearestSpawnPointPosition(snake.gameObject.transform.position);
    }

    /// <summary>
    /// Spawn the snake food on the dungeon
    /// </summary>
    public void SpawnFood()
    {
        // Know in which room the food should spawn
        Vector3 spawnPosition = new(Random.Range(currentRoom.tilemapLowerBounds.x, currentRoom.tilemapUpperBounds.x),
            Random.Range(currentRoom.tilemapLowerBounds.y, currentRoom.tilemapUpperBounds.y), 0f);

        // Make sure the food spawns within the room
        Food food = (Food)PoolManager.Instance.ReuseComponent(GameResources.Instance.foodPrefab, HelperUtilities.GetNearestSpawnPointPosition(spawnPosition),
            Quaternion.identity);
        food.gameObject.SetActive(true);
        foodSpawnTimer = timer;
    }

    /// <summary>
    /// Sets the current room the player is in
    /// </summary>
    /// <param name="room">The room to set as the current room</param>
    public void SetCurrentRoom(Room room)
    {
        previousRoom = currentRoom;
        currentRoom = room;
    }

    /// <summary>
    /// Gets the current room 
    /// </summary>
    /// <returns>The room the player currently is in</returns>
    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    /// <summary>
    /// Gets the current game level
    /// </summary>
    /// <returns>Returns the current GameLevelSO</returns>
    public GameLevelSO GetCurrentDungeonLevel()
    {
        return gameLevelList[currentDungeonLevelListIndex];
    }

    public Snake GetSnake()
    {
        return snake;
    }

    public Sprite GetMinimapIcon()
    {
        return snakeDetails.snakeMinimapIcon;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(gameLevelList), gameLevelList);
    }
#endif
    #endregion
}
