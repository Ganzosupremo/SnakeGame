#define ENABLE_LOGS
#define A
using System;
using System.Diagnostics;

namespace SnakeGame.Debuging
{
    /// <summary>
    /// Use this class to debug anything to the unity console.
    /// </summary>
    public static class Debuger
    {
#if UNITY_EDITOR
        //[Conditional("ENABLE_LOGS")]
        public static void Log(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        //[Conditional("ENABLE_LOGS")]
        public static void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        //[Conditional("ENABLE_LOGS")]
        public static void LogError(object message)
        {
            UnityEngine.Debug.LogError(message);
        }
#endif
    }
}
