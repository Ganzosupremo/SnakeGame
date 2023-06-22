using Cysharp.Threading.Tasks;
using SnakeGame.AStarPathfinding;
using SnakeGame.Debuging;
using SnakeGame.ProceduralGenerationSystem;
using SnakeGame.VisualEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SnakeGame.Decorations
{
    public class MoveableDecoration : MonoBehaviour
    {
        [SerializeField] private float m_MoveSpeed = 3.5f;

        private Stack<Vector3> m_MovementSteps;
        private int m_FramesUpdate = 1;
        private Coroutine m_MoveCoroutine;
        private WaitForFixedUpdate m_WaitForFixedUpdate;
        private MovementToPositionEvent m_MovementToPositionEvent;
        private Room m_CurrentRoom;
        private Animator m_Animator;
        private MaterializeEffect m_MaterializeEffect;
        private CancellationTokenSource m_CancellationTokenSource;

        private static int Idle = Animator.StringToHash("idle");

        private void Awake()
        {
            m_MovementToPositionEvent = GetComponent<MovementToPositionEvent>();
            m_Animator = GetComponent<Animator>();
            m_MovementSteps = new Stack<Vector3>();
            m_MaterializeEffect = GetComponent<MaterializeEffect>();
            m_CancellationTokenSource = new CancellationTokenSource();
        }

        private async void Start()
        {
            m_WaitForFixedUpdate = new WaitForFixedUpdate();
            m_MovementSteps = new();
            m_CurrentRoom = GameManager.Instance.GetCurrentRoom();
            m_Animator.SetBool(Idle, false);

            await MaterializeDecoration();
        }

        private void Update()
        {
            Move();
        }

        private void OnDestroy()
        {
            m_CancellationTokenSource.Cancel();
            m_CancellationTokenSource.Dispose();
        }

        private async UniTask MaterializeDecoration()
        {
            EnableDecoration(false);

            await m_MaterializeEffect.MaterializeAsync(GameResources.Instance.MaterializeShader, new Color(54, 65, 89), 1.5f, GameResources.Instance.litMaterial, GetComponent<SpriteRenderer>());

            EnableDecoration(true);
        }

        private void EnableDecoration(bool enabled)
        {
            this.enabled = enabled;
            m_Animator.enabled = enabled;
            m_MovementToPositionEvent.enabled = enabled;
        }

        private async void Move()
        {
            if (Time.frameCount % Settings.targetFramesToSpreadPathfindingOver != m_FramesUpdate) return;

            CreatePath();

            try
            {
                await MoveDecorationAsync(m_MovementSteps, m_CancellationTokenSource.Token);
            }
            catch (NullReferenceException)
            {
                m_Animator.SetBool(Idle, true);
            }
            catch (OperationCanceledException)
            {
                Debuger.Log("Object was Destroyed");
            }
        }

        private IEnumerator MoveDecorationRoutine(Stack<Vector3> movementSteps)
        {
            m_Animator.SetBool(Idle, false);
            while (movementSteps.Count > 0)
            {
                Vector3 nextPos = movementSteps.Pop();

                while (Vector3.Distance(nextPos, transform.position) > 0.5f)
                {
                    m_MovementToPositionEvent.CallMovementToPosition(nextPos, transform.position, (nextPos - transform.position).normalized, m_MoveSpeed);

                    yield return m_WaitForFixedUpdate;
                }

                yield return m_WaitForFixedUpdate;
            }
        }

        private async UniTask MoveDecorationAsync(Stack<Vector3> movementSteps, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            m_Animator.SetBool(Idle, false);
            while (movementSteps.Count > 0)
            {
                Vector3 nextPos = movementSteps.Pop();
                while (Vector3.Distance(nextPos, transform.position) > 0.3f)
                {
                    m_MovementToPositionEvent.CallMovementToPosition(nextPos, transform.position, (nextPos - transform.position), m_MoveSpeed);
                    await UniTask.WaitForFixedUpdate(cancellationToken);
                }
                await UniTask.WaitForFixedUpdate(cancellationToken);
            }
        }

        private void CreatePath()
        {
            if (m_CurrentRoom != GameManager.Instance.GetCurrentRoom())
                return;

            Grid grid = m_CurrentRoom.InstantiatedRoom.grid;

            // Gets the target position on the grid
            Vector3Int targetGridPosition = GetNearestPosition(m_CurrentRoom);

            // Gets this objects position on the grid
            Vector3Int objectGridPosition = grid.WorldToCell(transform.position);

            try
            {
                // Build a path
                m_MovementSteps = AStar.BuildPath(m_CurrentRoom, objectGridPosition, targetGridPosition);
                // Take off the first step on path
                m_MovementSteps?.Pop();
            }
            catch (NullReferenceException)
            {
                this.LogWarning("No path could be Built");
                return;
            }
        }

        private Vector3Int GetNearestPosition(Room currentRoom)
        {
            return (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];
        }
    }
}
