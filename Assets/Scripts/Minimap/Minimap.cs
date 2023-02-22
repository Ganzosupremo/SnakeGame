using UnityEngine;
using Cinemachine;

namespace SnakeGame.Minimap
{
    [DisallowMultipleComponent]
    public class Minimap : MonoBehaviour
    {
        [Tooltip("Populate with the MinimapSnake gameobject")]
        [SerializeField] private GameObject minimapSnake;

        private Transform snakePosition;

        private void Start()
        {
            snakePosition = GameManager.Instance.GetSnake().transform;

            // Use the snake icon for the cinemachine target
            CinemachineVirtualCamera virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            virtualCamera.Follow = snakePosition;

            // Set the snake minimap icon
            SpriteRenderer spriteRenderer = minimapSnake.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                spriteRenderer.sprite = GameManager.Instance.GetMinimapIcon();
        }

        private void Update()
        {
            // Make the minimap icon follow the snake
            if (snakePosition != null && minimapSnake != null)
                minimapSnake.transform.position = snakePosition.position;
        }

        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValue(this, nameof(minimapSnake), minimapSnake);
        }
#endif
        #endregion
    }
}