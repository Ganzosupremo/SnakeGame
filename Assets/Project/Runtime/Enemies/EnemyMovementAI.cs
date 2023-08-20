using Cysharp.Threading.Tasks;
using SnakeGame.AStarPathfinding;
using SnakeGame.Debuging;
using SnakeGame.GameUtilities;
using SnakeGame.ProceduralGenerationSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SnakeGame.Enemies
{
    [RequireComponent(typeof(Enemy))]
    [DisallowMultipleComponent]
    public class EnemyMovementAI : MonoBehaviour
    {
        //public struct EnemyMovementData
        //{
        //    public void MoveEnemy()
        //    {

        //    }
        //}

        //// test code
        //[SerializeField] private float sightRange = 2f;
        //[SerializeField] private LayerMask _EnemySightMask;
        private Vector3 _AmmoDirection;

        private Enemy enemy;
        private Stack<Vector3> movementSteps = new();
        private Vector3 playerReferencePosition;

        private Coroutine moveEnemyRoutine;
        private Coroutine _dashCoroutine;
        private float currentEnemyPathRebuildCooldown;

        private WaitForFixedUpdate fixedUpdateWait;
        private bool shouldChasePlayer = false;


        //private bool _isDashing = false;
        //private float _dashCooldownTimer = 0f;
        private CancellationTokenSource _tokenSource = new();
        private Vector3 _nextPos;


        private List<Vector2Int> surroundingsPositionsList = new();

        [HideInInspector] public int updateFramesNumber = 1;
        [HideInInspector] public float enemySpeed;

        private void Awake()
        {
            enemy = GetComponent<Enemy>();
        }

        private void Start()
        {
            fixedUpdateWait = new WaitForFixedUpdate();
            enemySpeed = enemy.enemyDetails.MovementDetails.GetMoveSpeed();
            playerReferencePosition = GameManager.Instance.GetSnake().GetSnakePosition();
        }

        private void Update()
        {
            UpdateEnemyPosition();

            //await UpdateEnemyPositionAsync(_tokenSource.Token);
        }

        private void OnDisable()
        {
            _tokenSource.Cancel();
        }

        private void OnDestroy()
        {
            _tokenSource.Dispose();
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.collider.CompareTag(Settings.CollisionTilemapTag))
            {
                if (_dashCoroutine != null)
                    StopCoroutine(_dashCoroutine);
            }
        }


        private async UniTask UpdateEnemyPositionAsync(CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            // Movement cooldown timer
            currentEnemyPathRebuildCooldown -= Time.deltaTime;

            // Check the distance to the player to determine if the enemy should start chasing after him
            if (!shouldChasePlayer && Vector3.Distance(transform.position, GameManager.Instance.GetSnake().GetSnakePosition()) < enemy.enemyDetails.enemyChaseDistance)
                shouldChasePlayer = true;

            if (!shouldChasePlayer) return;

            // If the cooldown timer is zero or the player has moved more than the required distance, then
            // rebuild the path and move the enemy
            if (currentEnemyPathRebuildCooldown <= 0f || (Vector3.Distance(playerReferencePosition, GameManager.Instance.GetSnake().GetSnakePosition()) >
                Settings.playerMoveDistanceToRebuildPath))
            {
                // Reset the path rebuild timer
                currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldown;

                // Reset the player referenced position
                playerReferencePosition = GameManager.Instance.GetSnake().GetSnakePosition();

                // Move the enemy using AStar - Triggers the rebuilding of the path
                await CreatePathAsync(token);

                // If a path has been built, move the enemy
                if (movementSteps != null)
                {
                    if (moveEnemyRoutine != null)
                    {
                        enemy.idleEvent.CallIdleEvent();
                        StopCoroutine(moveEnemyRoutine);
                    }
                    //Move the enemy along the path using a coroutine
                    moveEnemyRoutine = StartCoroutine(MoveEnemyCoroutine(movementSteps));
                }
            }
        }
        /// <summary>
        /// Moves the enemy, uses the AStar Pathfinding to build a path to the player.
        /// Coroutine method.
        /// </summary>
        private void UpdateEnemyPosition()
        {
            // Movement cooldown timer
            currentEnemyPathRebuildCooldown -= Time.deltaTime;

            // Check the distance to the player to determine if the enemy should start chasing after him
            if (!shouldChasePlayer && Vector3.Distance(transform.position, GameManager.Instance.GetSnake().GetSnakePosition()) < enemy.enemyDetails.enemyChaseDistance)
                shouldChasePlayer = true;

            if (!shouldChasePlayer) return;

            // Only process A Star rebuild on certain frames to not overload the CPU usage
            if (Time.frameCount % Settings.targetFramesToSpreadPathfindingOver != updateFramesNumber) return;

            // If the cooldown timer is zero or the player has move more than the required distance, then
            // rebuild the path and move the enemy
            if (currentEnemyPathRebuildCooldown <= 0f || (Vector3.Distance(playerReferencePosition, GameManager.Instance.GetSnake().GetSnakePosition()) >
                Settings.playerMoveDistanceToRebuildPath))
            {
                // Reset the path rebuild timer
                currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldown;

                // Reset the player referenced position
                playerReferencePosition = GameManager.Instance.GetSnake().GetSnakePosition();

                //Move the enemy using AStar - Triggers the rebuilding of the path
                CreatePath();

                //If a path has been built, move the enemy
                if (movementSteps != null)
                {
                    //await MoveEnemyAsync(movementSteps, _tokenSource.Token);

                    if (moveEnemyRoutine != null)
                    {
                        enemy.idleEvent.CallIdleEvent();
                        StopCoroutine(moveEnemyRoutine);
                    }
                    //Move the enemy along the path using a coroutine
                    moveEnemyRoutine = StartCoroutine(MoveEnemyCoroutine(movementSteps));
                }
            }
        }

        private void CreatePath()
        {
            Room currentRoom = GameManager.Instance.GetCurrentRoom();
            Grid grid = currentRoom.InstantiatedRoom.grid;

            // Gets the player position on the grid
            Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);

            // Gets the enemy position on the grid
            Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

            // Build a path for the enemy to move
            movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

            // Take off the first step on path - this is the square the enemy is already on
            if (movementSteps != null)
                movementSteps.Pop();
            else // If theres no path, go idle
                enemy.idleEvent.CallIdleEvent();
        }

        private async UniTask CreatePathAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            try
            {
                Room currentRoom = GameManager.Instance.GetCurrentRoom();
                Grid grid = currentRoom.InstantiatedRoom.grid;

                // Gets the player position on the grid
                Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);

                // Gets the enemy position on the grid
                Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

                // Build a path for the enemy to move
                movementSteps = await AStar.BuildPathAsync(currentRoom, enemyGridPosition, playerGridPosition, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        private IEnumerator MoveEnemyCoroutine(Stack<Vector3> movementSteps)
        {
            while (movementSteps.Count > 0)
            {
                _nextPos = movementSteps.Pop();

                // While not very close continue moving, too close move to the next point
                while (Vector3.Distance(_nextPos, transform.position) > 0.3f)
                {
                    //Call the movement event
                    enemy.movementToPositionEvent.CallMovementToPosition(_nextPos, transform.position, (_nextPos - transform.position).normalized, enemySpeed);
                    //RotateBodyWithAngle(_nextPos);

                    yield return fixedUpdateWait; // The enemy moves using the 2D physics , so wait for the fixed update.
                }

                yield return fixedUpdateWait;
            }

            // End of path steps - trigger the enemy idle event
            enemy.idleEvent.CallIdleEvent();
        }

        /// <summary>
        /// Set the frame on which the enemy pathfinding will be recalculated - to avoid spikes in the CPU usage
        /// </summary>
        public void UpdateFramesNumber(int updatedFrameNumber)
        {
            this.updateFramesNumber = updatedFrameNumber;
        }

        /// <summary>
        /// This method is to retrieve a position that is not marked as an obstacle where the
        /// enemy can't go. The player can ocupy various tiles at a given time, that's why we use this method.
        /// </summary>
        /// <param name="currentRoom"></param>
        /// <returns>Returns the grip position of the player that is not an obstacle</returns>
        private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
        {
            Vector3 playerPosition = GameManager.Instance.GetSnake().GetSnakePosition();

            Vector3Int playerCellPosition = currentRoom.InstantiatedRoom.grid.WorldToCell(playerPosition);

            Vector2Int adjustedPlayerCellPosition = new Vector2Int(playerCellPosition.x - currentRoom.tilemapLowerBounds.x,
                playerCellPosition.y - currentRoom.tilemapLowerBounds.y);

            int obstacle = Mathf.Min(currentRoom.InstantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x, adjustedPlayerCellPosition.y],
                        currentRoom.InstantiatedRoom.aStarMoveableObstacles[adjustedPlayerCellPosition.x, adjustedPlayerCellPosition.y]);

            // if the player is not on an obstacle, then return the player position
            if (obstacle != 0)
            {
                return playerCellPosition;
            }
            // find the nearest cell that is not an obstacle, whether thats a collision tile or a moveable object
            else
            {
                surroundingsPositionsList.Clear();

                // Populate the surrounding position list - this will hold the 8 posible vector locations of a (0,0) grid square
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (j == 0 && i == 0) continue;

                        surroundingsPositionsList.Add(new Vector2Int(i, j));
                    }
                }

                // Loop on all positions
                for (int l = 0; l < 8; l++)
                {
                    // Generate an index for the list
                    int index = Random.Range(0, surroundingsPositionsList.Count);

                    // See of there is an obstacle in the selected surrounded position
                    try
                    {
                        obstacle = Mathf.Min(currentRoom.InstantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x + surroundingsPositionsList[index].x,
                            adjustedPlayerCellPosition.y + surroundingsPositionsList[index].y], currentRoom.InstantiatedRoom.aStarMoveableObstacles[adjustedPlayerCellPosition.x
                            + surroundingsPositionsList[index].x, adjustedPlayerCellPosition.y + surroundingsPositionsList[index].y]);

                        if (obstacle != 0)
                        {
                            return new Vector3Int(playerCellPosition.x + surroundingsPositionsList[index].x,
                                playerCellPosition.y + surroundingsPositionsList[index].y, 0);
                        }
                    }
                    // Catch errors where the surrounded position is outside the grid
                    catch (Exception e)
                    {
                        this.LogError("Outside the room bounds... Calculating new value. More Info: " + e.Message);
                    }

                    surroundingsPositionsList.RemoveAt(index);
                }

                //No non-obstacle tiles surronding the player - send the enemy in the direction of an enemy spawn position transform
                return (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];
            }
        }

        private void RotateBodyWithAngle(Vector3 position)
        {
            float angle = HelperUtilities.GetAngleFromVector(position);
            transform.eulerAngles = new(0f, 0f, angle);
        }
    }

    //[BurstCompile]
    //public struct EnemyParallelMovement : IJobParallelFor
    //{
    //    public NativeArray<EnemyMovementData> MovementDataArray;

    //    public void Execute(int index)
    //    {
    //        var data = MovementDataArray[index];
    //        data.MoveEnemy();
    //        MovementDataArray[index] = data;
    //    }
    //}
}