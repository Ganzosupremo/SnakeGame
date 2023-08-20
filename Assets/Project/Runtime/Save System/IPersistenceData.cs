using SnakeGame.SaveAndLoadSystem;

namespace SnakeGame.Interfaces
{
    /// <summary>
    /// All the things that are gonna be saved to the disk
    /// need to implement this interface.
    /// The <see cref="SaveDataManager"/> will find all the objects implementing this interface.
    /// </summary>
    public interface IPersistenceData
    {
        /// <summary>
        /// Loads the saved game data from the disk
        /// </summary>
        /// <param name="data"></param>
        void Load(GameData data);

        /// <summary>
        /// Saves the game data to the disk
        /// </summary>
        /// <param name="data"></param>
        void Save(GameData data);
    }
}