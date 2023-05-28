using System;
using UnityEngine;

namespace SnakeGame
{
    public class DifficultyManager : MonoBehaviour
    {
        public static event Action<Difficulty> OnDifficultyChanged;

        public static void CallOnDifficultyChangedEvent(Difficulty difficulty)
        {
            OnDifficultyChanged?.Invoke(difficulty);
        }
    }

    public class DifficultyEventArgs : EventArgs
    {
        public Difficulty Difficulty;
    }
}
