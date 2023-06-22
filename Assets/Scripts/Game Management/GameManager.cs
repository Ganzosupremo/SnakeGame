using Cysharp.Threading.Tasks;
using SnakeGame.AudioSystem;
using SnakeGame.Debuging;
using SnakeGame.Enemies;
using SnakeGame.GameUtilities;
using SnakeGame.HealthSystem;
using SnakeGame.HighscoreSystem;
using SnakeGame.Minimap;
using SnakeGame.PlayerSystem;
using SnakeGame.ProceduralGenerationSystem;
using SnakeGame.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SnakeGame
{
    [DisallowMultipleComponent]
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        /// <summary>
        /// Triggers when the player completes a level.
        /// Passes the dungeon level index, if needed, so the susbcriber can know what level should load next. 
        /// </summary>
        public static event Action<int> OnLevelCompleted;
        private bool m_ShouldTriggerOnLevelCompletedEvent = true;

        #region Header GAME LEVELS
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

        [SerializeField] private MusicSO _GameOverMusic;

        #region Header UI REFERENCES
        [Header("UI REFERENCES")]
        [Space(5)]
        #endregion
        [SerializeField] private GameObject _ObjectivesBackground;

        /// <summary>
        /// Gets the total count of game levels
        /// </summary>
        public int LevelCount { get { return gameLevelList.Count; } }
        public bool IsFading { get; private set; } = false;
        public static GameState CurrentGameState { get { return m_CurrentGameState; } set { m_CurrentGameState = value; } }
        public static GameState PreviousGameState { get { return m_PreviousGameState; } set { m_PreviousGameState = value; } }

        private Room m_CurrentRoom;
        private Room m_PreviousRoom;

        private static GameState m_CurrentGameState;
        private static GameState m_PreviousGameState;

        private InstantiatedRoom m_BossRoom;
        private SnakeDetailsSO m_SnakeDetails;
        private Snake m_Snake;
        private long m_GameScore;
        private int m_ScoreMultiplier;
        private CancellationTokenSource m_CancellationTokenSource = new();

        protected override void Awake()
        {
            base.Awake();
            m_SnakeDetails = GameResources.Instance.currentSnake.snakeDetails;

            InstantiatePlayer();
        }

        /// <summary>
        /// Instantiates the snake in the scene
        /// </summary>
        private async void InstantiatePlayer()
        {
            GameObject snakeGameObject = Instantiate(m_SnakeDetails.snakePrefab);
            m_Snake = snakeGameObject.GetComponent<Snake>();

            await m_Snake.Initialise(m_SnakeDetails);

            StaticEventHandler.CallOnDisplayObjectivesEvent(
                Settings.DisplayObjectivesTime, 0f, 1f, $"Welcome! Find The Next Room.");
        }

        private async void Start()
        {
            m_PreviousGameState = GameState.Started;
            m_CurrentGameState = GameState.Started;
            m_GameScore = 0;
            m_ScoreMultiplier = 0;

            //StartCoroutine(FadeScreen(0f, 1f, 0f, Color.black));
            await FadeScreenAsync(0f, 1f, 0f, Color.black);
        }

        private async void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
                CurrentGameState = GameState.Started;

            await HandleGameStatesAsync();
        }

        private void OnEnable()
        {
            StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
            StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
            StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointsScored;
            StaticEventHandler.OnMultiplier += StaticEventHandler_OnMultiplier;
            m_Snake.destroyEvent.OnDestroy += Snake_OnDestroy;
        }

        private void OnDisable()
        {
            m_CancellationTokenSource.Cancel();
            StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
            StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
            StaticEventHandler.OnPointsScored -= StaticEventHandler_OnPointsScored;
            StaticEventHandler.OnMultiplier -= StaticEventHandler_OnMultiplier;
            m_Snake.destroyEvent.OnDestroy -= Snake_OnDestroy;
        }

        private void OnDestroy()
        {
            m_CancellationTokenSource.Dispose();
        }

        private void Snake_OnDestroy(DestroyEvent destroyEvent, DestroyedEventArgs destroyedEventArgs)
        {
            m_PreviousGameState = m_CurrentGameState;
            m_CurrentGameState = GameState.GameLost;
        }

        private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
        {
            EnemiesDefeated();
        }

        private void StaticEventHandler_OnMultiplier(MultiplierArgs multiplierArgs)
        {
            if (multiplierArgs.multiplier)
                m_ScoreMultiplier++;
            else
                m_ScoreMultiplier--;

            m_ScoreMultiplier = Mathf.Clamp(m_ScoreMultiplier, 1, 69);
            StaticEventHandler.CallScoreChangedEvent(m_GameScore, m_ScoreMultiplier);
        }

        private void StaticEventHandler_OnPointsScored(PointsScoredArgs pointsScoredArgs)
        {
            m_GameScore += pointsScoredArgs.score * m_ScoreMultiplier;
            StaticEventHandler.CallScoreChangedEvent(m_GameScore, m_ScoreMultiplier);
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
            switch (m_CurrentGameState)
            {
                case GameState.Started:

                    PlayGameLevel(currentDungeonLevelListIndex);
                    m_CurrentGameState = GameState.Playing;
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
                    if (m_PreviousGameState != GameState.GameWon)
                        //GameWonAsync();
                        StartCoroutine(GameWon());

                    break;
                case GameState.GameLost:
                    // Just call this once
                    if (m_PreviousGameState != GameState.GameLost)
                    {
                        m_CancellationTokenSource.Cancel();
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

        private async UniTask HandleGameStatesAsync()
        {
            switch (m_CurrentGameState)
            {
                case GameState.Started:
                    PlayGameLevel(currentDungeonLevelListIndex);
                    m_CurrentGameState = GameState.Playing;
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
                    await LevelCompletedAsync(m_CancellationTokenSource.Token);
                    break;
                case GameState.GameWon:
                    if (GetSnake().GetSnakeControler().GetInputActions().Snake.Pause.WasPressedThisFrame())
                        PauseGameMenu();

                    if (GetSnake().GetSnakeControler().GetInputActions().Snake.DisplayMap.IsPressed())
                        DisplayOverviewMap();

                    // Just call this once
                    if (m_PreviousGameState != GameState.GameWon)
                        await GameWonAsync(m_CancellationTokenSource.Token);
                    break;
                case GameState.GameLost:
                    // Just call this once
                    if (m_PreviousGameState != GameState.GameLost)
                    {
                        //StopAllCoroutines();
                        await GameLostAsync(m_CancellationTokenSource.Token);
                        m_CancellationTokenSource.Cancel();
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

        private async void PlayGameLevel(int currentDungeonLevelListIndex)
        {
            bool dungeonBuiltSuccesfully = DungeonBuilder.Instance.GenerateDungeon(gameLevelList[currentDungeonLevelListIndex]);

            if (!dungeonBuiltSuccesfully)
                this.LogError("Couldn't build dungeon from the specified node graphs");

            // Trigger the room changed event on the first load
            StaticEventHandler.CallRoomChangedEvent(m_CurrentRoom);

            // Set the snake position roughly in the middle of the room
            m_Snake.gameObject.transform.position = new Vector3((m_CurrentRoom.lowerBounds.x + m_CurrentRoom.upperBounds.x) / 2f,
                (m_CurrentRoom.lowerBounds.y + m_CurrentRoom.upperBounds.y) / 2f, 0f);

            // Get the nearest spawn point position of the room, so the snake doesn't spawn in walls or something
            m_Snake.gameObject.transform.position = HelperUtilities.GetNearestSpawnPointPosition(m_Snake.gameObject.transform.position);

            CallOnLevelCompletedEvent();

            await ShowLevelNameAsync(m_CancellationTokenSource.Token);
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

        private async UniTask ShowLevelNameAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            await FadeScreenAsync(0f, 1f, 0f, Color.black);

            GetSnake().GetSnakeControler().DisableSnake();

            string levelName = "LOADING LEVEL... \n\n" + gameLevelList[currentDungeonLevelListIndex].levelName.ToUpper();

            await DisplayMessageAsync(levelName, Color.white, 4.2f, m_CancellationTokenSource.Token);
            GetSnake().GetSnakeControler().EnableSnake();

            await FadeScreenAsync(1f, 0f, 2f, Color.black);
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
        /// Displays a message on the screen, if the timeSeconds equals zero
        /// then the method will wait until the enter key is pressed.
        /// </summary>
        /// <param name="levelName">The name of the current level</param>
        /// <param name="textColor">The color of the text</param>
        /// <param name="timeSeconds">The time that the message will appear</param>
        private async UniTask DisplayMessageAsync(string levelName, Color textColor, float timeSeconds, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            loadScreenText.SetText(levelName);
            loadScreenText.color = textColor;

            // Display the text for a given period of time
            if (timeSeconds > 0f)
            {
                float timer = timeSeconds;

                while (timer > 0f && !Input.GetKeyDown(KeyCode.Return))
                {
                    timer -= Time.deltaTime;
                    await UniTask.NextFrame();
                }
            }
            else
            {
                while (!Input.GetKeyDown(KeyCode.Return))
                {
                    await UniTask.NextFrame();
                }
            }

            await UniTask.NextFrame();

            // Clear the text
            loadScreenText.SetText("");
        }

        //private async UniTask DisplayCurrentObjectiveAsync(float displayTime, CancellationToken cancellationToken,params string[] displayTexts)
        //{
        //    if (cancellationToken.IsCancellationRequested) return;

        //    if (displayTime <= 0f)
        //        displayTime = Settings.DisplayObjectivesTime;

        //    ObjectiveUI objectiveUI = _ObjectivesBackground.GetComponentInParent<ObjectiveUI>();
        //    if (objectiveUI != null)
        //    {
        //        await objectiveUI.Display(0f, 1f, displayTime, displayTexts);
        //    }
        //}

        /// <summary>
        /// Loads the next game level if all rooms have no more enemies.
        /// </summary>
        private async void EnemiesDefeated()
        {
            bool isDungeonClearOfNormalEnemies = true;
            m_BossRoom = null;

            // See if all rooms have been cleared of enemies
            foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.DungeonBuilderRoomDictionary)
            {
                // Skip the boos room for the moment
                if (keyValuePair.Value.roomNodeType.isBossRoom)
                {
                    m_BossRoom = keyValuePair.Value.InstantiatedRoom;
                    continue;
                }

                // Check if the other rooms have been cleared of enemies
                if (!keyValuePair.Value.IsClearOfEnemies)
                {
                    isDungeonClearOfNormalEnemies = false;
                    break;
                }
            }

            // Set the game state
            // If the dungeon level has been cleared completely - all the normal rooms have been cleared except the boss room,
            // Or if there's no boss room
            // Or all rooms and the boss room have been cleared
            if ((isDungeonClearOfNormalEnemies && m_BossRoom == null) || (isDungeonClearOfNormalEnemies && m_BossRoom.room.IsClearOfEnemies))
            {
                // If there are more dungeon level, then 
                if (currentDungeonLevelListIndex < gameLevelList.Count - 1)
                {
                    m_ShouldTriggerOnLevelCompletedEvent = true;
                    m_CurrentGameState = GameState.LevelCompleted;
                }
                else
                {
                    m_ShouldTriggerOnLevelCompletedEvent = false;
                    m_CurrentGameState = GameState.GameWon;
                }
            }
            //If just the boss room is not cleared yet
            else if (isDungeonClearOfNormalEnemies)
            {
                m_CurrentGameState = GameState.BossStage;
                await BossStageAsync(m_CancellationTokenSource.Token);
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
            DisplayMusicUI.CanDisplay = false;

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
            DisplayMusicUI.CanDisplay = true;
        }

        public async UniTask FadeScreenAsync(float currentAlpha, float targetAlpha, float timeSeconds, Color screenColor, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested) return;

            try
            {
                IsFading = true;
                DisplayMusicUI.CanDisplay = false;

                Image image = canvasGroup.GetComponent<Image>();
                image.color = screenColor;
                float timer = 0f;

                while (timer <= timeSeconds)
                {
                    timer += Time.deltaTime;
                    canvasGroup.alpha = Mathf.Lerp(currentAlpha, targetAlpha, timer / timeSeconds);
                    await UniTask.Yield();
                }

                IsFading = false;
                DisplayMusicUI.CanDisplay = true;
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        /// <summary>
        /// Use this method when you wanna call ShowMessageRoutine() and the StartCoroutine() method is not available.
        /// </summary>
        /// <param name="messageToDisplay"></param>
        /// <param name="time"></param>
        public void CallShowMesageRoutine(string messageToDisplay, float time)
        {
            StartCoroutine(ShowMessageRoutine(messageToDisplay, time));
        }
        
        /// <summary>
        /// Shows a message on the game UI
        /// </summary>
        /// <param name="messageToDisplay"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public IEnumerator ShowMessageRoutine(string messageToDisplay, float time)
        {
            if (IsFading) yield break;

            feedbackText.text = messageToDisplay;
            yield return new WaitForSeconds(time);
            feedbackText.text = "";
        }

        /// <summary>
        /// Shows a message on the game UI
        /// </summary>
        /// <param name="messageToDisplay"></param>
        /// <param name="displayTime"></param>
        public async UniTask ShowMessageAsync(string messageToDisplay, float displayTime, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested) return;
            if (IsFading) return;

            feedbackText.text = messageToDisplay;
            await UniTask.Delay((int)displayTime * 1000);
            feedbackText.text = "";
        }

        #region Different Game States
        private IEnumerator BossStage()
        {
            m_BossRoom.gameObject.SetActive(true);
            //await m_bossRoom.UnlockDoorsAsync(0f);
            m_BossRoom.UnlockDoors(0f);

            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(FadeScreen(0f, 1f, 2f, Color.Lerp(new Color(0f, 0f, 0f, 0.45f), new Color(0f, 0f, 0f, 0.3f), 1f)));

            yield return StartCoroutine(DisplayMessageRoutine("Well Done " + GameResources.Instance.currentSnake.snakeName + "!" +
                " You Defeated all Enemies. Except for... The Boss/es! \n\n Find Him/Them and Finish The Job.", Color.white, 4.5f));

            yield return StartCoroutine(FadeScreen(1f, 0f, 2f, Color.Lerp(new Color(0f, 0f, 0f, 0.45f), new Color(0f, 0f, 0f, 0.3f), 1f)));
        }

        private async UniTask BossStageAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;
            
            m_BossRoom.gameObject.SetActive(true);
            await m_BossRoom.UnlockDoorsAsync(0f);

            await UniTask.Delay(1000);

            StaticEventHandler.CallOnDisplayObjectivesEvent(Settings.DisplayObjectivesTime, 0f, 1f, $"Defeat the Boss!");
        }

        private IEnumerator LevelCompleted()
        {
            m_CurrentGameState = GameState.Playing;

            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(FadeScreen(0f, 1f, 2f, new(0f, 0f, 0f, 0.4f)));

            currentDungeonLevelListIndex++;

            string name = GameResources.Instance.currentSnake.snakeName;
            if (name == "") name = m_SnakeDetails.snakeName.ToUpper();

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

        private async UniTask LevelCompletedAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            m_CurrentGameState = GameState.Playing;
            await UniTask.Delay(1000);
            await FadeScreenAsync(0f, 1f, 2f, new(0f, 0f, 0f, 0.4f));

            currentDungeonLevelListIndex++;
            CallOnLevelCompletedEvent();

            string name = GameResources.Instance.currentSnake.snakeName;
            if (name == "") name = m_SnakeDetails.snakeName.ToUpper();

            await DisplayMessageAsync($"Well Done {name}!" +
                $"\n You defeated the boss on this biome, but there are more out there...", Color.white, 3.5f, cancellationToken);

            await DisplayMessageAsync($"Press 'Enter'" +
                "\n or head to the exit room, to continue your Journey.", Color.white, 3.5f, cancellationToken);

            //StaticEventHandler.CallOnDisplayObjectivesEvent(Settings.DisplayObjectivesTime, 0f, 1f, $"Well Done {name}! Find the exit or press 'Enter' to continue.");
            await FadeScreenAsync(1f, 0f, 2f, new(0f, 0f, 0f, 0.4f));

            while (!Input.GetKeyDown(KeyCode.Return))
            {
                await UniTask.NextFrame();
            }

            PlayGameLevel(currentDungeonLevelListIndex);
        }

        private IEnumerator GameWon()
        {
            m_PreviousGameState = GameState.GameWon;

            //Fade in the canvas to display a message
            yield return StartCoroutine(FadeScreen(0f, 1f, 2f, Color.black));

            //GetSnake().GetSnakeControler().DisableSnake();

            int rank = HighScoreManager.Instance.GetRank(m_GameScore);
            string rankText;

            if (rank > 0 && rank <= Settings.maxNumberOfHighScoresToSave)
            {
                rankText = $"Your Score this time was ranked {rank} on the Top {Settings.maxNumberOfHighScoresToSave}.";

                string playerName = GameResources.Instance.currentSnake.snakeName;
                if (playerName == "")
                    playerName = m_SnakeDetails.snakeName.ToUpper();

                //Update the score
                HighScoreManager.Instance.AddScore(new Score()
                {
                    PlayerName = playerName,
                    LevelDescription = $"Level {currentDungeonLevelListIndex + 1} " +
                    $"- {GetCurrentDungeonLevel().levelName.ToUpper()}",
                    PlayerScore = m_GameScore
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

            yield return StartCoroutine(DisplayMessageRoutine($"Your final Score: {m_GameScore:###,###}. \n\n{rankText}", Color.white, 6f));
            yield return StartCoroutine(DisplayMessageRoutine($"Thanks For Playing. Head to the Exit or Press 'Enter' to restart the game", Color.white, 4f));
            yield return StartCoroutine(FadeScreen(1f, 0f, 1.5f, Color.black));

            // Change later to the new input system
            while (!Input.GetKeyDown(KeyCode.Return))
                yield return null;

            m_CurrentGameState = GameState.Restarted;
        }

        private async UniTask GameWonAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            m_PreviousGameState = GameState.GameWon;

            await FadeScreenAsync(0f, 1f, 2f, Color.black);

            int rank = HighScoreManager.Instance.GetRank(m_GameScore);
            string rankText;

            if (rank > 0 && rank <= Settings.maxNumberOfHighScoresToSave)
            {
                rankText = $"Your Score this time was ranked {rank} on the Top {Settings.maxNumberOfHighScoresToSave}.";

                string playerName = GameResources.Instance.currentSnake.snakeName;
                if (playerName == "")
                    playerName = m_SnakeDetails.snakeName.ToUpper();

                //Update the score
                HighScoreManager.Instance.AddScore(new Score()
                {
                    PlayerName = playerName,
                    LevelDescription = $"Level {currentDungeonLevelListIndex + 1} " +
                    $"- {GetCurrentDungeonLevel().levelName.ToUpper()}",
                    PlayerScore = m_GameScore
                }, rank);
            }
            else
            {
                rankText = $"Your Score could not get on the Top {Settings.maxNumberOfHighScoresToSave} this Time.\n Try Next Time!";
            }

            await DisplayMessageAsync($"Well Done {GameResources.Instance.currentSnake.snakeName}!\n You Defeated every Boss on Every Biome. " +
                $"\n\nYou're now the Ultimate Snake.", Color.white, 5.5f, m_CancellationTokenSource.Token);
            await DisplayMessageAsync($"Your final Score: {m_GameScore:###,###}. \n\n{rankText}", Color.white, 6f, m_CancellationTokenSource.Token);
            await DisplayMessageAsync($"Thanks For Playing! Press 'Enter' to restart the game", Color.white, 0f, m_CancellationTokenSource.Token);
            await FadeScreenAsync(1f, 0f, 1.5f, Color.black);

            // Change later to the new input system
            while (!Input.GetKeyDown(KeyCode.Return))
                await UniTask.NextFrame();

            m_CurrentGameState = GameState.Restarted;
        }

        private IEnumerator GameLost()
        {
            m_PreviousGameState = GameState.GameLost;
            GetSnake().GetSnakeControler().DisableSnake();

            int rank = HighScoreManager.Instance.GetRank(m_GameScore);
            string rankText;

            if (rank > 0 && rank <= Settings.maxNumberOfHighScoresToSave)
            {
                rankText = $"Your Score this time was ranked {rank} on the Top {Settings.maxNumberOfHighScoresToSave}.";

                string playerName = GameResources.Instance.currentSnake.snakeName;
                if (playerName == "")
                    playerName = m_SnakeDetails.snakeName.ToUpper();

                //Update the score
                HighScoreManager.Instance.AddScore(new Score()
                {
                    PlayerName = playerName,
                    LevelDescription = $"Level {currentDungeonLevelListIndex + 1} " +
                    $"- {GetCurrentDungeonLevel().levelName.ToUpper()}",
                    PlayerScore = m_GameScore
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
            if (name == "") name = m_SnakeDetails.snakeName.ToUpper();

            yield return StartCoroutine(DisplayMessageRoutine($"You Died {name}!" +
                    $"\nYou Failed (Miserably), But Are YOU Gonna Give Up?", Color.white, 3.5f));

            yield return StartCoroutine(DisplayMessageRoutine($"Your Final Score: {m_GameScore:###.###}\n\n {rankText}", Color.white, 4f));

            yield return StartCoroutine(DisplayMessageRoutine("Press 'Enter' to Try Again", Color.white, 0f));

            m_CurrentGameState = GameState.Restarted;
        }

        private async UniTask GameLostAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            m_PreviousGameState = GameState.GameLost;
            GetSnake().GetSnakeControler().DisableSnake();

            MusicManager.CallOnMusicClipChangedEvent(_GameOverMusic);

            int rank = HighScoreManager.Instance.GetRank(m_GameScore);
            string rankText;

            if (rank > 0 && rank <= Settings.maxNumberOfHighScoresToSave)
            {
                rankText = $"Your Score this time was ranked {rank} on the Top {Settings.maxNumberOfHighScoresToSave}.";

                string playerName = GameResources.Instance.currentSnake.snakeName;
                if (playerName == "")
                    playerName = m_SnakeDetails.snakeName.ToUpper();

                //Update the score
                HighScoreManager.Instance.AddScore(new Score()
                {
                    PlayerName = playerName,
                    LevelDescription = $"Level {currentDungeonLevelListIndex + 1} " +
                    $"- {GetCurrentDungeonLevel().levelName.ToUpper()}",
                    PlayerScore = m_GameScore
                }, rank);
            }
            else
            {
                rankText = $"Your Score could not get on the Top {Settings.maxNumberOfHighScoresToSave} this Time.\n Try Next Time!";
            }

            await UniTask.Delay(1000);
            await FadeScreenAsync(0f, 1f, 2f, Color.black);

            // Disable the enemies that are present(FindObjectOfType requires a lot of resourcess - it's ok in this stage of the game)
            Enemy[] enemiesArray = FindObjectsOfType<Enemy>();
            foreach (Enemy enemy in enemiesArray)
            {
                enemy.gameObject.SetActive(false);
            }

            string name = GameResources.Instance.currentSnake.snakeName;
            if (name == "") name = m_SnakeDetails.snakeName.ToUpper();

            await DisplayMessageAsync($"You Died {name}!" +
                    $"\nYou Failed (Miserably), But Are YOU Gonna Give Up?", Color.white, 3.5f, m_CancellationTokenSource.Token);
            await DisplayMessageAsync($"Your Final Score: {m_GameScore:###.###}\n\n {rankText}", Color.white, 4f, m_CancellationTokenSource.Token);
            await DisplayMessageAsync("Press 'Enter' to Try Again", Color.white, 0f,m_CancellationTokenSource.Token);

            m_CurrentGameState = GameState.Restarted;
        }

        public void PauseGameMenu()
        {
            if (m_CurrentGameState != GameState.Paused)
            {
                pauseMenuUI.SetActive(true);
                GetSnake().GetSnakeControler().DisableSnake();

                // Set the game states
                m_PreviousGameState = m_CurrentGameState;
                m_CurrentGameState = GameState.Paused;
            }
            else if (m_CurrentGameState == GameState.Paused)
            {
                pauseMenuUI.SetActive(false);
                GetSnake().GetSnakeControler().EnableSnake();

                // Restore the game states
                m_CurrentGameState = m_PreviousGameState;
                m_PreviousGameState = GameState.Paused;
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

        private void CallOnLevelCompletedEvent()
        {
            if (!m_ShouldTriggerOnLevelCompletedEvent) return;

            OnLevelCompleted?.Invoke(currentDungeonLevelListIndex);
        }
        #endregion

        #region Setters
        /// <summary>
        /// Sets the current room the player is in
        /// </summary>
        /// <param name="room">The room to set as the current room</param>
        public void SetCurrentRoom(Room room)
        {
            m_PreviousRoom = m_CurrentRoom;
            m_CurrentRoom = room;
        }
        #endregion

        #region Getters
        /// <summary>
        /// Gets the current room 
        /// </summary>
        /// <returns>The room the player currently is in</returns>
        public Room GetCurrentRoom()
        {
            return m_CurrentRoom;
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
            return m_Snake;
        }

        public Sprite GetMinimapIcon()
        {
            return m_SnakeDetails.snakeMinimapIcon;
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