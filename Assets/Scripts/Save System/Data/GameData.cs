using SnakeGame.UI;

namespace SnakeGame.SaveAndLoadSystem
{
    public class GameData
    {
        public long lastUpdated;

        public int soundsVolume;
        public int minigunVolume;
        public int musicVolume;

        public Difficulty savedDifficulty;
        public DayCicle savedTime;
        public int test;

        public GameData()
        {
            savedDifficulty = Difficulty.Noob;
            savedTime = DayCicle.Morning;
            test = 126554;

            soundsVolume= 6;
            minigunVolume= 6;
            musicVolume= 9;
        }
    }
}