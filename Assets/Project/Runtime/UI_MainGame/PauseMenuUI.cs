using EasyUI.Tabs;
using SnakeGame.AudioSystem;
using SnakeGame.GameUtilities;
using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;
using SnakeGame.TimeSystem;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SnakeGame.UI
{
    public class PauseMenuUI : MonoBehaviour, IPersistenceData
    {
        [SerializeField] private TMP_Dropdown dayCicleDropdown;

        private DayCicle _selectedTime;

        void Start()
        {
            dayCicleDropdown.onValueChanged.RemoveAllListeners();
            dayCicleDropdown.onValueChanged.AddListener(SetDayTime);

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            Time.timeScale = 0f;
            TabsUI.OnTabClicked += TabsUI_OnTabClicked;
            SaveDataManager.Instance.LoadGame();
            StartCoroutine(InitializeUI());
        }

        private void TabsUI_OnTabClicked(int obj)
        {
            SaveDataManager.Instance.SaveGame();
        }

        private void OnDisable()
        {
            Time.timeScale = 1f;
            TabsUI.OnTabClicked -= TabsUI_OnTabClicked;
            SaveDataManager.Instance.SaveGame();
        }

        public void SetDayTime(int dayIndex)
        {
            _selectedTime = (DayCicle)dayIndex;
            TimeManager.Instance.CallOnTimeChangedEvent((DayCicle)dayIndex);
        }

        private IEnumerator InitializeUI()
        {
            yield return null;
            dayCicleDropdown.value = (int)_selectedTime;
        }

        public void LoadMainMenu()
        {
            SceneManager.LoadScene((int)SceneIndex.MainMenu, LoadSceneMode.Single);
        }

        public void Load(GameData data)
        {
            _selectedTime = data.TimeDataSaved.SavedTime;
            dayCicleDropdown.value = (int)data.TimeDataSaved.SavedTime;
        }

        public void Save(GameData data)
        {
            data.TimeDataSaved.SavedTime = _selectedTime;
        }
    }
}
