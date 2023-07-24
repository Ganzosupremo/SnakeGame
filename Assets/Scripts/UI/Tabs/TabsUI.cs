using SnakeGame.AudioSystem;
using SnakeGame.Debuging;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//------- Created by  : Hamza Herbou
//------- Email       : hamza95herbou@gmail.com

namespace EasyUI.Tabs
{
    public enum TabsType
    {
        Horizontal,
        Vertical
    }
    public abstract class TabsUI : MonoBehaviour
    {
        [System.Serializable]
        public class TabsUIEvent : UnityEvent<int>
        {

        }

        [Header("Tabs customization:")]
        [SerializeField] private Color themeColor = Color.gray;

        [SerializeField] private float _tabSpacing = 2f;

        [Space]
        [Header("Tab's Font Customization")]
        [SerializeField] private Color _activeTabFontColor = Color.black;
        [SerializeField] private Color _inactiveTabFontColor = Color.white;

        [Space]
        #region Tooltip
        [Tooltip("A flag that defines if the tab's background color should be customizable, " +
            "if false the color will default to a darker color of the main theme.")]
        #endregion
        [SerializeField] private bool _customTabColor = false;
        [SerializeField] private Color _tabColorActive = Color.white, _tabColorInactive = Color.white;

        [Space]
        [Header("OnTabChange event:")]
        public TabsUIEvent OnTabChange;

        [Space]
        [Header("Sound effect when a tab is clicked.")]
        [SerializeField] private SoundEffectSO TabSelectedSoundEffect;

        private TabButtonUI[] _tabButtonsArray;
        private GameObject[] _tabContentArray;

#if UNITY_EDITOR
        private LayoutGroup _layoutGroup;
#endif

        private Color _defaultTabColorActive, _defaultTabColorInactive;
        private int _currentTab, _previousTab;

        private Transform _parentButtons, _parentContent;

        private int _tabBtnsCount, _tabContentCount;

        private void Start()
        {
            InitialiseUI();
        }

        private void InitialiseUI()
        {
            _parentButtons = transform.GetChild(0);
            _parentContent = transform.GetChild(1);
            _tabBtnsCount = _parentButtons.childCount;
            _tabContentCount = _parentContent.childCount;

            if (_tabBtnsCount != _tabContentCount)
            {
                this.LogError($"!!Number of <b>[Buttons] ({_tabBtnsCount})</b> is not the same as <b>[Contents] ({_tabContentCount})</b>.");
                return;
            }

            _tabButtonsArray = new TabButtonUI[_tabBtnsCount];
            _tabContentArray = new GameObject[_tabBtnsCount];
            for (int i = 0; i < _tabBtnsCount; i++)
            {
                _tabButtonsArray[i] = _parentButtons.GetChild(i).GetComponent<TabButtonUI>();
                int i_copy = i;
                _tabButtonsArray[i].uiButton.onClick.RemoveAllListeners();
                _tabButtonsArray[i].uiButton.onClick.AddListener(() => OnTabButtonClicked(i_copy));

                _tabContentArray[i] = _parentContent.GetChild(i).gameObject;
                _tabButtonsArray[i].uiButton.GetComponentInChildren<TextMeshProUGUI>().color = _inactiveTabFontColor;
                _tabContentArray[i].SetActive(false);
            }

            _previousTab = _currentTab = 0;

            _defaultTabColorActive = _tabButtonsArray[0].uiImage.color;
            _defaultTabColorInactive = _tabButtonsArray[1].uiImage.color;

            _tabButtonsArray[0].uiButton.interactable = false;
            _tabButtonsArray[0].uiButton.GetComponentInChildren<TextMeshProUGUI>().color = _activeTabFontColor;
            _tabContentArray[0].SetActive(true);
        }

