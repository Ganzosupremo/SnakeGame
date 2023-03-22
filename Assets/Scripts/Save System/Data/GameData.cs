using SnakeGame.HighscoreSystem;
using SnakeGame.UI;

namespace SnakeGame.SaveAndLoadSystem
{
    public class GameData
    {
        public long LastUpdated;

        public int SoundsVolume;
        public int MinigunVolume;
        public int MusicVolume;

        public Difficulty SavedDifficulty;
        public DayCicle SavedTime;

        public HighScore HighScores;

        public GameData()
        {
            SavedDifficulty = Difficulty.Noob;
            SavedTime = DayCicle.Morning;
            HighScores = new();

            SoundsVolume= 6;
            MinigunVolume= 6;
            MusicVolume= 9;
        }
    }
}