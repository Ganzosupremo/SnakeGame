using SnakeGame.Debuging;
using SnakeGame.Enemies;
using SnakeGame.GameUtilities;
using SnakeGame.HighscoreSystem;
using SnakeGame.Minimap;
using SnakeGame.PlayerSystem;
using SnakeGame.ProceduralGenerationSystem;
using SnakeGame.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SnakeGame
{
    [DisallowMultipleComponent]
    public class GameManager : MonoBehaviour
    {
        public static event Action<int> OnLevelChanged;
        private static GameManager instance;
        public static GameManager Instance { get { return instance; } }

        #region Header REFERENCES
        [Header("GAME LEVELS")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("Populate with all the game level for this game")]
        #endregion
        [SerializeField] private List<GameLevelSO> gameLevelList;
        #region Tooltip
        [Tooltip("Populate with the starting dungeon level for testing, first dungeon level = 0")]
        #endregion
        [SerializeField] private int currentDungeonLevelListIndex = 0;

        #region Header REFERENCES
        [Space(10)]
        [Header("OBJECT REFERENCES")]
        #endregion
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

        /// <summary>
        /// Gets the total count of game levels
        /// </summary>
        public int LevelCount { get { return gameLevelList.Count; } }
        public bool IsFading { get; private set; } = false;
        public static GameState CurrentGameState { get { return currentGameState; } set { currentGameState = value; } }
        public static GameState PreviousGameState { get { return previousGameState; } set { previousGameState = value; } }

        private Room currentRoom;
        private Room previousRoom;

        private static GameState currentGameState;
        private static GameState previousGameState;

        private InstantiatedRoom bossRoom;
        private SnakeDetailsSO snakeDetails;
        private Snake snake;
        private long gameScore;
        private int scoreMultiplier;

        protected void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);

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
            snake.ChangeLightIntensity();
            StartCoroutine(FadeScreen(0f, 1f, 0f, Color.black));
        }

        private void Update()
        {
            HandleGameStates();

            if (Input.GetKeyDown(KeyCode.P))
            {
                currentGameState = GameState.Restarted;
            }
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
            StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
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

            // Trigger the room changed event on the first load
            StaticEventHandler.CallRoomChangedEvent(currentRoom);

            // Set the snake position roughly in the middle of the room
            snake.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f,
                (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);

            // Get the nearest spawn point position of the room, so the snake doesn't spawn in walls or something
            snake.gameObject.transform.position = HelperUtilities.GetNearestSpawnPointPosition(snake.gameObject.transform.position);

            CallOnLevelChangedEvent();
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
            this.Log($"Game State: {currentGameState}");
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

            this.Log($"Game State After Enemies Defeated: {currentGameState}");
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

            yield return StartCoroutine(FadeScreen(0f, 1f, 2f, Color.Lerp(new Color(0f, 0f, 0f, 0.45f), new Color(0f, 0f, 0f, 0.3f), 1f)));

            yield return StartCoroutine(DisplayMessageRoutine("Well Done " + GameResources.Instance.currentSnake.snakeName + "!" +
                " You Defeated all Enemies. Except for... The Boss/es! \n\n Find Him/Them and Finish The Job.", Color.white, 4.5f));

            yield return StartCoroutine(FadeScreen(1f, 0f, 2f, Color.Lerp(new Color(0f, 0f, 0f, 0.45f), new Color(0f, 0f, 0f, 0.3f), 1f)));
        }

        private IEnumerator LevelCompleted()
        {
            currentGameState = GameState.Playing;

            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(FadeScreen(0f, 1f, 2f, new(0f, 0f, 0f, 0.4f)));

            currentDungeonLevelListIndex++;

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

            //GetSnake().GetSnakeControler().DisableSnake();

            int rank = HighScoreManager.Instance.GetRank(gameScore);
            string rankText;

            if (rank > 0 && rank <= Settings.maxNumberOfHighScoresToSave)
            {
                rankText = $"Your Score this time was ranked {rank} on the Top {Settings.maxNumberOfHighScoresToSave}.";

                string playerName = GameResources.Instance.currentSnake.snakeName;
                if (playerName == "")
                    playerName = snakeDetails.snakeName.ToUpper();

                //Update the score
                HighScoreManager.Instance.AddScore(new Score()
                {
                    PlayerName = playerName,
                    LevelDescription = $"Level {currentDungeonLevelListIndex + 1} " +
                    $"- {GetCurrentDungeonLevel().levelName.ToUpper()}",
                    PlayerScore = gameScore
                }, rank);
            }
            else
            {
                rankText = $"Your Score could not get on the Top {Settings.maxNumberOfHighScoresToSave} this Time.\n Try Next Time!";
            }

            //yield return new WaitForSeconds(0.5f);

            //Display the game complete message
            yield return StartCoroutine(DisplayMessageRoutine($"Well Done {GameResources.Instance.currentSnake.snakeName}!\n You Defeated every Boss on Every Biome. " +
                $"\n\nYou're now the Ultimate Snake.", Color.white, 5.5f));

            yield return StartCoroutine(DisplayMessageRoutine($"Your final Score: {gameScore:###,###}. \n\n{rankText}", Color.white, 6f));
            yield return StartCoroutine(DisplayMessageRoutine($"Thanks For Playing. Head to the Exit or Press 'Enter' to restart the game", Color.white, 4f));
            yield return StartCoroutine(FadeScreen(1f, 0f, 1.5f, Color.black));

            // Change later to the new input system
            while (!Input.GetKeyDown(KeyCode.Return))
                yield return null;

            currentGameState = GameState.Restarted;
        }

        private IEnumerator GameLost()
        {
            previousGameState = GameState.GameLost;
            GetSnake().GetSnakeControler().DisableSnake();

            int rank = HighScoreManager.Instance.GetRank(gameScore);
            string rankText;

            if (rank > 0 && rank <= Settings.maxNumberOfHighScoresToSave)
            {
                rankText = $"Your Score this time was ranked {rank} on the Top {Settings.maxNumberOfHighScoresToSave}.";

                string playerName = GameResources.Instance.currentSnake.snakeName;
                if (playerName == "")
                    playerName = snakeDetails.snakeName.ToUpper();

                //Update the score
                HighScoreManager.Instance.AddScore(new Score()
                {
                    PlayerName = playerName,
                    LevelDescription = $"Level {currentDungeonLevelListIndex + 1} " +
                    $"- {GetCurrentDungeonLevel().levelName.ToUpper()}",
                    PlayerScore = gameScore
                }, rank);
            }
            else
            {
                rankText = $"Your Score could not get on the Top {Settings.maxNumberOfHighScoresToSave} this Time.\n Try Next Time!";
            }

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

            yield return StartCoroutine(DisplayMessageRoutine($"You Died {name}!" +
                    $"\nYou Failed (Miserably), But Are YOU Gonna Give Up?", Color.white, 3.5f));

            yield return StartCoroutine(DisplayMessageRoutine($"Your Final Score: {gameScore:###.###0}\n\n {rankText}", Color.white, 4f));

            yield return StartCoroutine(DisplayMessageRoutine("Press 'Enter' to Try Again", Color.white, 0f));

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

        private void CallOnLevelChangedEvent()
        {
            OnLevelChanged?.Invoke(currentDungeonLevelListIndex);
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