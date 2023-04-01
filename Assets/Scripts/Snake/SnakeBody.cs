using UnityEngine;

namespace SnakeGame.PlayerSystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SnakeBody : MonoBehaviour
    {
        public Sprite[] bodySprites;

        private SpriteRenderer spriteRenderer;
        private Vector3 m_NextPosition = Vector3.zero;
        private int m_WaitPreviousPart;
        private Snake m_Snake;

        public BoxCollider2D boxCollider2D;
        [HideInInspector] public Bounds segmentBounds;

        private void Awake()
        {
            m_Snake = GameManager.Instance.GetSnake();
            spriteRenderer = GetComponent<SpriteRenderer>();
            boxCollider2D = GetComponentInChildren<BoxCollider2D>();
            segmentBounds = boxCollider2D.bounds;
        }

        private void OnEnable()
        {
            GameManager.OnLevelChanged += GameManager_OnLevelChanged;
        }

        private void OnDisable()
        {
            GameManager.OnLevelChanged -= GameManager_OnLevelChanged;
        }

        private void GameManager_OnLevelChanged(int index)
        {
            // Send all snake segments to the head
            transform.position = m_Snake.transform.position;
        }

        private void Start()
        {
            m_NextPosition = transform.position;

            if (bodySprites.Length != 0)
            {
                // Set a random sprite for diferent parts of the snake body
                int randomSprite = Random.Range(0, bodySprites.Length);
                spriteRenderer.sprite = bodySprites[randomSprite];
            }
        }

        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, m_NextPosition, m_Snake.GetSnakeControler().GetMoveSpeed() * Time.deltaTime);
        }

        public void SetTargetPosition(Vector3 position)
        {
            if (m_WaitPreviousPart > 0)
            {
                m_WaitPreviousPart--;
                return;
            }
            m_NextPosition = position;
        }

        public void WaitHeadUpdateCycle(int value)
        {
            m_WaitPreviousPart = value;
        }
    }
}