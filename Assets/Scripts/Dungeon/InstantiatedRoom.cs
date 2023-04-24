using Cysharp.Threading.Tasks;
using SnakeGame.Decorations;
using SnakeGame.Dungeon.NoiseGenerator;
using SnakeGame.GameUtilities;
using SnakeGame.PlayerSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SnakeGame.ProceduralGenerationSystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider2D))]
    public class InstantiatedRoom : MonoBehaviour
    {
        [HideInInspector] public Room room;
        [HideInInspector] public Grid grid;

        public Tilemap Groundtilemap { get; private set; }
        public Tilemap Decorations1Tilemap { get; private set; }
        public Tilemap Decorations2Tilemap { get; private set; }
        public Tilemap FrontTilemap { get; private set; }
        public Tilemap CollisionTilemap { get; private set; }
        public Tilemap MinimapTilemap { get; private set; }

        [HideInInspector] public int[,] aStarMovementPenalty; // This is used to store the movement penalties for the AStar Pathfinding
        [HideInInspector] public int[,] aStarItemObstacles; // Store the position of moveable items which acts as an obstacle
                                                            // ***** test code
        [HideInInspector] public int[,] aStarSnakeSegmentsObstacles;
        // *****

        [HideInInspector] public Bounds roomColliderBounds;
        [HideInInspector] public List<MoveableObstacle> moveableItemsList = new();
        // ****** test code
        [HideInInspector] public List<SnakeBody> snakeBodyList = new();
        // ******

        private BoxCollider2D boxCollider2D;
        private NoiseMap noiseMap;
        private MapPresetSO mapPreset;

        #region Header References
        [Header("REFERENCES")]
        [Tooltip("Populate with the environment child placeholder gameobject")]
        #endregion
        [SerializeField] private GameObject environmentGameObject;
        public PolygonCollider2D cameraConfinerCollider;

        private void Awake()
        {
            boxCollider2D = GetComponent<BoxCollider2D>();
            noiseMap = GetComponent<NoiseMap>();
            roomColliderBounds = boxCollider2D.bounds;
        }

        private void Start()
        {
            UpdateMoveableObstacles();
            //UpdateSnakeSegmenstObstacles();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Settings.PlayerTag) && room != GameManager.Instance.GetCurrentRoom())
            {
                room.isPreviouslyVisited = true;
                StaticEventHandler.CallRoomChangedEvent(room);
            }

        }

        //private void OnTriggerExit2D(Collider2D other)
        //{
        //    if (other.CompareTag(Settings.playerTag) && room != GameManager.Instance.GetCurrentRoom())
        //        this.room.isPreviouslyVisited = true;
        //}

        //private void OnDrawGizmos()
        //{
        //    try
        //    {
        //        for (int i = 0; i < (room.tilemapUpperBounds.x - room.tilemapLowerBounds.x + 1); i++)
        //        {
        //            for (int j = 0; j < (room.tilemapUpperBounds.y - room.tilemapLowerBounds.y + 1); j++)
        //            {
        //                if (aStarItemObstacles[i, j] == 0)
        //                {
        //                    Vector3 worldCellPos = grid.CellToWorld(new Vector3Int(i + room.tilemapLowerBounds.x,
        //                        j + room.tilemapLowerBounds.y, 0));

        //                    Gizmos.DrawWireCube(new Vector3(worldCellPos.x + 0.5f, worldCellPos.y + 0.5f, 0f), Vector3.one);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.LogException(e);
        //        throw;
        //    }
        //}

        /// <summary>
        /// Initialise the instantiated room.
        /// </summary>
        public void Initialise(GameObject roomGameObject)
        {
            PopulateTilemapMemberVariables(roomGameObject);

            BlockOffUnusedDoorways();

            AddObstaclesAndPreferredPaths();

            CreateObstaclesArray();
            // test code
            CreateSegmetsArray();

            AddDoorsToRooms();

            DisableCollisionTilemapRenderer();
        }

        /// <summary>
        /// Populate The Grid And Tilemap Variables Specified Above.
        /// </summary>
        private void PopulateTilemapMemberVariables(GameObject roomGameobject)
        {
            //Get the grid component
            grid = roomGameobject.GetComponentInChildren<Grid>();

            //Get the tilemap component in the children
            Tilemap[] tilemaps = roomGameobject.GetComponentsInChildren<Tilemap>();

            foreach (Tilemap tilemap in tilemaps)
            {
                if (tilemap.gameObject.CompareTag("Tilemap_Ground"))
                {
                    Groundtilemap = tilemap;
                }
                else if (tilemap.gameObject.CompareTag("Tilemap_Decorations1"))
                {
                    Decorations1Tilemap = tilemap;
                }
                else if (tilemap.gameObject.CompareTag("Tilemap_Decorations2"))
                {
                    Decorations2Tilemap = tilemap;
                }
                else if (tilemap.gameObject.CompareTag("Tilemap_Front"))
                {
                    FrontTilemap = tilemap;
                }
                else if (tilemap.gameObject.CompareTag("Tilemap_Collision"))
                {
                    CollisionTilemap = tilemap;
                }
                else if (tilemap.gameObject.CompareTag("Tilemap_Minimap"))
                {
                    MinimapTilemap = tilemap;
                }
            }
        }

        private void GenerateProceduralMap()
        {
            this.mapPreset = noiseMap.mapPreset;
            noiseMap.GenerateMap(mapPreset);
        }

        /// <summary>
        /// Blocks Off Unused Doorways Of The Room
        /// </summary>
        private void BlockOffUnusedDoorways()
        {
            foreach (Doorway doorway in room.doorwayList)
            {
                if (doorway.isConnected)
                    continue;

                if (Groundtilemap != null)
                    BlockADoorwayOnTilemap(Groundtilemap, doorway);
                if (Decorations1Tilemap != null)
                    BlockADoorwayOnTilemap(Decorations1Tilemap, doorway);
                if (Decorations2Tilemap != null)
                    BlockADoorwayOnTilemap(Decorations2Tilemap, doorway);
                if (FrontTilemap != null)
                    BlockADoorwayOnTilemap(FrontTilemap, doorway);
                if (CollisionTilemap != null)
                    BlockADoorwayOnTilemap(CollisionTilemap, doorway);
                if (MinimapTilemap != null)
                    BlockADoorwayOnTilemap(MinimapTilemap, doorway);
            }
        }

        /// <summary>
        /// Blocks The Doorway On The Corresponding Tilemap
        /// </summary>
        private void BlockADoorwayOnTilemap(Tilemap tilemap, Doorway doorway)
        {
            switch (doorway.doorOrientation)
            {
                case Orientation.North:
                case Orientation.South:
                    BlockDoorwayHorizontally(tilemap, doorway);
                    break;
                case Orientation.East:
                case Orientation.West:
                    BlockDoorwayVertically(tilemap, doorway);
                    break;

                case Orientation.None:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Blocks the doorways horizontally
        /// </summary>
        /// <param name="tilemap"></param>
        /// <param name="doorway"></param>
        private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
        {
            Vector2Int copyStartPosition = doorway.doorwayStartCopyPosition;

            // loop through all the tiles in x position to copy
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
            {
                // loop through all tiles in y position to copy
                for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
                {
                    // Get the rotation of the tile that is gonna be copied
                    Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(copyStartPosition.x + xPos, copyStartPosition.y - yPos, 0));

                    // Copy the tile
                    tilemap.SetTile(new Vector3Int(copyStartPosition.x + 1 + xPos, copyStartPosition.y - yPos, 0),
                        tilemap.GetTile(new Vector3Int(copyStartPosition.x + xPos, copyStartPosition.y - yPos, 0)));

                    // Set the correct rotation of the copied tile
                    tilemap.SetTransformMatrix(new Vector3Int(copyStartPosition.x + 1 + xPos, copyStartPosition.y - yPos, 0), transformMatrix);
                }
            }
        }

        /// <summary>
        /// Blocks the doorways vertically
        /// </summary>
        /// <param name="tilemap"></param>
        /// <param name="doorway"></param>
        private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
        {
            Vector2Int copyStartPosition = doorway.doorwayStartCopyPosition;

            // loop through all the tiles in y position to copy
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
            {
                // loop through all tiles in x position to copy
                for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
                {
                    // Get the rotation of the tile that is gonna be copied
                    Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(copyStartPosition.x + xPos,
                        copyStartPosition.y - yPos, 0));

                    //Copy the tile
                    tilemap.SetTile(new Vector3Int(copyStartPosition.x + xPos, copyStartPosition.y - 1 - yPos, 0),
                        tilemap.GetTile(new Vector3Int(copyStartPosition.x + xPos, copyStartPosition.y - yPos, 0)));

                    // Set the correct rotation of the copied tile
                    tilemap.SetTransformMatrix(new Vector3Int(copyStartPosition.x + xPos, copyStartPosition.y - 1 - yPos, 0), transformMatrix);
                }
            }
        }

        /// <summary>
        /// Update Obstacles Used In The AStar Pathfinding
        /// </summary>
        private void AddObstaclesAndPreferredPaths()
        {
            // This array will be populated with wall obstacles
            aStarMovementPenalty = new int[room.tilemapUpperBounds.x - room.tilemapLowerBounds.x + 1,
                room.tilemapUpperBounds.y - room.tilemapLowerBounds.y + 1];

            // Loop through all grid squares
            for (int x = 0; x < (room.tilemapUpperBounds.x - room.tilemapLowerBounds.x + 1); x++)
            {
                for (int y = 0; y < room.tilemapUpperBounds.y - room.tilemapLowerBounds.y + 1; y++)
                {
                    aStarMovementPenalty[x, y] = Settings.defaultAStarMovementPenalty;

                    TileBase tile = CollisionTilemap.GetTile(new Vector3Int(x + room.tilemapLowerBounds.x, y + room.tilemapLowerBounds.y, 0));

                    foreach (TileBase collTile in GameResources.Instance.enemyUnwalkableCollisionTilesArray)
                    {
                        if (tile == collTile)
                        {
                            aStarMovementPenalty[x, y] = 0;
                            break;
                        }
                    }

                    // Add preferred path for enemies, 1 is the preferred path value.
                    // The default value is 69 an is set in the Settings file
                    if (tile == GameResources.Instance.preferredEnemyPathTile)
                    {
                        aStarMovementPenalty[x, y] = Settings.preferredPathAStarMovementPenalty;
                    }
                }
            }
        }

        /// <summary>
        /// Adds the doorways to the instantiated room
        /// </summary>
        private void AddDoorsToRooms()
        {
            if (room.roomNodeType.isCorridorEW || room.roomNodeType.isCorridorNS) return;

            // Instantiate door prefabs at the doorway position
            foreach (Doorway doorway in room.doorwayList)
            {
                if (doorway.doorPrefab != null && doorway.isConnected)
                {
                    float tileDistance = Settings.tileSizePixels / Settings.pixelsPerUnit;

                    GameObject door = null;

                    switch (doorway.doorOrientation)
                    {
                        case Orientation.North:
                            door = Instantiate(doorway.doorPrefab, gameObject.transform);
                            // Position the door correctly on the map
                            door.transform.localPosition = new Vector3(doorway.doorPosition.x + tileDistance / 2,
                                doorway.doorPosition.y + tileDistance, 0f);
                            break;
                        case Orientation.East:
                            door = Instantiate(doorway.doorPrefab, gameObject.transform);
                            // Position the door correctly on the map
                            door.transform.localPosition = new Vector3(doorway.doorPosition.x + tileDistance,
                                doorway.doorPosition.y + tileDistance * 1.25f, 0f);
                            break;
                        case Orientation.South:
                            door = Instantiate(doorway.doorPrefab, gameObject.transform);
                            // Position the door correctly on the map
                            door.transform.localPosition = new Vector3(doorway.doorPosition.x + tileDistance / 2, doorway.doorPosition.y, 0f);
                            break;
                        case Orientation.West:

                            door = Instantiate(doorway.doorPrefab, gameObject.transform);
                            // Position the door correctly on the map
                            door.transform.localPosition = new Vector3(doorway.doorPosition.x,
                                doorway.doorPosition.y + tileDistance * 1.25f, 0f);
                            break;
                        default:
                            break;
                    }

                    Door doorComponent = door.GetComponent<Door>();

                    if (room.roomNodeType.isBossRoom)
                    {
                        doorComponent.isBossRoomDoor = true;

                        // Prevent access until all enemies in the other rooms have been cleared
                        doorComponent.LockDoors();

                        GameObject bossRoomCue = Instantiate(GameResources.Instance.bossRoomCuePrefab, gameObject.transform);
                        bossRoomCue.transform.localPosition = door.transform.localPosition;
                    }
                }
            }
        }

        /// <summary>
        /// Disable The Collision Tilemap Renderer
        /// </summary>
        private void DisableCollisionTilemapRenderer()
        {
            CollisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
        }

        public void DeactivateEnvironmentObjects()
        {
            if (environmentGameObject != null)
            {
                environmentGameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Disables the trigger room collider that is used to know when a player has enter a room
        /// </summary>
        private void DisableRoomCollider(bool isActive)
        {
            boxCollider2D.enabled = isActive;
        }

        public void ActivateEnvironmentObjects()
        {
            if (environmentGameObject != null)
            {
                environmentGameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Locks the doors when the player enters a new room
        /// and this room does not have set to true the bool 'isClearOfEnemies
        /// </summary>
        public IEnumerator LockDoors(float lockDelay)
        {
            Door[] doors = GetComponentsInChildren<Door>();

            // Wait a little before locking the doors
            // so that the player can enter the room without colliding
            // with the door collider when the door is closed.
            yield return new WaitForSeconds(lockDelay);

            // Lock each door
            foreach (Door door in doors)
            {
                door.LockDoors();
            }

            DisableRoomCollider(false);
        }

        /// <summary>
        /// Locks the door when the player enters a new room.
        /// </summary>
        public async UniTask LockDoorsAsync(float lockDelay = Settings.DoorLockDelay)
        {
            Door[] doors = GetComponentsInChildren<Door>();

            var task = new UniTask[doors.Length];
            await UniTask.Delay((int)lockDelay * 1000);

            for (int i = 0; i < doors.Length; i++)
            {
                task[i] = doors[i].LockDoorsAsync();
            }

            await UniTask.WhenAll(task);

            DisableRoomCollider(false);
        }

        /// <summary>
        /// Unlocks the doors on the room.
        /// </summary>
        /// <param name="unlockDelay"></param>
        public void UnlockDoors(float unlockDelay)
        {
            StartCoroutine(UnlockDoorsRoutine(unlockDelay));
        }

        private IEnumerator UnlockDoorsRoutine(float unlockDelay)
        {
            if (unlockDelay > 0f)
                yield return new WaitForSeconds(unlockDelay);

            Door[] doors = GetComponentsInChildren<Door>();

            foreach (Door door in doors)
            {
                door.UnlockDoor();
            }

            DisableRoomCollider(true);
        }

        //public async void UnlockDoorsAsync(float unlockDelay)
        //{
        //    await UnlockDoorsAsync(unlockDelay);
        //}

        /// <summary>
        /// Unlocks the doors on the room.
        /// </summary>
        /// <param name="unlockDelay"></param>
        /// <returns></returns>
        public async UniTask UnlockDoorsAsync(float unlockDelay)
        {
            if (unlockDelay > 0f)
                await UniTask.Delay((int)unlockDelay * 1000);

            Door[] doors = GetComponentsInChildren<Door>();

            foreach (Door door in doors)
            {
                door.UnlockDoor();
            }
            DisableRoomCollider(true);
        }

        private void CreateObstaclesArray()
        {
            aStarItemObstacles = new int[room.tilemapUpperBounds.x - room.tilemapLowerBounds.x + 1,
                room.tilemapUpperBounds.y - room.tilemapLowerBounds.y + 1];
        }

        /// <summary>
        /// Set the default A* penalty on the obstacles array
        /// </summary>
        private void InitializeObstaclesArray()
        {
            for (int x = 0; x < (room.tilemapUpperBounds.x - room.tilemapLowerBounds.x + 1); x++)
            {
                for (int y = 0; y < (room.tilemapUpperBounds.y - room.tilemapLowerBounds.y + 1); y++)
                {
                    // Set the default penalty for the grid squares, with higher penalty the enemies will avoid this grid squares
                    aStarItemObstacles[x, y] = Settings.defaultAStarMovementPenalty;
                }
            }
        }

        // test code
        private void CreateSegmetsArray()
        {
            aStarSnakeSegmentsObstacles = new int[room.tilemapUpperBounds.x - room.tilemapLowerBounds.x + 1,
                room.tilemapUpperBounds.y - room.tilemapLowerBounds.y + 1];
        }

        private void InitializeSegmentsArray()
        {
            // test code
            for (int x = 0; x < (room.tilemapUpperBounds.x - room.tilemapLowerBounds.x + 1); x++)
            {
                for (int y = 0; y < (room.tilemapUpperBounds.y - room.tilemapLowerBounds.y + 1); y++)
                {
                    // Set the default penalty for the grid squares, with higher penalty the enemies will avoid this grid squares
                    aStarSnakeSegmentsObstacles[x, y] = Settings.defaultAStarMovementPenalty;
                }
            }
        }

        // test code
        public void UpdateSnakeSegmenstObstacles()
        {
            // Test code
            InitializeSegmentsArray();

            foreach (SnakeBody snakeBody in GameManager.Instance.GetSnake().SnakeBodyList)
            {
                Vector3Int minColliderBounds = grid.WorldToCell(snakeBody.boxCollider2D.bounds.min);
                Vector3Int maxColliderBounds = grid.WorldToCell(snakeBody.boxCollider2D.bounds.max);

                // Loop through and add the snake body collider bounds to the snake segments array
                for (int i = minColliderBounds.x; i <= maxColliderBounds.x; i++)
                {
                    for (int j = minColliderBounds.y; j <= maxColliderBounds.y; j++)
                    {
                        aStarSnakeSegmentsObstacles[i - room.tilemapLowerBounds.x, j - room.tilemapLowerBounds.y] = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the array of moveable obstacles
        /// </summary>
        public void UpdateMoveableObstacles()
        {
            InitializeObstaclesArray();

            foreach (MoveableObstacle moveable in moveableItemsList)
            {
                Vector3Int minColliderBounds = grid.WorldToCell(moveable.boxCollider2D.bounds.min);
                Vector3Int maxColliderBounds = grid.WorldToCell(moveable.boxCollider2D.bounds.max);

                //Loop through and add moveable item colliders bounds to the obstacle array
                for (int i = minColliderBounds.x; i <= maxColliderBounds.x; i++)
                {
                    for (int j = minColliderBounds.y; j <= maxColliderBounds.y; j++)
                    {
                        aStarItemObstacles[i - room.tilemapLowerBounds.x, j - room.tilemapLowerBounds.y] = 0;
                    }
                }
            }
        }

        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValue(this, nameof(environmentGameObject), environmentGameObject);
        }
#endif
        #endregion
    }
}