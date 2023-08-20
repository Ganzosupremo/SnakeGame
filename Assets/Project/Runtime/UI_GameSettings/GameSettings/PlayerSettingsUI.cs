using Cysharp.Threading.Tasks;
using SnakeGame.GameUtilities;
using SnakeGame.PlayerSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SnakeGame
{
    public class PlayerSettingsUI : MonoBehaviour
    {
        #region Tooltip
        [Tooltip("Populate with the Input Field")]
        #endregion
        [SerializeField] private TMP_InputField _PlayerNameInput;
        
        [Tooltip("The parent for the instantiated buttons, should be" +
            " inside the scroll rect's viewport gameobject")]
        [SerializeField] private Transform _scrollRectContentGameobject;
        [SerializeField] private GameObject ButtonTemplatePrefab;

        [Space]
        [Header("Right Panel References")]
        [SerializeField] private Image Portraitimage;
        [SerializeField] private TextMeshProUGUI SnakeNameText;
        [SerializeField] private TextMeshProUGUI SnakeAbilityText;

        private CurrentPlayerSO _currentSnake;
        private List<SnakeDetailsSO> _allSnakeDetails = new();
        private List<Button> _buttonsInScrollView = new();
        private void Awake()
        {
            EnumerateAllSnakes();
        }

        private void OnEnable()
        {
            ClearButtonsList();
            InitializeUI();
        }



        private void InitializeUI()
        {
            GameObject buttonTemplate = Instantiate(ButtonTemplatePrefab, _scrollRectContentGameobject);
            Button buttonComponent = buttonTemplate.GetComponent<Button>();

            for (int i = 0; i < _allSnakeDetails.Count; i++)
            {
                GameObject buttonInstance = Instantiate(buttonTemplate, _scrollRectContentGameobject);
                _buttonsInScrollView.Add(buttonInstance.GetComponent<Button>());

                _buttonsInScrollView[i].GetComponentInChildren<TextMeshProUGUI>().text = _allSnakeDetails[i].snakeName;

                int index = i; // Store the value in a separate variable to capture it correctly in the lambda.
                _buttonsInScrollView[i].onClick.AddListener(() => OnButtonClicked(index));
            }

            SelectFirstButton(_buttonsInScrollView[0]);

            // Destroy the original button template as it's not needed anymore.
            Destroy(buttonTemplate);
        }

        private void OnButtonClicked(int index)
        {
            _currentSnake.snakeDetails = _allSnakeDetails[index];

            Portraitimage.sprite = _allSnakeDetails[index].PortraitImage;
            SnakeNameText.text = _allSnakeDetails[index].snakeName;
            SnakeAbilityText.text = _allSnakeDetails[index].ability.AbilityName;
        }

        private void Start()
        {
            _PlayerNameInput.text = _currentSnake.snakeName;
        }

        public void UpdatePlayerName()
        {
            _PlayerNameInput.text = _PlayerNameInput.text.ToUpper();
            _currentSnake.snakeName = _PlayerNameInput.text;
        }

        private void EnumerateAllSnakes()
        {
            _currentSnake = GameResources.Instance.currentSnake;
            _allSnakeDetails = GameResources.Instance.snakeDetailsList;
        }

        private void SelectFirstButton(Button firstSelected)
        {
            firstSelected.Select();
            firstSelected.onClick.Invoke();
        }

        private void ClearButtonsList()
        {
            foreach (var button in _buttonsInScrollView)
            {
                button.onClick.RemoveAllListeners();
                Destroy(button.gameObject);
            }
            _buttonsInScrollView.Clear();
        }

        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValue(this, nameof(_PlayerNameInput), _PlayerNameInput);
        }
#endif
        #endregion

    }
}
