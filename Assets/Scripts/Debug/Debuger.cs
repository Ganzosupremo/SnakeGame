#define ENABLE_LOGS
using UnityEngine;
using System;

namespace SnakeGame.Debuging
{
    /// <summary>
    /// Use this class to debug anything to the unity console.
    /// </summary>
    public static class Debuger
    {
        public static string Color(this string myString, string color)
        {
            return $"<color={color}>{myString}</color>";
        }

#if ENABLE_LOGS

        public static void Log(this UnityEngine.Object myObject, params object[] message)
        {
            DoLog(Debug.Log, "<:)".Color("green"), myObject, message);
        }

        /// <summary>
        /// Use only this Log method on things that do not inherit from the 
        /// UnityEngine.Object class.
        /// </summary>
        /// <param name="message"></param>
        public static void Log(this object myObject, params object[] message)
        {
            DoLog(Debug.Log, "<:)".Color("white"), myObject, message);
        }

        public static void LogSucces(this UnityEngine.Object myObject, params object[] message)
        {
            DoLog(Debug.Log, "<:)".Color("green"),myObject, message);
        }

        public static void LogWarning(this UnityEngine.Object myObject, params object[] message)
        {
            DoLog(Debug.LogWarning, ">:(".Color("yellow"), myObject, message);
        }

        /// <summary>
        /// Use only this LogWarning method on things that do not inherit from the 
        /// UnityEngine.Object class.
        /// </summary>
        /// <param name="message"></param>
        public static void LogWarning(this object myObject, params object[] message)
        {
            DoLog(Debug.LogWarning, ">:(".Color("yellow"), myObject, message);
        }

        public static void LogError(this UnityEngine.Object myObject, params object[] message)
        {
            DoLog(Debug.LogError,"<!>".Color("red"), myObject, message);
        }

        /// <summary>
        /// Use only this LogError method on things that do not inherit from the 
        /// UnityEngine.Object class.
        /// </summary>
        /// <param name="message"></param>
        public static void LogError(this object myObject, params object[] message)
        {
            DoLog(Debug.LogError, "<!>".Color("red"), myObject, message);
        }

        private static void DoLog(Action<string, UnityEngine.Object> function, string prefix,UnityEngine.Object myObject, params object[] message)
        {
#if UNITY_EDITOR
            function($"{prefix} [{myObject.name}]: {string.Join("; ", message)}\n", myObject);
#endif
        }

        private static void DoLog(Action<string> function, string prefix, object myObject, params object[] message)
        {
#if UNITY_EDITOR
            function($"{prefix} [{myObject}]: {string.Join("; ", message)}\n");
#endif
        }
#endif
    }
}
