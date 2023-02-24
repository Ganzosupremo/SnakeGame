using System.Diagnostics;

namespace SnakeGame.Debuging
{
    /// <summary>
    /// Use this class to debug anything to the unity console.
    /// </summary>
    public static class Debuger
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        [Conditional("ENABLE_LOGS")]
        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        [Conditional("ENABLE_LOGS")]
        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError(message);
        }
    }
}