        public void OnTabButtonClicked(int tabIndex)
        {
            if (_currentTab == tabIndex) return;

            OnTabChange?.Invoke(tabIndex);

            PlayAudio();

            _previousTab = _currentTab;
            _currentTab = tabIndex;

            _tabContentArray[_previousTab].SetActive(false);
            _tabContentArray[_currentTab].SetActive(true);

            if (_customTabColor) 
            { 
                _tabButtonsArray[_previousTab].uiImage.color = _tabColorInactive;
                _tabButtonsArray[_currentTab].uiImage.color = _tabColorActive;
            }
            else
            {
                _tabButtonsArray[_previousTab].uiImage.color = _defaultTabColorInactive;
                _tabButtonsArray[_currentTab].uiImage.color = _defaultTabColorActive;
            }

            _tabButtonsArray[_previousTab].uiButton.GetComponentInChildren<TextMeshProUGUI>().color = _inactiveTabFontColor;
            _tabButtonsArray[_currentTab].uiButton.GetComponentInChildren<TextMeshProUGUI>().color = _activeTabFontColor;

            _tabButtonsArray[_previousTab].uiButton.interactable = true;
            _tabButtonsArray[_currentTab].uiButton.interactable = false;
        }

        public void PlayAudio()
        {
            if (TabSelectedSoundEffect != null)
                SoundEffectManager.CallOnSoundEffectChangedEvent(TabSelectedSoundEffect);
        }


#if UNITY_EDITOR
        public void Validate(TabsType type)
        {
            _parentButtons = transform.GetChild(0);
            _parentContent = transform.GetChild(1);
            _tabBtnsCount = _parentButtons.childCount;
            _tabContentCount = _parentContent.childCount;

            ClearArraysIfNeeded();

            _tabButtonsArray = new TabButtonUI[_tabBtnsCount];
            _tabContentArray = new GameObject[_tabBtnsCount];

            for (int i = 0; i < _tabBtnsCount; i++)
            {
                _tabButtonsArray[i] = _parentButtons.GetChild(i).GetComponent<TabButtonUI>();
                _tabContentArray[i] = _parentContent.GetChild(i).gameObject;
            }

            UpdateThemeColor(themeColor);

            if (_layoutGroup == null)
                _layoutGroup = _parentButtons.GetComponent<LayoutGroup>();

            if (type == TabsType.Horizontal)
                ((HorizontalLayoutGroup)_layoutGroup).spacing = _tabSpacing;
            else if (type == TabsType.Vertical)
                ((VerticalLayoutGroup)_layoutGroup).spacing = _tabSpacing;

        }

        private void ClearArraysIfNeeded()
        {
            if (_tabButtonsArray != null)
            {
                Array.Clear(_tabButtonsArray, 0, _tabButtonsArray.Length);
                _tabButtonsArray = null;
            }

            if (_tabContentArray != null)
            {
                Array.Clear(_tabContentArray, 0, _tabContentArray.Length);
                _tabContentArray = null;
            }
        }

        public void UpdateThemeColor(Color color)
        {
            if (_customTabColor)
                _tabButtonsArray[0].uiImage.color = _tabColorActive;
            else
                _tabButtonsArray[0].uiImage.color = color;

            _tabButtonsArray[0].uiButton.GetComponentInChildren<TextMeshProUGUI>().color = _activeTabFontColor;
            
            Color colorDark = DarkenColor(color, 0.3f);

            for (int i = 1; i < _tabBtnsCount; i++)
            {
                if (_customTabColor)
                    _tabButtonsArray[i].uiImage.color = _tabColorInactive;
                else
                    _tabButtonsArray[i].uiImage.color = colorDark;

                _tabButtonsArray[i].uiButton.GetComponentInChildren<TextMeshProUGUI>().color = _inactiveTabFontColor;
            }

            _parentContent.GetComponent<Image>().color = color;
        }

        private Color DarkenColor(Color color, float amount)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            v = Mathf.Max(0f, v - amount);
            return Color.HSVToRGB(h, s, v);
        }
#endif

    }
}