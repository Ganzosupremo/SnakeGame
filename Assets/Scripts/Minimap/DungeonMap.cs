using Cinemachine;
using Cysharp.Threading.Tasks;
using SnakeGame.Debuging;
using SnakeGame.GameUtilities;
using SnakeGame.ProceduralGenerationSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SnakeGame.Minimap
{
    public class DungeonMap : SingletonMonoBehaviour<DungeonMap>
    {
        #region Header GameObject References
        [Header("References")]
        [Space(10)]
        #endregion

        #region Tooltip
        [Tooltip("This is the gameobject that contains the minimap appearing" +
            " on the top right of the screen.")]
        #endregion
        public GameObject MinimapUI;

        private Camera dungeonMapCamera;
        private Camera mainCamera;
        //private Snake snake;

        private float waitBeforeTeleporting = 1f;
        private readonly float counter = 1f;

        private float _DisplayCooldown = 0.15f;
        private float _Timer = 0.15f;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            mainCamera = Camera.main;
            Transform playerTransform = GameManager.Instance.GetSnake().transform;

            GameManager.Instance.GetSnake().GetSnakeControler().GetInputActions().OverviewMap.Enable();

            // Populate the cinemachine camera target with the player
            CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            cinemachineVirtualCamera.Follow = playerTransform;

            dungeonMapCamera = GetComponentInChildren<Camera>();
            dungeonMapCamera.gameObject.SetActive(false);
        }

        private void Update()
        {
            waitBeforeTeleporting -= Time.deltaTime;
            _DisplayCooldown -= Time.deltaTime;

            if (GameManager.Instance.GetSnake().GetSnakeControler().GetInputActions().OverviewMap.Click.WasPressedThisFrame() && 
                GameManager.CurrentGameState == GameState.OverviewMap && 
                waitBeforeTeleporting <= 0f)
            {
                GetRoomClicked();
                waitBeforeTeleporting = counter;
            }
        }

        /// <summary>
        /// Displays the map on all the screen
        /// </summary>
        public void DisplayDungeonOverviewMap()
        {
            if (_DisplayCooldown >= 0f) return;

            GameManager.Instance.GetSnake().GetSnakeControler().GetInputActions().OverviewMap.Enable();

            // Set the game states
            GameManager.PreviousGameState = GameManager.CurrentGameState;
            GameManager.CurrentGameState = GameState.OverviewMap;

            // Disable the player
            GameManager.Instance.GetSnake().idleEvent.CallIdleEvent();
            GameManager.Instance.GetSnake().GetSnakeControler().DisableSnake();

            // Disable the main camera and display the overview map camera
            mainCamera.gameObject.SetActive(false);
            dungeonMapCamera.gameObject.SetActive(true);

            // Ensure all room are active when displaying the overview map
            ActivateDungeonRoomsForDisplay();

            // Disable the small minimap UI on the top right corner
            MinimapUI.SetActive(false);

            _DisplayCooldown = _Timer;
        }

        public async UniTask DisplayDungeonOverviewMapAsync()
        {
            if (_DisplayCooldown >= 0f) return;

            GameManager.Instance.GetSnake().GetSnakeControler().GetInputActions().OverviewMap.Enable();

            await UniTask.Delay(100);

            // Set the game states
            GameManager.PreviousGameState = GameManager.CurrentGameState;
            GameManager.CurrentGameState = GameState.OverviewMap;

            // Disable the player
            GameManager.Instance.GetSnake().idleEvent.CallIdleEvent();
            GameManager.Instance.GetSnake().GetSnakeControler().DisableSnake();

            // Disable the main camera and display the overview map camera
            mainCamera.gameObject.SetActive(false);
            dungeonMapCamera.gameObject.SetActive(true);

            // Ensure all room are active when displaying the overview map
            ActivateDungeonRoomsForDisplay();

            // Disable the small minimap UI on the top right corner
            MinimapUI.SetActive(false);

            _DisplayCooldown = _Timer;
        }

        public void ClearDungeonOverviewMap()
        {
            if (_DisplayCooldown >= 0f) return;

            GameManager.Instance.GetSnake().GetSnakeControler().GetInputActions().OverviewMap.Disable();

            // Restore the game states
            GameManager.CurrentGameState = GameManager.PreviousGameState;
            GameManager.PreviousGameState = GameState.OverviewMap;

            // Reenable the player
            GameManager.Instance.GetSnake().GetSnakeControler().EnableSnake();

            // Renable the main camera and disable the overview map
            mainCamera.gameObject.SetActive(true);
            dungeonMapCamera.gameObject.SetActive(false);
            
            // Enable again the small minimap UI on the top right corner
            MinimapUI.SetActive(true);

            _DisplayCooldown = _Timer;
        }

        public async  UniTask ClearDungeonMapAsync()
        {
            await UniTask.Delay(100);

            // Restore the game states
            GameManager.CurrentGameState = GameManager.PreviousGameState;
            GameManager.PreviousGameState = GameState.OverviewMap;

            // Reenable the player
            GameManager.Instance.GetSnake().GetSnakeControler().EnableSnake();

            // Renable the main camera and disable the overview map
            mainCamera.gameObject.SetActive(true);
            dungeonMapCamera.gameObject.SetActive(false);

            // Enable again the small minimap UI on the top right corner
            MinimapUI.SetActive(true);
        }

        /// <summary>
        /// Gets the room clicked and teleports the player to the clicked room
        /// </summary>
        private void GetRoomClicked()
        {
            Vector3 worldPosition = dungeonMapCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            worldPosition = new Vector3(worldPosition.x, worldPosition.y, 0f);

            // Check for collisions at the mouse position
            Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(new Vector2(worldPosition.x, worldPosition.y), 2f);

            foreach (Collider2D collider in collider2Ds)
            {
                if (collider.GetComponent<InstantiatedRoom>() != null)
                {
                    InstantiatedRoom instantiatedRoom = collider.GetComponent<InstantiatedRoom>();

                    // If the room has been cleared of enemies an has been previously visited, the player can be teleported there
                    if (instantiatedRoom.room.isClearOfEnemies && instantiatedRoom.room.isPreviouslyVisited)
                        StartCoroutine(TeleportToRoom(worldPosition, instantiatedRoom.room));
                }
            }
        }

        private IEnumerator TeleportToRoom(Vector3 worldPosition, Room room)
        {
            StaticEventHandler.CallRoomChangedEvent(room);

            // Fade the screen to black
            yield return StartCoroutine(GameManager.Instance.FadeScreen(0f, 1f, 0.1f, Color.black));

            ClearDungeonOverviewMap();

            // Disable the player
            GameManager.Instance.GetSnake().idleEvent.CallIdleEvent();
            GameManager.Instance.GetSnake().GetSnakeControler().DisableSnake();

            // Move the player to the nearest spawn position of the room
            Vector3 nearestSpawnPosition = HelperUtilities.GetNearestSpawnPointPosition(worldPosition);
            GameManager.Instance.GetSnake().transform.position = nearestSpawnPosition;

            // Return the screen back to normal
            yield return StartCoroutine(GameManager.Instance.FadeScreen(1f, 0f, 1f, Color.black));
            
            GameManager.Instance.GetSnake().GetSnakeControler().EnableSnake();
        }

        private void ActivateDungeonRoomsForDisplay()
        {
            foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.DungeonBuilderRoomDictionary)
            {
                Room room = keyValuePair.Value;

                room.instantiatedRoom.gameObject.SetActive(true);
            }
        }
    }
}
