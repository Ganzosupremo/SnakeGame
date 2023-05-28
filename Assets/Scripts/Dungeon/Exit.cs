using SnakeGame.VisualEffects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SnakeGame.ProceduralGenerationSystem
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

        private void OnEnable()
        {
            StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
            GameManager.OnLevelCompleted += GameManager_OnLevelChanged;
        }

        private void OnDisable()
        {
            StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
            GameManager.OnLevelCompleted -= GameManager_OnLevelChanged;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Settings.PlayerTag))
            {
                if (m_gameLevelIndex < GameManager.Instance.LevelCount)
                {
                    if (GameManager.CurrentGameState == GameState.Playing)
                    {
                        GameManager.Instance.PlayNextLevel(m_gameLevelIndex);
                    }
                    else if (GameManager.CurrentGameState == GameState.GameWon)
                    {
                        SceneManager.LoadScene((int)SceneIndex.GameWon);
                    }
                }
            }
        }

        private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
        {
            MaterializeExit();
        }

        private void GameManager_OnLevelChanged(int dungeonIndex)
        {
            m_gameLevelIndex = dungeonIndex;
        }

        private async void MaterializeExit()
        {
            EnableExit(false);

            await m_MaterializeEffect.MaterializeAsync
                (materializeShader,materializeColor,materializeTime, defaultLitMaterial, m_SpriteRenderer);

            EnableExit(true);
        }

        private void EnableExit(bool isEnabled)
        {
            m_Collider.enabled = isEnabled;
        }
    }
}
