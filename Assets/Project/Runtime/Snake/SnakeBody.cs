using SnakeGame.GameUtilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SnakeGame.PlayerSystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SnakeBody : MonoBehaviour
    {
        private Sprite[] _bodySprites;
        private SpriteRenderer _spriteRenderer;
        private Vector3 _nextPosition = Vector3.zero;
        private int _waitPreviousPart;
        private Snake _snake;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _snake = GameManager.Instance.GetSnake();
        }

        private void Start()
        {
            _nextPosition = transform.position;
            _bodySprites = _snake.SegmentsArray;

            if (_bodySprites.Length != 0)
            {
                // Set a random sprite for diferent parts of the snake body
                int randomSprite = Random.Range(0, _bodySprites.Length);
                _spriteRenderer.sprite = _bodySprites[randomSprite];
            }
        }

        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, _nextPosition, _snake.GetSnakeControler().GetMoveSpeed() * Time.deltaTime);

            //if (_nextPosition != Vector3.zero)
            //{
            //    transform.position = Vector3.MoveTowards(transform.position, _nextPosition, _snake.GetSnakeControler().GetMoveSpeed() * Time.deltaTime);
            //    if (transform.position == _nextPosition)
            //    {
            //        _nextPosition = Vector3.zero;
            //    }
            //}
        }

        public void SetTargetPosition(Vector3 position)
        {
            if (_waitPreviousPart > 0)
            {
                _waitPreviousPart--;
                return;
            }
            _nextPosition = position;
        }

        public void WaitHeadUpdateCycle(int value)
        {
            _waitPreviousPart = value;
        }

        public void RotateBodyWithAngle(Vector2 direction)
        {
            float angle = HelperUtilities.GetAngleFromVector(direction);
            transform.eulerAngles = new Vector3(0f, 0f, angle);
        }
    }
}