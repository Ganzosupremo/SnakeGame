using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SnakeGame.TimeSystem
{
    [DisallowMultipleComponent]
    public class TimeManager : SingletonMonoBehaviour<TimeManager>, IPersistenceData
    {
        public static event Action<DayCicle> OnTimeChanged;

        public DayCicle CurrentTime { get; private set; } = DayCicle.Morning;
        private Light2D globalLight;

        protected override void Awake()
        {
            base.Awake();
            globalLight = Instantiate(GameResources.Instance.globalLight, GameManager.Instance.transform);
        }

        private void Start()
        {
            ChangeTime(CurrentTime);
        }

        private void OnEnable()
        {
            OnTimeChanged += ChangeTime;
        }

        private void OnDisable()
        {
            OnTimeChanged -= ChangeTime;
        }

        public void CallOnTimeChangedEvent(DayCicle time)
        {
            OnTimeChanged?.Invoke(time);
        }

        /// <summary>
        /// Changes the global light intensity based on the selected time.
        /// </summary>
        /// <param name="dayCicle"></param>
        private void ChangeTime(DayCicle dayCicle)
        {
            CurrentTime = dayCicle;
            switch (dayCicle)
            {
                case DayCicle.Morning:
                    SetLightIntesity(1f);
                    break;
                case DayCicle.Afternoon:
                    SetLightIntesity(0.8f);
                    break;
                case DayCicle.Evening:
                    SetLightIntesity(0.5f);
                    break;
                case DayCicle.Night:
                    SetLightIntesity(0.35f);
                    break;
                default:
                    break;
            }
        }

        private void SetLightIntesity(float intensity)
        {
            globalLight.intensity = intensity;
        }

        public void Load(GameData data)
        {
            CurrentTime = data.SavedTime;
        }

        public void Save(GameData data)
        {
            data.SavedTime = CurrentTime;
        }
    }

    public enum DayCicle
    {
        Morning = 0,
        Afternoon = 1,
        Evening = 2,
        Night = 3
    }
}
