using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace SnakeGame.VisualEffects
{
    public class LowHealthEffectUI : MonoBehaviour
    {
        private CanvasGroup _CanvasGroup;
        private CancellationTokenSource _CancellationTokenSource;
        private bool _IsFading;

        private void Awake()
        {
            _CanvasGroup = GetComponent<CanvasGroup>();
            _CancellationTokenSource = new();
            _IsFading = false;
        }

        private void OnEnable()
        {
            _CanvasGroup.alpha = 0f;
            GameManager.Instance.GetSnake().healthEvent.OnHealthChanged += OnSnakeHealthChanged;
            GameManager.Instance.GetSnake().destroyEvent.OnDestroy += DestroyEvent_OnDestroy;
        }

        private void OnDisable()
        {
            _CancellationTokenSource.Cancel();
            GameManager.Instance.GetSnake().healthEvent.OnHealthChanged -= OnSnakeHealthChanged;
            GameManager.Instance.GetSnake().destroyEvent.OnDestroy -= DestroyEvent_OnDestroy;
        }

        private void OnDestroy()
        {
            _CancellationTokenSource.Dispose();
        }

        private void DestroyEvent_OnDestroy(HealthSystem.DestroyEvent arg1, HealthSystem.DestroyedEventArgs arg2)
        {
            _CancellationTokenSource.Cancel();
            _CanvasGroup.gameObject.SetActive(false);
        }

        private async void OnSnakeHealthChanged(HealthSystem.HealthEvent healthEvent, HealthSystem.HealthEventArgs args)
        {
            CancellationToken token = _CancellationTokenSource.Token;

            // if the health is between 25 and 35 percent start fading in the effect
            if (args.healthPercent > .25f && args.healthPercent < .35f)
                await FadeCanvasGroup(_CanvasGroup.alpha, 0.45f, 1f, token);

            // if the health is between 15 and 25 percent make the effect even more visible
            else if (args.healthPercent > .15f && args.healthPercent < .25f)
                await FadeCanvasGroup(_CanvasGroup.alpha, 0.69f, 1f, token);

            // if the health is between 5 and 15 percent make the effect almost full visible
            else if (args.healthPercent > .05f && args.healthPercent < .15f)
                await FadeCanvasGroup(_CanvasGroup.alpha, 0.8f, 1f, token);
            //else if (args.healthPercent <= 0f)
            //    await FadeCanvasGroup(_CanvasGroup.alpha, 0f, 1f, token);
            
            else if (args.healthPercent > .35f || _IsFading)
            {
                if (_CanvasGroup.alpha > 0f)
                    await FadeCanvasGroup(_CanvasGroup.alpha, 0f, .1f, token);
                _IsFading = false;
            }
        }

        private async UniTask FadeCanvasGroup(float currentAlpha, float targetAlpha, float timeToFade, CancellationToken token)
        {
            if (token.IsCancellationRequested || !gameObject.activeSelf) return;

            _CanvasGroup.gameObject.SetActive(true);
            _IsFading = true;

            float timer = 0f;
            while (timer <= timeToFade)
            {
                timer += Time.deltaTime;
                _CanvasGroup.alpha = Mathf.Lerp(currentAlpha, targetAlpha, timer / timeToFade);
                await UniTask.NextFrame(token);
            }
            _IsFading = false;
        }
    }
}
