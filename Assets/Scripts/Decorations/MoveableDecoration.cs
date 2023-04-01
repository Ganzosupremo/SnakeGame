using SnakeGame.AStarPathfinding;
using SnakeGame.ProceduralGenerationSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame
{
    public class MoveableDecoration : MonoBehaviour
    {
        private Stack<Vector3> movementSteps;
        [SerializeField] private float moveSpeed = 3.5f;
        private int framesUpdate = 1;
        private Coroutine moveCoroutine;
        private WaitForFixedUpdate waitForFixedUpdate;
        private MovementToPositionEvent movementToPositionEvent;

        private void Awake()
        {
            movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        }

        private void Start()
        {
            waitForFixedUpdate= new WaitForFixedUpdate();
            movementSteps = new();
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            if (Time.frameCount % Settings.targetFramesToSpreadPathfindingOver != framesUpdate) return;

            CreatePath();

            if (movementSteps != null)
            {
                if (moveCoroutine != null)
                    StopCoroutine(moveCoroutine);

                moveCoroutine = StartCoroutine(MoveDecorationRoutine(movementSteps));
            }
        }

        private IEnumerator MoveDecorationRoutine(Stack<Vector3> movementSteps)
        {
            while (movementSteps.Count > 0)
            {
                Vector3 nextPos = movementSteps.Pop();

                while (Vector3.Distance(nextPos, transform.position) > 0.3f)
                {
                    movementToPositionEvent.CallMovementToPosition(nextPos, transform.position, (nextPos - transform.position).normalized, moveSpeed);

                    yield return waitForFixedUpdate;
                }

                yield return waitForFixedUpdate;
            }
        }

        private void CreatePath()
        {
            Room currentRoom = GameManager.Instance.GetCurrentRoom();
            Grid grid = currentRoom.instantiatedRoom.grid;

            // Gets the target position on the grid
            Vector3Int targetGridPosition = GetNearestNonObstaclePosition(currentRoom);

            // Gets this objects position on the grid
            Vector3Int objectGridPosition = grid.WorldToCell(transform.position);

            // Build a path for the enemy to move
            movementSteps = AStar.BuildPath(currentRoom, objectGridPosition, targetGridPosition);

            // Take off the first step on path
            movementSteps?.Pop();
        }

        private Vector3Int GetNearestNonObstaclePosition(Room currentRoom)
        {
            return (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];
        }
    }
}
