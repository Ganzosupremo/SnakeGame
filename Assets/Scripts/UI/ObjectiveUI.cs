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
        
        private CanvasGroup _CanvasGroup;
        private CancellationTokenSource m_CancellationToken;
        private void Awake()
        {
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
            m_CancellationToken.Cancel();
            StaticEventHandler.OnDisplayObjectives -= StaticEventHandler_OnDisplayObjectivesEvent;
            StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
        }

        private void OnDestroy()
        {
            m_CancellationToken.Dispose();
        }

        private void StaticEventHandler_OnDisplayObjectivesEvent(DisplayObjectivesUIArgs args)
        {
            Run(args.CurrentAlpha, args.TargetAlpha, args.DisplayTime, destroyCancellationToken, args.DisplayTexts);
        }

        private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs args)
        {
            if (GameManager.CurrentGameState == GameState.Playing)
            {
                Run(0f, 1f, Settings.DisplayObjectivesTime, m_CancellationToken.Token,$"Go to the next Room and Defeat the Enemies.");
            }
        }

        private async void Run(float currentAlpha, float targetAlpha, float displayTime, CancellationToken cancellationToken = default, params string[] texts)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            try
            {
                _ObjectiveBackground.SetActive(true);
                BuildText(texts);

                await ControlUIFadingAsync(currentAlpha, targetAlpha, displayTime, cancellationToken);

                await UniTask.Delay((int)displayTime * 1000, false, PlayerLoopTiming.Update, cancellationToken);

                await ControlUIFadingAsync(targetAlpha, currentAlpha, displayTime, cancellationToken);

                BuildText(string.Empty);
                _ObjectiveBackground.SetActive(false);
            }
            catch (OperationCanceledException)
            {
                return;
            }
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
                await UniTask.NextFrame(cancellationToken);
            }

            await UniTask.Delay((int)displayTime * 1000, false, PlayerLoopTiming.Update, cancellationToken);

            await HideUI(displayTime, cancellationToken);
        }

        private async UniTask ControlUIFadingAsync(float currentAlpha, float targetAlpha, float displayTime, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested || !gameObject.activeSelf) return;

            float timer = 0f;
            // Control the fade of the canvas group
            while (timer <= displayTime)
            {
                timer += Time.deltaTime;
                _CanvasGroup.alpha = Mathf.Lerp(currentAlpha, targetAlpha, timer / 1.2f);
                await UniTask.NextFrame(cancellationToken);
            }
            await UniTask.NextFrame(cancellationToken);
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
                await UniTask.NextFrame(cancellationToken);
            }
            await UniTask.NextFrame(cancellationToken);
        }

        private void BuildText(params string[] displayTexts)
        {
            StringBuilder builder = new();
            for (int i = 0; i < displayTexts.Length; i++)
            {
                builder.Append(displayTexts[i]);
            }
            _ObjectiveText.text = builder.ToString();
        }
    }
}
