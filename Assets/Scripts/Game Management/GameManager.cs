using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Room currentRoom;
    private Room previousRoom;
    private SnakeDetailsSO snakeDetails;
    private Snake snake;

    protected override void Awake()
    {
        base.Awake();

        snakeDetails = GameResources.Instance.currentPlayer.snakeDetails;

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
        currentGameState = GameState.Started;
    }

    private void Update()
    {
        HandleGameStates();

        if (Input.GetKeyDown(KeyCode.P))
        {
            currentGameState = GameState.Started;
        }
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
        snake.SpawnFood();
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

        // Set the snake position roughly in the middle of the room
        snake.gameObject.transform.position = new Vector3((currentRoom.worldLowerBounds.x + currentRoom.worldUpperBounds.x) / 2f,
            (currentRoom.worldLowerBounds.y + currentRoom.worldUpperBounds.y) / 2f, 0f);

        // Get the nearest spawn point position of the room, so the snake doesn't spawn in walls or something
        snake.gameObject.transform.position = HelperUtilities.GetNearestSpawnPointPosition(snake.gameObject.transform.position);
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

    public Snake GetSnake()
    {
        return snake;
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
