using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SnakeGame
{
    public class ModalWindow : MonoBehaviour
    {
        [SerializeField] private Transform _windowBox;

        [Header("Window Header")]
        [SerializeField] private Transform _headerTransform;
        [SerializeField] private TextMeshProUGUI _title;
        
        [Space]

        [Header("Vertical Layout Window")]
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private Transform _verticalLayoutTransform;
        [SerializeField] private Image _heroImage;
        [SerializeField] private TextMeshProUGUI _heroText;

        [Space]
        [Header("Horizontal Layout Window")]
        [SerializeField] private Transform _horizontalLayoutTransform;

        [Header("Window Footer")]
        [SerializeField] private Transform _footerTransform;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private TextMeshProUGUI _confirmButtonText;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private TextMeshProUGUI _cancelButtonText;
        [SerializeField] private Button _alternativeButton;
        [SerializeField] private TextMeshProUGUI _alternativeButtonText;

        private Action OnConfirmClicked;
        private Action OnCancelClicked;
        private Action OnAlternativeClicked;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Shows the modal window without a title but with an image.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="imageToShow"></param>
        /// <param name="confirm"></param>
        /// <param name="cancel"></param>
        public void ShowWindow(string message, Sprite imageToShow, Action confirm, Action cancel)
        {
            ShowWindow("", imageToShow, message, confirm, "Confirm",cancel, "Decline");
        }

        /// <summary>
        /// Shows the modal window without an image.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="confirm"></param>
        /// <param name="cancel"></param>
        public void ShowWindow(string title, string message, Action confirm, Action cancel)
        {
            ShowWindow(title, null, message, confirm, "Confirm", cancel, "Decline");
        }

        /// <summary>
        /// Shows the modal window with an image.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="imageToShow"></param>
        /// <param name="message"></param>
        /// <param name="confirmAction"></param>
        /// <param name="cancelAction"></param>
        /// <param name="alternateAction"></param>
        public void ShowWindow(string title, Sprite imageToShow, string message, Action confirmAction, string confirmButtonText, Action cancelAction, string cancelButtonText, Action alternateAction = null, string alternativeButtonText = "")
        {
            gameObject.SetActive(true);

            _horizontalLayoutTransform.gameObject.SetActive(false);

            HasTitle(title);

            HasHeroImage(imageToShow, message);

            _heroText.text = message;

            OnConfirmClicked = confirmAction;
            _confirmButtonText.text = confirmButtonText;
            OnCancelClicked = cancelAction;
            _cancelButtonText.text = cancelButtonText;

            bool hasAlternateAction = alternateAction != null;
            _alternativeButton.gameObject.SetActive(hasAlternateAction);
            OnAlternativeClicked = alternateAction;
            _alternativeButtonText.text = alternativeButtonText;
        }

        private void HasTitle(string title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                _headerTransform.gameObject.SetActive(true);
                _title.text = title;
            }
            else
            {
                _headerTransform.gameObject.SetActive(false);
                _title.text = string.Empty;
            }
        }

        private void HasHeroImage(Sprite imageToShow, string message)
        {
            if (imageToShow != null)
            {
                _heroImage.gameObject.SetActive(true);
                _heroImage.sprite = imageToShow;
            }
            else
            {
                _heroImage.gameObject.SetActive(false);
                _heroImage.sprite = null;
            }
        }

        public void Confirm()
        {
            OnConfirmClicked?.Invoke();
            CloseWindow();
        }

        public void Cancel()
        {
            OnCancelClicked?.Invoke();
            CloseWindow();
        }

        public void Alternate()
        {
            OnAlternativeClicked?.Invoke();
            CloseWindow();
        }

        public void CloseWindow()
        {
            gameObject.SetActive(false);
        }
    }
}
