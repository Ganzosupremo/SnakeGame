using Cinemachine;
using SnakeGame.GameUtilities;
using UnityEngine;

namespace SnakeGame.Miscelaneous
{
    [RequireComponent(typeof(CinemachineTargetGroup))]
    public class CMTarget : MonoBehaviour
    {
        private CinemachineTargetGroup m_TargetGroup;

        #region Tooltip
        [Tooltip("Populate with the gameobject called MouseTarget")]
        #endregion
        [SerializeField] private Transform cursorTarget;

        private void Awake()
        {
            m_TargetGroup = GetComponent<CinemachineTargetGroup>();
        }

        private void Start()
        {
            SetCinemachineTarget();
        }

        private void OnEnable()
        {
            GameManager.Instance.GetSnake().destroyEvent.OnDestroy += Snake_OnDestroy;
        }

        private void OnDisable()
        {
            GameManager.Instance.GetSnake().destroyEvent.OnDestroy-= Snake_OnDestroy;
        }

        private void Snake_OnDestroy(HealthSystem.DestroyEvent arg1, HealthSystem.DestroyedEventArgs arg2)
        {
            m_TargetGroup.RemoveMember(cursorTarget);
        }

        private void Update()
        {
            cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
        }

        /// <summary>
        /// Set The Cinemachine Camera Target Group.
        /// Creates a target group that will follow both the snake and the mouse.
        /// </summary>
        private void SetCinemachineTarget()
        {
            CinemachineTargetGroup.Target snakeTarget = new()
            {
                weight = 1.5f,
                radius = 2f,
                target = GameManager.Instance.GetSnake().transform
            };

            CinemachineTargetGroup.Target CMCursorTarget = new()
            {
                weight = 1f,
                radius = 1f,
                target = cursorTarget
            };

            CinemachineTargetGroup.Target[] cmTargetArray = new CinemachineTargetGroup.Target[] { snakeTarget, CMCursorTarget };

            m_TargetGroup.m_Targets = cmTargetArray;
        }
    }
}