using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using SnakeGame.UI;
using SnakeGame.Minimap;
using SnakeGame.Enemies;
using SnakeGame.PlayerSystem;

namespace SnakeGame
{
    [DisallowMultipleComponent]
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        [HideInInspector] public GameState currentGameState;
        [HideInInspector] public GameState previousGameState;

        /// <summary>
        /// Gets the current level index, which is used by the game manager
        /// to load the next level
        /// </summary>
        public int LevelIndex { get { return currentDungeonLevelListIndex; } }
        
        /// <summary>
        /// Gets the total count of game levels
        /// </summary>
        public int LevelCount { get { return gameLevelList.Count; } }
        public bool IsFading { get; private set; } = false;

        #region Header REFERENCES
        [Space(10)]
        [Header("OBJECT REFERENCES")]
        #endregion
        #region Tooltip
        [Tooltip("Populate with all the game level for this game")]
        #endregion
        [SerializeField] private List<GameLevelSO> gameLevelList;
        #region Tooltip
        [Tooltip("Populate with the starting dungeon level for testing, first dungeon level = 0")]
        #endregion
        [SerializeField] private int currentDungeonLevelListIndex = 0;
        #region Tooltip
        [Tooltip("Populate with the text component that is in the LoadScreenUI")]
        #endregion
        [SerializeField] private TextMeshProUGUI loadScreenText;
        #region Tooltip
        [Tooltip("Populate with the canvas group component in the FadeScreen UI")]
        #endregion
        [SerializeField] private CanvasGroup canvasGroup;
        #region Tooltip
        [Tooltip("Populate with the pause menu prefab.")]
        #endregion
        [SerializeField] private GameObject pauseMenuUI;
        #region Tooltip
        [Tooltip("This text will give feedback about different things to the player.")]
        #endregion
        [SerializeField] private TextMeshProUGUI feedbackText;

        private Room currentRoom;
        private Room previousRoom;

        private InstantiatedRoom bossRoom;
        private SnakeDetailsSO snakeDetails;
        private Snake snake;
        private long gameScore;
        private int scoreMultiplier;
        protected override void Awake()
        {
            base.Awake();
            //DontDestroyOnLoad(this.gameObject);

            snakeDetails = GameResources.Instance.currentSnake.snakeDetails;

            InstantiatePlayer();
        }

        /// <summary>
        /// Instantiates the snake in the scene
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
            gameScore = 0;
            scoreMultiplier = 0;

