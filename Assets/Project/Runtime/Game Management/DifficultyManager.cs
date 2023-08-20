using System;

namespace SnakeGame
{
    public static class DifficultyManager
    {
        public static event Action<DifficultyEventArgs> OnDifficultyChanged;

        public static void CallOnDifficultyChangedEvent(Difficulty difficulty)
        {
            OnDifficultyChanged?.Invoke(new DifficultyEventArgs
            {
                Difficulty = difficulty
            });
        }
    }

    public class DifficultyEventArgs : EventArgs
    {
        public Difficulty Difficulty;
    }
}
