using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace SnakeGame.VisualEffects
{
    public class LowHealthEffectUI : MonoBehaviour
    {
        private CanvasGroup _CanvasGroup;
        private CancellationTokenSource _CancellationTokenSource;

        private void Awake()
        {
            _CanvasGroup = GetComponent<CanvasGroup>();
            _CancellationTokenSource = new();
        }

        private void OnEnable()
        {
            _CanvasGroup.alpha = 0f;
            GameManager.Instance.GetSnake().healthEvent.OnHealthChanged += OnSnakeHealthChanged;
        }

        private void OnDisable()
        {
            _CancellationTokenSource.Cancel();
            GameManager.Instance.GetSnake().healthEvent.OnHealthChanged -= OnSnakeHealthChanged;
        }

        private void OnDestroy()
        {
            _CancellationTokenSource.Dispose();
        }

        private async void OnSnakeHealthChanged(HealthSystem.HealthEvent healthEvent, HealthSystem.HealthEventArgs args)
        {
            CancellationToken token = _CancellationTokenSource.Token;

            if (args.healthPercent > .25f && args.healthPercent < .35f)
                await FadeCanvasGroup(_CanvasGroup.alpha, 0.45f, 1f, token);
            else if (args.healthPercent > .15f && args.healthPercent < .25f)
                await FadeCanvasGroup(_CanvasGroup.alpha, 0.69f, 1f, token);
            else if (args.healthPercent > .05f && args.healthPercent < .15f)
                await FadeCanvasGroup(_CanvasGroup.alpha, 0.8f, 1f, token);
            //else if (args.healthPercent <= 0f)
            //    await FadeCanvasGroup(_CanvasGroup.alpha, 0f, 1f, token);
            else
                await FadeCanvasGroup(_CanvasGroup.alpha, 0f, .1f, token);
        }

        private async UniTask FadeCanvasGroup(float currentAlpha, float targetAlpha, float time, CancellationToken token)
        {
            if (token.IsCancellationRequested || !gameObject.activeSelf) return;

            float timer = 0f;
            while (timer <= time)
            {
                timer += Time.deltaTime;
                _CanvasGroup.alpha = Mathf.Lerp(currentAlpha, targetAlpha, timer / time);
                await UniTask.NextFrame(token);
            }
            await UniTask.NextFrame(token);
        }
    }
}
