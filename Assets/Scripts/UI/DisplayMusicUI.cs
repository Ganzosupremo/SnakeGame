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
        public static bool CanDisplay { get; set; } = false;

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
            CanDisplay = false;
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

        private void MusicManager_OnMusicClipChanged(MusicClipChangeEventArgs args)
        {
            ChangeMusicNameText(args, m_CancellationSource.Token);
        }

        private async void ChangeMusicNameText(MusicClipChangeEventArgs args, CancellationToken token)
        {
            if (args.CanPlayMultipleClips)
            {
                _MusicBackground.SetActive(true);
                _MusicNameText.text = args.NameRandomClip;
                LeanTween.moveX(_MusicBackground.GetComponent<RectTransform>(), 150f, TimeToTween).setEase(LeanTweenType.easeInOutCubic);

                await UniTask.Delay((int)DelayTime * 1000);

                LeanTween.moveX(_MusicBackground.GetComponent<RectTransform>(), 0f, TimeToTween).setEase(LeanTweenType.easeInOutCubic).setOnComplete(DisableBackground);
                CanDisplay = true;
            }
            else
            {
                _MusicBackground.SetActive(true);
                _MusicNameText.text = args.Music.musicName;

                LeanTween.moveX(_MusicBackground.GetComponent<RectTransform>(), 150f, TimeToTween).setEase(LeanTweenType.easeInOutCubic);

                await UniTask.Delay((int)DelayTime * 1000);

                LeanTween.moveX(_MusicBackground.GetComponent<RectTransform>(), 0f, TimeToTween).setEase(LeanTweenType.easeInOutCubic).setOnComplete(DisableBackground);
                CanDisplay = true;
            }

            await UniTask.NextFrame();
        }

        private void DisableBackground()
        {
            _MusicNameText.text = "";
            _MusicBackground.SetActive(false);
        }
    }
}
