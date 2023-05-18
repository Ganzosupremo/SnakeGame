using System;

namespace SnakeGame
{
    public static class DifficultyManager
    {
        public static event Action<Difficulty> OnDifficultyChanged;

        public static void CallOnDifficultyChangedEvent(Difficulty difficulty)
        {
            OnDifficultyChanged?.Invoke(difficulty);
        }
    }
}