            PauseMenuUI.Instance.ChangeDayCicle();
            StartCoroutine(FadeScreen(0f, 1f, 0f, Color.black));
        }

        private void Update()
        {
            HandleGameStates();

            if (Input.GetKeyDown(KeyCode.P))
            {
                currentGameState = GameState.Restarted;
            }

            //HandleGameDifficulty();
        }

        private void OnEnable()
        {
            StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
            StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
            StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointsScored;
            StaticEventHandler.OnMultiplier += StaticEventHandler_OnMultiplier;
            snake.destroyEvent.OnDestroy += Snake_OnDestroy;
        }

        private void OnDisable()
        {
            StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
            StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
            StaticEventHandler.OnPointsScored -= StaticEventHandler_OnPointsScored;
            StaticEventHandler.OnMultiplier -= StaticEventHandler_OnMultiplier;
            snake.destroyEvent.OnDestroy -= Snake_OnDestroy;
        }

        private void Snake_OnDestroy(DestroyEvent destroyEvent, DestroyedEventArgs destroyedEventArgs)
        {
            previousGameState = currentGameState;
            currentGameState = GameState.GameLost;
        }

        private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
        {
            EnemiesDefeated();
        }

        private void StaticEventHandler_OnMultiplier(MultiplierArgs multiplierArgs)
        {
            if (multiplierArgs.multiplier)
                scoreMultiplier++;
            else
                scoreMultiplier--;

            scoreMultiplier = Mathf.Clamp(scoreMultiplier, 1, 69);
            StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
        }

        private void StaticEventHandler_OnPointsScored(PointsScoredArgs pointsScoredArgs)
        {
            gameScore += pointsScoredArgs.score * scoreMultiplier;
            StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
        }

        private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
        {
            SetCurrentRoom(roomChangedEventArgs.room);
        }

        /// <summary>
        /// Handles the state of the current playtrought using the enum <see cref="GameState"/>. 
        /// </summary>
        private void HandleGameStates()
        {
            switch (currentGameState)
            {
                case GameState.Started:

                    PlayGameLevel(currentDungeonLevelListIndex);
                    currentGameState = GameState.Playing;
                    EnemiesDefeated();

                    break;
                case GameState.Playing:
                    
                    if (GetSnake().GetSnakeControler().GetInputActions().Snake.Pause.WasPressedThisFrame())
                        PauseGameMenu();

                    if (GetSnake().GetSnakeControler().GetInputActions().Snake.DisplayMap.IsPressed())
                        DisplayOverviewMap();

                    break;
                case GameState.EngagingEnemies:
                    
                    if (GetSnake().GetSnakeControler().GetInputActions().Snake.Pause.WasPressedThisFrame())
                        PauseGameMenu();

                    break;
                case GameState.BossStage:
                    
                    if (GetSnake().GetSnakeControler().GetInputActions().Snake.Pause.WasPressedThisFrame())
                        PauseGameMenu();

                    if (GetSnake().GetSnakeControler().GetInputActions().Snake.DisplayMap.IsPressed())
                        DisplayOverviewMap();

                    break;
                case GameState.EngagingBoss:
                    
                    if (GetSnake().GetSnakeControler().GetInputActions().Snake.Pause.WasPressedThisFrame())
                        PauseGameMenu();

                    break;
                case GameState.LevelCompleted:

                    StartCoroutine(LevelCompleted());

                    break;
                case GameState.GameWon:
                    // Just call this once
                    if (previousGameState != GameState.GameWon)
                        StartCoroutine(GameWon());

                    break;
                case GameState.GameLost:
                    // Just call this once
                    if (previousGameState != GameState.GameLost)
                    {
                        StopAllCoroutines();
                        StartCoroutine(GameLost());
                    }

                    break;
                case GameState.Paused:

                    if (GetSnake().GetSnakeControler().GetInputActions().Snake.Pause.WasPressedThisFrame())
                        PauseGameMenu();

                    break;

                case GameState.OverviewMap:

                    if (GetSnake().GetSnakeControler().GetInputActions().Snake.DisplayMap.WasReleasedThisFrame())
                        DungeonMap.Instance.ClearDungeonOverviewMap();

                    break;

                case GameState.Restarted:

                    RestartGame();

                    break;
                default:
                    break;
            }
        }

        public void PlayNextLevel(int indexLevel)
        {
            GetSnake().idleEvent.CallIdleEvent();
            GetSnake().GetSnakeControler().DisableSnake();

            PlayGameLevel(indexLevel);
        }

        private void PlayGameLevel(int currentDungeonLevelListIndex)
        {
            bool dungeonBuiltSuccesfully = DungeonBuilder.Instance.GenerateDungeon(gameLevelList[currentDungeonLevelListIndex]);

            if (!dungeonBuiltSuccesfully)
                Debug.LogError("Couldn't build dungeon from the specified node graphs");

            // Trigger the room changed event for the first time
            StaticEventHandler.CallRoomChangedEvent(currentRoom);

            // Set the snake position roughly in the middle of the room
            snake.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f,
                (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);

            // Get the nearest spawn point position of the room, so the snake doesn't spawn in walls or something
            snake.gameObject.transform.position = HelperUtilities.GetNearestSpawnPointPosition(snake.gameObject.transform.position);

            //snake.gameObject.transform.position = new Vector3(NoiseMap.Instance.mapPresets[0].width / 2, NoiseMap.Instance.mapPresets[0].height / 2, 0f);
            //snake.gameObject.transform.position = HelperUtilities.GetSpawnPosition(snake.gameObject.transform.position);

            GetSnake().GetSnakeControler().EnableSnake();

            StartCoroutine(ShowLevelName());
        }

        private IEnumerator ShowLevelName()
        {
            // Set the screen to black
            StartCoroutine(FadeScreen(0f, 1f, 0f, Color.black));

            GetSnake().GetSnakeControler().DisableSnake();

            string levelName = "LOADING LEVEL... \n\n" + gameLevelList[currentDungeonLevelListIndex].levelName.ToUpper();
            //+ (currentDungeonLevelListIndex + 1).ToString()
            //+ "\n\n" + gameLevelList[currentDungeonLevelListIndex].levelName.ToUpper();

            yield return StartCoroutine(DisplayMessageRoutine(levelName, Color.white, 4.2f));

            GetSnake().GetSnakeControler().EnableSnake();

            // Fade out the screen again
            StartCoroutine(FadeScreen(1f, 0f, 2f, Color.black));
        }

        /// <summary>
        /// Displays a message on the screen, if the timeSeconds equals zero
        /// then we wait until the enter is pressed.
        /// </summary>
        /// <param name="levelName">The name of the current level</param>
        /// <param name="textColor">The color of the text</param>
        /// <param name="timeSeconds">The time that the message will appear</param>
        /// <returns>Whatever a Coroutine returns</returns>
        private IEnumerator DisplayMessageRoutine(string levelName, Color textColor, float timeSeconds)
        {
            loadScreenText.SetText(levelName);
            loadScreenText.color = textColor;

            //Display the text for a given period of time
            if (timeSeconds > 0f)
            {
                float timer = timeSeconds;

                while (timer > 0f && !Input.GetKeyDown(KeyCode.Return))
                {
                    timer -= Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                while (!Input.GetKeyDown(KeyCode.Return))
                {
                    yield return null;
                }
            }

            yield return null;

            //Clear the text
            loadScreenText.SetText("");
        }

        /// <summary>
        /// Loads the next game level if all rooms have no more enemies.
        /// </summary>
        private void EnemiesDefeated()
        {
            bool isDungeonClearOfNormalEnemies = true;
            bossRoom = null;

            // See if all rooms have been cleared of enemies
            foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
            {
                // Skip the boos room for the moment
                if (keyValuePair.Value.roomNodeType.isBossRoom)
                {
                    bossRoom = keyValuePair.Value.instantiatedRoom;
                    continue;
                }

                // Check if the other rooms have been cleared of enemies
                if (!keyValuePair.Value.isClearOfEnemies)
                {
                    isDungeonClearOfNormalEnemies = false;
                    break;
                }
            }

            // Set the game state
            // If the dungeon level has been cleared completely - all the normal rooms have been cleared except the boss room,
            // Or if there's no boss room
            // Or all rooms and the boss room have been cleared
            if ((isDungeonClearOfNormalEnemies && bossRoom == null) || (isDungeonClearOfNormalEnemies && bossRoom.room.isClearOfEnemies))
            {
                // If there are more dungeon level, then 
                if (currentDungeonLevelListIndex < gameLevelList.Count - 1)
                    currentGameState = GameState.LevelCompleted;
                else
                    currentGameState = GameState.GameWon;
            }
            //If just the boss room is not cleared yet
            else if (isDungeonClearOfNormalEnemies)
            {
                currentGameState = GameState.BossStage;

                StartCoroutine(BossStage());
            }
        }

        /// <summary>
        /// Fades the screen to black using the canvas group and manipulating it's alpha value
        /// </summary>
        /// <param name="currentAlpha"></param>
        /// <param name="targetAlpha"></param>
        /// <param name="timeSeconds"></param>
        /// <param name="screenColor"></param>
        /// <returns></returns>
        public IEnumerator FadeScreen(float currentAlpha, float targetAlpha, float timeSeconds, Color screenColor)
        {
            IsFading = true;

            Image image = canvasGroup.GetComponent<Image>();
            image.color = screenColor;
            float timer = 0f;

            while (timer <= timeSeconds)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(currentAlpha, targetAlpha, timer / timeSeconds);
                yield return null;
            }

            IsFading = false;
        }

        public IEnumerator ShowMessage(string messageToDisplay, float time)
        {
            if (IsFading) yield break;

            feedbackText.text = messageToDisplay;
            yield return new WaitForSeconds(time);
            feedbackText.text = "";
        }

        #region Different Game States
        private IEnumerator BossStage()
        {
            bossRoom.gameObject.SetActive(true);
            bossRoom.UnlockDoors(0f);

            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(FadeScreen(0f, 1f, 2f, new Color(0f, 0f, 0.4f)));

            yield return StartCoroutine(DisplayMessageRoutine("Well Done " + GameResources.Instance.currentSnake.snakeName + "!" +
                " You Defeated all Enemies. Except... The Boss! \n\n Find Him and Finish The Job.", Color.white, 4.5f));

            yield return StartCoroutine(FadeScreen(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));
        }

        private IEnumerator LevelCompleted()
        {
            currentGameState = GameState.Playing;

            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(FadeScreen(0f, 1f, 2f, new(0f, 0f, 0f, 0.4f)));

            currentDungeonLevelListIndex++;
            //Debug.Log(currentDungeonLevelListIndex);
            //Debug.Log("Level Count: " + LevelCount);
            //Debug.Log("Level Index: " + LevelIndex);
            string name = GameResources.Instance.currentSnake.snakeName;
            if (name == "") name = snakeDetails.snakeName.ToUpper();

            //Display the level complete message
            yield return StartCoroutine(DisplayMessageRoutine("Well Done " + name + "!" +
                "\n You defeated the boss on this biome, but there are more out there", Color.white, 4f));

            yield return StartCoroutine(DisplayMessageRoutine("Press 'Enter'" +
                "\n or head to the exit room, to continue your Journey.", Color.white, 4f));

            //Fade out the canvas to clear the display
            yield return StartCoroutine(FadeScreen(1f, 0f, 2f, new(0f, 0f, 0f, 0.4f)));

            // Change later to the new input system
            while (!Input.GetKeyDown(KeyCode.Return))
                yield return null;

            yield return null;

            PlayGameLevel(currentDungeonLevelListIndex);
        }

        private IEnumerator GameWon()
        {
            previousGameState = GameState.GameWon;

            //Fade in the canvas to display a message
            yield return StartCoroutine(FadeScreen(0f, 1f, 2f, Color.black));

            GetSnake().GetSnakeControler().DisableSnake();

            //Display the game complete message
            yield return StartCoroutine(DisplayMessageRoutine("Good Job Soldier " + GameResources.Instance.currentSnake.snakeName + "!"
                + "\n\nYou've Fulfilled Your Journey!", Color.white, 5.5f));

            yield return StartCoroutine(DisplayMessageRoutine("Your Final Score Was " + gameScore.ToString("###,###,###0"), Color.white, 6f));

            yield return StartCoroutine(DisplayMessageRoutine("Congratulations For Completing The Game,\n\nAnd Thanks For Playing", Color.white, 6f));

            yield return StartCoroutine(DisplayMessageRoutine("Press 'Enter' To Restart The Game Or Enjoy The Music :)", Color.white, 0f));

            currentGameState = GameState.Restarted;
        }

        private IEnumerator GameLost()
        {
            previousGameState = GameState.GameLost;
            GetSnake().GetSnakeControler().DisableSnake();
            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(FadeScreen(0f, 1f, 2f, Color.black));

            // Disable the enemies that are present(FindObjectOfType requires a lot of resourcess - it's ok in this stage of the game)
            Enemy[] enemiesArray = FindObjectsOfType<Enemy>();
            foreach (Enemy enemy in enemiesArray)
            {
                enemy.gameObject.SetActive(false);
            }

            string name = GameResources.Instance.currentSnake.snakeName;
            if (name == "") name = snakeDetails.snakeName.ToUpper();

            yield return StartCoroutine(DisplayMessageRoutine("You Died " + name +
        "! \n\nYou Failed (Miserably), But Are YOU Gonna Give Up?", Color.white, 3.5f));

            yield return StartCoroutine(DisplayMessageRoutine("Your Final Score Was " + gameScore.ToString("###,###,###0"), Color.white, 4f));

            yield return StartCoroutine(DisplayMessageRoutine("Press 'Enter' to try again", Color.white, 0f));

            currentGameState = GameState.Restarted;
        }

        public void PauseGameMenu()
        {
            if (currentGameState != GameState.Paused)
            {
                pauseMenuUI.SetActive(true);
                GetSnake().GetSnakeControler().DisableSnake();

                // Set the game states
                previousGameState = currentGameState;
                currentGameState = GameState.Paused;
            }
            else if (currentGameState == GameState.Paused)
            {
                pauseMenuUI.SetActive(false);
                GetSnake().GetSnakeControler().EnableSnake();

                // Restore the game states
                currentGameState = previousGameState;
                previousGameState = GameState.Paused;
            }
        }

        private void DisplayOverviewMap()
        {
            if (IsFading) return;

            DungeonMap.Instance.DisplayDungeonOverviewMap();
        }

        private void RestartGame()
        {
            SceneManager.LoadScene((int)SceneIndex.MainMenu);
        }
        #endregion

        #region Setters
        /// <summary>
        /// Sets the current room the player is in
        /// </summary>
        /// <param name="room">The room to set as the current room</param>
        public void SetCurrentRoom(Room room)
        {
            previousRoom = currentRoom;
            currentRoom = room;
        }
        #endregion

        #region Getters
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
        #endregion

        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(gameLevelList), gameLevelList);
            HelperUtilities.ValidateCheckNullValue(this, nameof(canvasGroup), canvasGroup);
            HelperUtilities.ValidateCheckNullValue(this, nameof(loadScreenText), loadScreenText);
        }
#endif
        #endregion
    }
}