using UnityEngine;
using SnakeGame.ProceduralGenerationSystem;

namespace SnakeGame.GameUtilities
{
    /// <summary>
    /// This is a generic class and can be used to spawn any item on any room,
    /// just make sure that it's variables are defined in the <see cref="RoomTemplateSO"/>,
    /// and <seealso cref="Room"/> scripts for each and every room, that the desired item will spawn.
    /// </summary>
    [System.Serializable]
    public class RoomItemSpawnParameters
    {
        #region Tooltip
        [Tooltip("Defines the game level for this room with regard to how many food in total should be spawned")]
        #endregion Tooltip
        public GameLevelSO gameLevel;

        #region Tooltip
        [Tooltip("The minimum number of items that will be allowed to spawn on this room in this current game level at any given time.")]
        #endregion Tooltip
        public int minTotalItemsToSpawn;
        #region Tooltip
        [Tooltip("The maximum number of items that will be allowed to spawn on this room in this current game level at any given time.")]
        #endregion Tooltip
        public int maxTotalItemsToSpawn;

        #region Tooltip
        [Tooltip("The maximum number of this item that are gonna be active on the game world at any given time." +
            " The actual number will be a random value between the minimum and maximum values.")]
        #endregion Tooltip
        public int minConcurrentItems;
        #region Tooltip
        [Tooltip("The maximum number of this item that are gonna be active on the game world at any given time." +
            " The actual number will be a random value between the minimum and maximum values.")]
        #endregion Tooltip
        public int maxConcurrentItems;

        #region Tooltip
        [Tooltip("The minimun delay the items will have between spawns.")]
        #endregion Tooltip
        public int minSpawnInterval;
        #region Tooltip
        [Tooltip("The maximum delay the items will have between spawns.")]
        #endregion Tooltip
        public int maxSpawnInterval;
    }

    /// <summary>
    /// This generic class can be used to spawn any item on any level map.
    /// </summary>
    [System.Serializable]
    public class ItemSpawnParameters
    {
        #region Tooltip
        [Tooltip("The minimum number of items that will be allowed to spawn on this room in this current game level at any given time.")]
        #endregion Tooltip
        public int minTotalItemsToSpawn;
        #region Tooltip
        [Tooltip("The maximum number of items that will be allowed to spawn on this room in this current game level at any given time.")]
        #endregion Tooltip
        public int maxTotalItemsToSpawn;

        #region Tooltip
        [Tooltip("The maximum number of this item that are gonna be active on the game world at any given time." +
            " The actual number will be a random value between the minimum and maximum values.")]
        #endregion Tooltip
        public int minConcurrentItems;
        #region Tooltip
        [Tooltip("The maximum number of this item that are gonna be active on the game world at any given time." +
            " The actual number will be a random value between the minimum and maximum values.")]
        #endregion Tooltip
        public int maxConcurrentItems;

        #region Tooltip
        [Tooltip("The minimun delay the items will have between spawns.")]
        #endregion Tooltip
        public int minSpawnInterval;
        #region Tooltip
        [Tooltip("The maximum delay the items will have between spawns.")]
        #endregion Tooltip
        public int maxSpawnInterval;
    }
}