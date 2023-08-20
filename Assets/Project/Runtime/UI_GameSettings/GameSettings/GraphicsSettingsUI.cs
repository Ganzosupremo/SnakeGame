using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace SnakeGame.UI
{
    public class GraphicsSettingsUI : MonoBehaviour, IPersistenceData
    {
        [Header("Game Resolutions Settings")]
        [Space(3)]
        [SerializeField] private TMP_Dropdown _resolutionDropdown;
        [SerializeField] private Toggle _fullscreenToggle;
        [SerializeField] private Toggle _vSyncToggle;

        [Space]
        [Header("Post Processing Settings")]
        [SerializeField] private Volume _mainVolume;
        [SerializeField] private TextMeshProUGUI _bloomIntensityText;
        [SerializeField] private Toggle _postProcessingToggle;

        private Resolution[] _gameResolutionsArray;
        private Bloom _bloom;
        private bool _postProcessingEnabled = false;
        private bool _fullscreen = false;
        private bool _vSyncOn = false;

        private void Awake()
        {
            _mainVolume.profile.TryGet(out _bloom);
        }
        private void Start()
        {
            _gameResolutionsArray = Screen.resolutions;
            InitiliaseResolutionDropdown();
        }

        private void OnEnable()
        {
            SaveDataManager.Instance.LoadGame();
            InitialiseUI();
        }

        private void OnDisable()
        {
            SaveDataManager.Instance.SaveGame();
        }

        private void InitialiseUI()
        {
            _fullscreenToggle.isOn = Screen.fullScreen;
            _vSyncToggle.isOn = QualitySettings.vSyncCount > 0;

            _bloomIntensityText.SetText($"Bloom Intesity: {_bloom.intensity.value}");
        }

        private void InitiliaseResolutionDropdown()
        {
            ClearDropdown();

            List<string> options = new();

            int currentResolution = 0;
            for (int i = 0; i < _gameResolutionsArray.Length; i++)
            {
                string option = $"{_gameResolutionsArray[i].width} x {_gameResolutionsArray[i].height}";
                options.Add(option);

                if (IsEqualResolution(i))
                    currentResolution = i;
            }

            _resolutionDropdown.AddOptions(options);
            _resolutionDropdown.value = currentResolution;
            _resolutionDropdown.RefreshShownValue();
        }

        private bool IsEqualResolution(int index)
        {
            if (_gameResolutionsArray[index].width != Screen.currentResolution.width &&
                _gameResolutionsArray[index].height != Screen.currentResolution.height)
                return false;

            return true;
        }

        public void SetFullscreenToggle(bool fullscreen)
        {
            _fullscreen = fullscreen;
            Screen.fullScreen = _fullscreen;
        }

        public void SetVSync(bool vsync)
        {
            // Deactivate Vsync
            _vSyncOn = vsync;
            if (!_vSyncOn)
                QualitySettings.vSyncCount = 0;
            // Activate Vsync
            else
                QualitySettings.vSyncCount = 1;
        }

        public void SetQuality(int qualityLevel)
        {
            QualitySettings.SetQualityLevel(qualityLevel);
        }

        public void SetResolution(int resolutionLevel)
        {
            Resolution newResolution = _gameResolutionsArray[resolutionLevel];
            Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreen);
        }

        public void SetBloomIntensity(float intensity)
        {
            _bloom.intensity.value = intensity;
            _bloomIntensityText.SetText($"Bloom Intesity: {intensity:0.0}");
        }

        public void DisablePostProcessing(bool value)
        {
            _postProcessingEnabled = !value;
            _mainVolume.enabled = !value;
        }

        private void ClearDropdown()
        {
            _resolutionDropdown.ClearOptions();
        }

        public void Load(GameData data)
        {
            _fullscreen = data.GraphicsDataSaved.IsFullscreen;
            _vSyncOn = data.GraphicsDataSaved.VSyncOn;
        }

        public void Save(GameData data)
        {
            data.GraphicsDataSaved.IsFullscreen = _fullscreen;
            data.GraphicsDataSaved.VSyncOn = _vSyncOn;
        }
    }
}
