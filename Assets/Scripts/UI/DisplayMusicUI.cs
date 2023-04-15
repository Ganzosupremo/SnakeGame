using Cysharp.Threading.Tasks;
using SnakeGame.AudioSystem;
using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace SnakeGame.UI
{
    public class DisplayMusicUI : MonoBehaviour
    {
        [SerializeField] private GameObject _MusicBackground;
        [SerializeField] private TextMeshProUGUI _MusicNameText;
        [Range(0.1f, 4f)]
        [SerializeField] private float TimeToTween;
        [Range(0.5f, 5f)]
        [SerializeField] private float DelayTime;

        private CancellationTokenSource m_CancellationSource;
        private AudioClip m_CurrentMusicClip;

        private void Start()
        {
            m_CancellationSource = new CancellationTokenSource();
            _MusicBackground.SetActive(false);
        }

        private void OnEnable()
        {
            MusicManager.OnMusicClipChanged += MusicManager_OnMusicClipChanged;
        }

        private void OnDisable()
        {
            m_CancellationSource.Cancel();
            MusicManager.OnMusicClipChanged -= MusicManager_OnMusicClipChanged;
        }

        private void MusicManager_OnMusicClipChanged(MusicSO musicSO, float delay)
        {
            ChangeMusicNameText(musicSO, m_CancellationSource.Token);
        }

        private async void ChangeMusicNameText(MusicSO musicSO, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            if (musicSO.musicClip != m_CurrentMusicClip)
            {
                m_CurrentMusicClip = musicSO.musicClip;

                _MusicBackground.SetActive(true);
                _MusicNameText.text = musicSO.musicName;

                LeanTween.moveX(_MusicBackground.GetComponent<RectTransform>(), 150f, TimeToTween).setEase(LeanTweenType.easeOutBack);

                await UniTask.Delay((int)DelayTime * 1000);

                LeanTween.moveX(_MusicBackground.GetComponent<RectTransform>(), 0f, TimeToTween).setEase(LeanTweenType.easeOutBack).setOnComplete(DisableBackground);
            }
            else
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private async UniTask MoveObjectX(float delay, MusicSO musicSO)
        {
            float moveToTarget = 0f;
            _MusicNameText.text = musicSO.musicName;

            while (moveToTarget <= delay)
            {
                Vector3 newPosition = new(moveToTarget, 0f, 0f);
                await UniTask.WaitForEndOfFrame(this);
                _MusicBackground.transform.localPosition = newPosition;
                moveToTarget += TimeToTween * 30f * Time.deltaTime;
            }
        }

        private void DisableBackground()
        {
            _MusicBackground.SetActive(false);
            _MusicNameText.text = "";
        }
    }
}
