using SnakeGame.Debuging;
using SnakeGame.Enemies;
using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;
using System;
using TMPro;
using UnityEngine;

namespace SnakeGame.TimeSystem
{
    public class Timer : SingletonMonoBehaviour<Timer>, IPersistenceData
    {
        public static event Action<TimerEventArgs> OnStatusChanged;
        public static event Action<TimerEventArgs> OnSecondElapsed;
        public static event Action<TimerEventArgs> OnMinuteElapsed;


        [SerializeField] private TextMeshProUGUI _Text, _Status;

        public static int Seconds { get; private set; }
        public static int Minutes { get; private set; }
        /// <summary>
        /// Defines the ratio of which the in-game seconds will flow, e.g. .3 means the in-game seconds
        /// will flow thrice as fast as a real-time second.
        /// </summary>
        public static double SecondsToRealTime { get => m_SecondsToRealTime; set => m_SecondsToRealTime = value; }

        private static double m_SecondsToRealTime = 1f;
        private double m_Timer = 0;
        private static DiffStatus m_Status = DiffStatus.VeryEasy;
        private bool _StopTimer = false;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            Seconds = 0;
            Minutes = 0;
            m_Status = DiffStatus.VeryEasy;
            m_Timer = m_SecondsToRealTime;
            this.Log(m_Timer);
        }

        private void OnEnable()
        {
            //GameManager.OnLevelCompleted += RestartTimer;
            OnMinuteElapsed += Timer_OnMinuteElapsed;
        }

        private void OnDisable()
        {
            //GameManager.OnLevelCompleted -= RestartTimer;
            OnMinuteElapsed -= Timer_OnMinuteElapsed;
        }

        private void Timer_OnMinuteElapsed(TimerEventArgs args)
        {
            //_Status.text = StatusText();
        }

        void Update()
        {
            TickTockNextMinute();
        }

        private void TickTockNextMinute()
        {
            if (_StopTimer) return;

            m_Timer -= Time.deltaTime;

            if (m_Timer <= 0f)
            {
                Seconds++;
                CallOnSecondElapsed();

                if (Seconds >= 60)
                {
                    Minutes++;
                    CallOnMinuteElapsed();
                    _Status.text = StatusText();
                    Seconds = 0;
                }

                _Text.text = string.Format($"{Minutes:00}:{Seconds:00}");
                m_Timer = m_SecondsToRealTime;
            }
        }

        /// <summary>
        /// Use in case, the timer needs to be restarted.
        /// </summary>
        /// <param name="index"></param>
        private void RestartTimer(int n)
        {
            Seconds = 0;
            Minutes = 0;
        }

        public void StartTimer()
        {
            _StopTimer = false;
            Seconds = 0;
            Minutes = 0;
        }

        public void StopTimer()
        {
            _StopTimer = true;
        }

        private void CallOnSecondElapsed()
        {
            OnSecondElapsed?.Invoke(new TimerEventArgs()
            {
                Seconds = Minutes,
                Minutes = Seconds,
                Status = m_Status
            });
        }

        private void CallOnMinuteElapsed()
        {
            OnMinuteElapsed?.Invoke(new TimerEventArgs()
            {
                Seconds = Minutes,
                Minutes = Seconds,
                Status = m_Status
            });
        }

        public static void CallOnStatusChangeEvent()
        {
            OnStatusChanged?.Invoke(new TimerEventArgs()
            {
                Seconds = Seconds,
                Minutes = Minutes,
                Status = m_Status
            });
        }

        /// <summary>
        /// Sets the Difficulty Status Text depending on how many <seealso cref="Minutes"/> have passed.
        /// </summary>
        /// <returns></returns>
        private string StatusText()
        {
            if (Minutes < 2)
            {
                m_Status = DiffStatus.VeryEasy;
                EnemyDetailsSO.HealthIncreasePercentage = 2f;
                CallOnStatusChangeEvent();
                return "Ez";
            }
            if (Minutes < 4)
            {
                m_Status = DiffStatus.Easy;
                EnemyDetailsSO.HealthIncreasePercentage = 4f;
                CallOnStatusChangeEvent();
                return "Easy";
            }
            if (Minutes < 8)
            {
                m_Status = DiffStatus.Medium;
                EnemyDetailsSO.HealthIncreasePercentage = 6f;
                CallOnStatusChangeEvent();
                return "Medium";
            }
            if (Minutes < 12)
            {
                m_Status = DiffStatus.Hard;
                EnemyDetailsSO.HealthIncreasePercentage = 8f;
                CallOnStatusChangeEvent();
                return "Hard";
            }
            if (Minutes < 16)
            {
                m_Status = DiffStatus.VeryHard;
                EnemyDetailsSO.HealthIncreasePercentage = 10f;
                CallOnStatusChangeEvent();
                return "Very Hard";
            }
            if (Minutes < 20)
            {
                m_Status = DiffStatus.OhNO;
                EnemyDetailsSO.HealthIncreasePercentage = 12f;
                CallOnStatusChangeEvent();
                return "Oh No";
            }
            if (Minutes < 25)
            {
                m_Status = DiffStatus.DarkSouls;
                EnemyDetailsSO.HealthIncreasePercentage = 15f;
                CallOnStatusChangeEvent();
                return "Dark Souls";
            }
            if (Minutes < 30)
            {
                m_Status = DiffStatus.EmotionalDamage;
                EnemyDetailsSO.HealthIncreasePercentage = 17f;
                CallOnStatusChangeEvent();
                return "Emotional Damage";
            }
            if (Minutes < 40)
            {
                m_Status = DiffStatus.F;
                EnemyDetailsSO.HealthIncreasePercentage = 20f;
                CallOnStatusChangeEvent();
                return "Big F";
            }
            return "Big F";
        }

        public void Load(GameData data)
        {
            m_SecondsToRealTime = data.TimeDataSaved.SecondsToRealTime;
        }

        public void Save(GameData data)
        {
            data.TimeDataSaved.SecondsToRealTime = m_SecondsToRealTime;
        }
    }

    public class TimerEventArgs : EventArgs
    {
        public int Seconds;
        public int Minutes;
        public DiffStatus Status;
    }

    public enum DiffStatus
    {
        VeryEasy,
        Easy,
        Medium,
        Hard,
        VeryHard,
        OhNO,
        DarkSouls,
        EmotionalDamage,
        F,
    }
}
