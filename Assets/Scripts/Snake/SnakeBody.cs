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

        private void Awake()
        {
            m_Snake = GameManager.Instance.GetSnake();
            spriteRenderer = GetComponent<SpriteRenderer>();
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