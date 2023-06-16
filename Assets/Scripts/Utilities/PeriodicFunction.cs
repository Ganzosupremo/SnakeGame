using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame.GameUtilities
{
    /// <summary>
    /// Executes a given function periodically
    /// </summary>
    public class PeriodicFunction
    {
        /// <summary>
        /// Holds a reference to all active timers
        /// </summary>
        private static List<PeriodicFunction> _FuncList;
        /// <summary>
        /// Global game object used for initializing class, is destroyed on scene change
        /// </summary>
        private static GameObject _InitGameObject;


        private GameObject _GameObject;
        private float _Timer;
        private float _BaseTimer;
        private bool _UseUnscaledDeltaTime;
        private string _FunctionName;
        public Action _Action;
        public Func<bool> _TestDestroy;

        private PeriodicFunction(GameObject gameObject, Action action, float timer, Func<bool> testDestroy, string functionName, bool useUnscaledDeltaTime)
        {
            _GameObject = gameObject;
            _Action = action;
            _Timer = timer;
            _TestDestroy = testDestroy;
            _FunctionName = functionName;
            _UseUnscaledDeltaTime = useUnscaledDeltaTime;
            _BaseTimer = timer;
        }

        private static void InitIfNeeded()
        {
            if (_InitGameObject == null)
            {
                _InitGameObject = new GameObject("PeriodicFunctionGlobal");
                _FuncList = new List<PeriodicFunction>();
            }
        }

        /// <summary>
        /// Persist through scene loads
        /// </summary>
        /// <param name="action"></param>
        /// <param name="testDestroy"></param>
        /// <param name="timer"></param>
        /// <returns></returns>
        public static PeriodicFunction CreateGlobal(Action action, Func<bool> testDestroy, float timer)
        {
            PeriodicFunction functionPeriodic = Create(action, testDestroy, timer, "", false, false, false);
            MonoBehaviour.DontDestroyOnLoad(functionPeriodic._GameObject);
            return functionPeriodic;
        }

        private void Update()
        {
            if (_UseUnscaledDeltaTime)
            {
                _Timer -= Time.unscaledDeltaTime;
            }
            else
            {
                _Timer -= Time.deltaTime;
            }
            if (_Timer <= 0)
            {
                _Action?.Invoke();
                if (_TestDestroy != null && _TestDestroy())
                {
                    //Destroy
                    DestroySelf();
                }
                else
                {
                    //Repeat
                    _Timer += _BaseTimer;
                }
            }
        }

        /// <summary>
        /// Trigger the given action every given time.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="testDestroy">Executes after triggering the action</param>
        /// <param name="timer">Time intervalo to execute the action.</param>
        /// <returns>If returns true, it destroys itself.</returns>
        public static PeriodicFunction Create(Action action, Func<bool> testDestroy, float timer)
        {
            return Create(action, testDestroy, timer, "", false);
        }

        public static PeriodicFunction Create(Action action, float timer)
        {
            return Create(action, null, timer, "", false, false, false);
        }

        public static PeriodicFunction Create(Action action, float timer, string functionName)
        {
            return Create(action, null, timer, functionName, false, false, false);
        }

        public static PeriodicFunction Create(Action callback, Func<bool> testDestroy, float timer, string functionName, bool stopAllWithSameName)
        {
            return Create(callback, testDestroy, timer, functionName, false, false, stopAllWithSameName);
        }

        public static PeriodicFunction Create(Action action, Func<bool> testDestroy, float timer, string functionName, bool useUnscaledDeltaTime, bool triggerImmediately, bool stopAllWithSameName)
        {
            InitIfNeeded();

            if (stopAllWithSameName)
                StopAllFunc(functionName);

            GameObject gameObject = new("FunctionPeriodic Object " + functionName, typeof(MonoBehaviourHook));
            PeriodicFunction functionPeriodic = new(gameObject, action, timer, testDestroy, functionName, useUnscaledDeltaTime);
            gameObject.GetComponent<MonoBehaviourHook>().OnUpdate = functionPeriodic.Update;

            _FuncList.Add(functionPeriodic);

            if (triggerImmediately) action();

            return functionPeriodic;
        }

        public static void RemoveTimer(PeriodicFunction funcTimer)
        {
            InitIfNeeded();
            _FuncList.Remove(funcTimer);
        }

        public static void StopTimer(string name)
        {
            InitIfNeeded();
            for (int i = 0; i < _FuncList.Count; i++)
            {
                if (_FuncList[i]._FunctionName == name)
                {
                    _FuncList[i].DestroySelf();
                    return;
                }
            }
        }

        public static void StopAllFunc(string name)
        {
            InitIfNeeded();
            for (int i = 0; i < _FuncList.Count; i++)
            {
                if (_FuncList[i]._FunctionName == name)
                {
                    _FuncList[i].DestroySelf();
                    i--;
                }
            }
        }

        public static bool IsFuncActive(string name)
        {
            InitIfNeeded();
            for (int i = 0; i < _FuncList.Count; i++)
            {
                if (_FuncList[i]._FunctionName == name)
                    return true;
            }
            return false;
        }

        public void SkipTimerTo(float timer)
        {
            _Timer = timer;
        }

        public void SetBaseTimer(float baseTimer)
        {
            this._BaseTimer = baseTimer;
        }

        public float GetBaseTimer()
        {
            return _BaseTimer;
        }

        public void DestroySelf()
        {
            RemoveTimer(this);
            if (_GameObject != null)
                UnityEngine.Object.Destroy(_GameObject);
        }

        /// <summary>
        /// Class to hook Actions into MonoBehaviour
        /// </summary>
        private class MonoBehaviourHook : MonoBehaviour
        {
            public Action OnUpdate;

            private void Update()
            {
                OnUpdate?.Invoke();
            }

        }
    }
}
