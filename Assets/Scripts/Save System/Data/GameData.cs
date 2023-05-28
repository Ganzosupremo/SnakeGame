using SnakeGame.HighscoreSystem;
using SnakeGame.TimeSystem;

namespace SnakeGame.SaveAndLoadSystem
{
    [System.Serializable]
    public class GameData
    {
        public long LastUpdated;

        public VolumeData VolumeDataSaved = new();

        public DifficultyData DifficultyData = new();

        public TimeData TimeDataSaved = new();
        public DayCicle SavedTime;

        public HighScore HighScores;

        public GameData()
        {

            DifficultyData = new DifficultyData();
            TimeDataSaved = new TimeData();
            VolumeDataSaved = new VolumeData();

            SavedTime = DayCicle.Morning;
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
        public int MinigunVolume;
        public int MusicVolume;

        public int GetSoundsVolume()
        {
            return SoundsVolume;
        }

        public int GetMinigunVolume()
        {
            return MinigunVolume;
        }

        public int GetMusicVolume()
        {
            return MusicVolume;
        }

        public void Save(int soundsVolume = 9, int minigunVolume = 1, int musicVolume = 6)
        {
            SoundsVolume = soundsVolume;
            MinigunVolume = minigunVolume;
            MusicVolume = musicVolume;
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

}