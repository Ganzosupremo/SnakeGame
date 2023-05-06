using Cysharp.Threading.Tasks;
using System;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;

namespace SnakeGame.UI
{
    public class ObjectiveUI : MonoBehaviour
    {
        [SerializeField] private GameObject _ObjectiveBackground;
        [SerializeField] private TextMeshProUGUI _ObjectiveText;
        [SerializeField] private CanvasGroup _CanvasGroup;

        private CancellationTokenSource m_CancellationToken;

        private void Awake()
        {
            if (_CanvasGroup == null)
                _CanvasGroup = GetComponent<CanvasGroup>();

            m_CancellationToken = new();
        }
        private void Start()
        {
            _ObjectiveBackground.SetActive(false);
        }

        private void OnEnable()
        {
            StaticEventHandler.OnDisplayObjectives += StaticEventHandler_OnDisplayObjectivesEvent;
            StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
        }

        private void OnDisable()
        {
            StaticEventHandler.OnDisplayObjectives -= StaticEventHandler_OnDisplayObjectivesEvent;
            StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
        }

        private void StaticEventHandler_OnDisplayObjectivesEvent(DisplayObjectivesUIArgs args)
        {
            DisplayObjectives(args.CurrentAlpha, args.TargetAlpha, args.DisplayTime, destroyCancellationToken, args.DisplayTexts);
        }

        private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs args)
        {
            if (GameManager.CurrentGameState == GameState.Playing)
            {
                DisplayObjectives(0f, 1f, Settings.DisplayObjectivesTime, m_CancellationToken.Token,$"Go to the next Room and Defeat the Enemies.");
            }
        }

        [Obsolete]
        public async UniTask Display(float currentAlpha, float targetAlpha, float displayTime, CancellationToken cancellationToken, params string[] displayTexts)
        {
            _ObjectiveBackground.SetActive(true);
            InitializeText(displayTexts);

            await DisplayObjectivesAsync(currentAlpha, targetAlpha, displayTime, destroyCancellationToken);

            InitializeText("");
            _ObjectiveBackground.SetActive(false);
        }

        private async void DisplayObjectives(float currentAlpha, float targetAlpha, float displayTime, CancellationToken cancellationToken, params string[] displayTexts)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            _ObjectiveBackground.SetActive(true);
            InitializeText(displayTexts);

            await DisplayObjectivesAsync(currentAlpha, targetAlpha, displayTime, cancellationToken);
            
            InitializeText("");
            _ObjectiveBackground.SetActive(false);
        }

        private void InitializeText(params string[] displayTexts)
        {
            StringBuilder builder = new();
            for (int i = 0; i < displayTexts.Length; i++)
            {
                builder.Append(displayTexts[i]);
            }
            _ObjectiveText.text = builder.ToString();
        }

        private async UniTask DisplayObjectivesAsync(float currentAlpha, float targetAlpha, float displayTime, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            float timer = 0f;
            while (timer <= displayTime)
            {
                timer += Time.deltaTime;
                _CanvasGroup.alpha = Mathf.Lerp(currentAlpha, targetAlpha, timer / 1.2f);
                await UniTask.NextFrame();
            }

            await UniTask.Delay((int)displayTime * 1000);

            await HideUI(displayTime, cancellationToken);
        }

        private async UniTask HideUI(float displayTime, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            float hideTimer = 0f;
            while (hideTimer <= displayTime)
            {
                hideTimer += Time.deltaTime;
                _CanvasGroup.alpha = Mathf.Lerp(1f, 0f, hideTimer / 1.2f);
                await UniTask.NextFrame();
            }
        }
    }
}
