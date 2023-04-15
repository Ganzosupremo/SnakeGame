using SnakeGame.AStarPathfinding;
using SnakeGame.Debuging;
using SnakeGame.ProceduralGenerationSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame.Decorations
{
    public class MoveableDecoration : MonoBehaviour
    {
        private Stack<Vector3> m_MovementSteps;
        [SerializeField] private float m_MoveSpeed = 3.5f;
        private int m_FramesUpdate = 1;
        private Coroutine m_MoveCoroutine;
        private WaitForFixedUpdate m_WaitForFixedUpdate;
        private MovementToPositionEvent m_MovementToPositionEvent;
        private Room m_CurrentRoom;
        private Animator m_Animator;

        private static int Idle = Animator.StringToHash("idle");

        private void Awake()
        {
            m_MovementToPositionEvent = GetComponent<MovementToPositionEvent>();
            m_Animator = GetComponent<Animator>();
        }

        private void Start()
        {
            m_WaitForFixedUpdate = new WaitForFixedUpdate();
            m_MovementSteps = new();
            m_CurrentRoom = GameManager.Instance.GetCurrentRoom();
            m_Animator.SetBool(Idle, false);
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            if (Time.frameCount % Settings.targetFramesToSpreadPathfindingOver != m_FramesUpdate) return;

            CreatePath();

            if (m_MovementSteps != null)
            {
                if (m_MoveCoroutine != null)
                    StopCoroutine(m_MoveCoroutine);
                m_MoveCoroutine = StartCoroutine(MoveDecorationRoutine(m_MovementSteps));
            }
            else
            {
                m_Animator.SetBool(Idle, true);
            }
        }

        private IEnumerator MoveDecorationRoutine(Stack<Vector3> movementSteps)
        {
            m_Animator.SetBool(Idle, false);
            while (movementSteps.Count > 0)
            {
                Vector3 nextPos = movementSteps.Pop();

                while (Vector3.Distance(nextPos, transform.position) > 0.3f)
                {
                    m_MovementToPositionEvent.CallMovementToPosition(nextPos, transform.position, (nextPos - transform.position).normalized, m_MoveSpeed);

                    yield return m_WaitForFixedUpdate;
                }

                yield return m_WaitForFixedUpdate;
            }
        }

        private void CreatePath()
        {
            if (m_CurrentRoom != GameManager.Instance.GetCurrentRoom())
            {
                return;
            }

            Grid grid = m_CurrentRoom.instantiatedRoom.grid;

            // Gets the target position on the grid
            Vector3Int targetGridPosition = GetNearestNonObstaclePosition(m_CurrentRoom);

            // Gets this objects position on the grid
            Vector3Int objectGridPosition = grid.WorldToCell(transform.position);

            try
            {
                // Build a path for the enemy to move
                m_MovementSteps = AStar.BuildPath(m_CurrentRoom, objectGridPosition, targetGridPosition);

                // Take off the first step on path
                m_MovementSteps?.Pop();
            }
            catch (System.Exception e)
            {
                this.LogError(e.Message);
                return;
            }
        }

        private Vector3Int GetNearestNonObstaclePosition(Room currentRoom)
        {
            return (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];
        }
    }
}
