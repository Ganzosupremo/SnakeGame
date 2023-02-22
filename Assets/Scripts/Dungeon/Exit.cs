using System.Collections;
using UnityEngine;
using SnakeGame.VisualEffects;

namespace SnakeGame.Dungeon
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(MaterializeEffect))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Exit : MonoBehaviour
    {
        private BoxCollider2D m_Collider;
        private MaterializeEffect m_MaterializeEffect;
        private SpriteRenderer m_SpriteRenderer;
        
        [SerializeField] private int m_gameLevelIndex;

        [Header("Materialize Settings")]
        [SerializeField] private Shader materializeShader;

        [ColorUsage(true, true)]
        [SerializeField] private Color materializeColor;

        [SerializeField] private float materializeTime = 3f;

        [SerializeField] private Material defaultLitMaterial;

        private void Awake()
        {
            m_Collider = GetComponent<BoxCollider2D>();
            m_MaterializeEffect = GetComponent<MaterializeEffect>();
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            Listen();
        }

        private void Listen()
        {
            m_gameLevelIndex = GameManager.Instance.LevelIndex;
        }

        private void OnEnable()
        {
            StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
        }

        private void OnDisable()
        {
            StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Settings.playerTag))
            {
                if (m_gameLevelIndex < GameManager.Instance.LevelCount)
                {
                    if (GameManager.Instance.currentGameState == GameState.Playing)
                    {
                        //m_gameLevelIndex++;
                        GameManager.Instance.PlayNextLevel(m_gameLevelIndex);
                    }
                }
            }
        }

        private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
        {
            Debug.Log(m_gameLevelIndex);
            StartCoroutine(MaterializeExit());
        }

        private IEnumerator MaterializeExit()
        {
            EnableExit(false);

            yield return StartCoroutine(m_MaterializeEffect.MaterializeRoutine
                (materializeShader,materializeColor,materializeTime, defaultLitMaterial, m_SpriteRenderer));

            EnableExit(true);
        }

        private void EnableExit(bool isEnabled)
        {
            m_Collider.enabled = isEnabled;
        }
    }
}
