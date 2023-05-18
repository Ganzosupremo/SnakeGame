using SnakeGame.HighscoreSystem;
using SnakeGame.TimeSystem;

namespace SnakeGame.SaveAndLoadSystem
{
    public class GameData
    {
        public long LastUpdated;


        public VolumeData VolumeDataSaved;
        public int SoundsVolume;
        public int MinigunVolume;
        public int MusicVolume;

        public Difficulty SavedDifficulty;
        public DifficultyData DifficultyData;

        public DayTimeData DayTimeDataSaved;
        public DayCicle SavedTime;

        public HighScore HighScores;

        public GameData()
        {
            SavedDifficulty = Difficulty.Noob;
            SavedTime = DayCicle.Morning;
            HighScores = new();

            SoundsVolume = 6;
            MinigunVolume = 6;
            MusicVolume = 9;
        }
    }

    /// <summary>
    /// Used to store data of the game difficulty
    /// </summary>
    public struct DifficultyData
    {
        public Difficulty DifficultyToSave;
    }

    /// <summary>
    /// Used to store data of the game volume player preferences
    /// </summary>
    public struct VolumeData
    {
        public int SoundsVolume;
        public int MinigunVolume;
        public int MusicVolume;
    }

    /// <summary>
    /// Used to store data of the current datetime selected by the player
    /// </summary>
    public struct DayTimeData
    {
        public DayCicle SavedTime;
    }

}