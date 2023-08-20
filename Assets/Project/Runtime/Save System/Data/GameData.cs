using SnakeGame.HighscoreSystem;
using SnakeGame.TimeSystem;

namespace SnakeGame.SaveAndLoadSystem
{
    [System.Serializable]
    public class GameData
    {
        public long LastUpdated;

        public VolumeData GameAudioData = new();

        public DifficultyData DifficultyData = new();

        public TimeData TimeDataSaved = new();

        public GraphicsData GraphicsDataSaved = new();
        //public DayCicle SavedTime;

        public HighScore HighScores;

        public GameData()
        {

            DifficultyData = new();
            TimeDataSaved = new();
            GameAudioData = new();
            GraphicsDataSaved = new();

            HighScores = new();
        }
    }

    /// <summary>
    /// Used to store data of the game difficulty
    /// </summary>
    [System.Serializable]
    public struct DifficultyData
    {
        public Difficulty DifficultyToSave;
    }

    /// <summary>
    /// Used to store data of the game volume player preferences
    /// </summary>
    [System.Serializable]
    public struct VolumeData
    {
        public int SoundsVolume;
        public int HeavyArsenalVolume;
        public int MusicVolume;

        public int GetSoundsVolume()
        {
            return SoundsVolume;
        }

        public int GetHeavyArsenalVolume()
        {
            return HeavyArsenalVolume;
        }

        public int GetMusicVolume()
        {
            return MusicVolume;
        }
    }

    /// <summary>
    /// Used to store data of the current datetime selected by the player
    /// </summary>
    [System.Serializable]
    public struct TimeData
    {
        public DayCicle SavedTime;
        public double SecondsToRealTime;
    }

    [System.Serializable]
    public struct GraphicsData
    {
        public bool IsFullscreen;
        public bool VSyncOn;
    }

}