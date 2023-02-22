using UnityEngine;
using SnakeGame;

[System.Serializable]
public class RoomFoodSpawnParameters
{
    #region Tooltip
    [Tooltip("Defines the game level for this room with regard to how many food in total should be spawned")]
    #endregion Tooltip
    public GameLevelSO gameLevel;

    #region Tooltip
    [Tooltip("The minimum number of food that are gonna be be spawned at any given time in this room at this game level.")]
    #endregion Tooltip
    public int minTotalFoodToSpawn;
    #region Tooltip
    [Tooltip("The maximum number of food that are gonna be spawned at any given time in this room at this game level.")]
    #endregion Tooltip
    public int maxTotalFoodToSpawn;

    #region Tooltip
    [Tooltip("The minimum food gameobjects that are allowed to spawn at the same time. In order for more gameobjects to spawn, " +
        "one gameobjects needs to despawn (deactivate) from the world. The actual number will be a random value between the minimum and maximum values.")]
    #endregion Tooltip
    public int minConcurrentFood;
    #region Tooltip
    [Tooltip("The maximum food gameobjects that are allowed to spawn at the same time. In order for more gameobjects to spawn," +
        " one gameobjects needs to despawn (deactivate) from the world. The actual number will be a random value between the minimum and maximum values.")]
    #endregion Tooltip
    public int maxConcurrentFood;

    #region Tooltip
    [Tooltip("The minimun delay for the food to spawn")]
    #endregion Tooltip
    public int minSpawnInterval;
    #region Tooltip
    [Tooltip("The maximum delay for the food to spawn")]
    #endregion Tooltip
    public int maxSpawnInterval;
}
