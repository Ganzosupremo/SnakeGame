using System.Collections.Generic;

namespace SnakeGame.HighscoreSystem
{
    [System.Serializable]
    public class HighScore
    {
        public List<Score> ScoreList = new();
    }
}