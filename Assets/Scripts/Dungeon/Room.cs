using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public string id;
    public string templateID;
    public GameObject roomTilemapPrefab;
    public RoomNodeTypeSO roomNodeType;

    //public MusicTrackSO ambientMusic;
    //public MusicTrackSO battleMusic;

    public int roomWidth;
    public int roomHeight;
    /// <summary>
    /// The lower bounds in World space
    /// </summary>
    public Vector2Int worldLowerBounds;
    /// <summary>
    /// The upper bounds in World space
    /// </summary>
    public Vector2Int worldUpperBounds;
    /// <summary>
    /// The lower bounds in local space
    /// </summary>
    public Vector2Int tilemapLowerBounds;
    /// <summary>
    /// The upper bounds in local space
    /// </summary>
    public Vector2Int tilemapUpperBounds;
    public Vector2Int[] spawnPositionArray;

    public List<SpawnableObjectByLevel<EnemyDetailsSO>> EnemiesByLevelList { get; set; }
    public List<RoomItemSpawnParameters> RoomLevelEnemySpawnParametersList { get; set; }

    public List<SpawnableObjectByLevel<FoodSO>> FoodsByLevelList { get; set; }
    public List<RoomItemSpawnParameters> RoomLevelFoodSpawnParametersList { get; set; }
    
    public List<string> childRoomIDList;
    public string parentRoomID;
    public List<Doorway> doorwayList;
    public bool isPositioned = false;
    public InstantiatedRoom instantiatedRoom;
    public bool isLit = false;
    public bool isClearOfEnemies = false;
    public bool isPreviouslyVisited = false;

    public Room()
    {
        childRoomIDList = new();
        doorwayList = new();
    }

    /// <summary>
    /// Get the total number of items that will spawn on this specific room on this game level.
    /// </summary>
    /// <param name="gameLevel"></param>
    /// <param name="index"> 1 to retrieve the total number of enemies that are allowed to spawn on the current room.
    /// 2 to retrive the total number of foods that are allowed to spawn on that room on that game level.
    /// The default is set to 1.</param>
    /// <returns>Returns the total number of items to spawn on the specified room on this current game level.</returns>
    public int GetNumberOfItemsToSpawns(GameLevelSO gameLevel, int index = 1)
    {
        if (index == 1)
        {
            foreach (RoomItemSpawnParameters spawnParameters in RoomLevelEnemySpawnParametersList)
            {
                if (spawnParameters.gameLevel == gameLevel)
                {
                    return Random.Range(spawnParameters.minTotalItemsToSpawn, spawnParameters.maxTotalItemsToSpawn);
                }
            }

            return 0;
        }
        else if (index == 2)
        {
            foreach (RoomItemSpawnParameters spawnParameters in RoomLevelFoodSpawnParametersList)
            {
                if (spawnParameters.gameLevel == gameLevel)
                {
                    return Random.Range(spawnParameters.minTotalItemsToSpawn, spawnParameters.maxTotalItemsToSpawn);
                }
            }

            return 0;
        }
        else
            return 0;

    }

    /// <summary>
    /// Get the enemy spawn parameters for this game level.
    /// </summary>
    /// <param name="gameLevel"></param>
    /// <param name="index">1 to retrieve the EnemySpawnParameters.
    /// 2 to retrive the FoodSpawnParameters.
    /// The default is set to 1.</param>
    /// <returns>Returns the enemy spawn parameters, returns null if none has been found.</returns>
    public RoomItemSpawnParameters GetRoomItemSpawnParameters(GameLevelSO gameLevel, int index = 1)
    {
        if (index == 1)
        {
            foreach (RoomItemSpawnParameters spawnParameters in RoomLevelEnemySpawnParametersList)
            {
                if (spawnParameters.gameLevel == gameLevel)
                {
                    return spawnParameters;
                }
            }
            return null;
        }
        else if (index == 2)
        {
            foreach (RoomItemSpawnParameters spawnParameters in RoomLevelFoodSpawnParametersList)
            {
                if (spawnParameters.gameLevel == gameLevel)
                {
                    return spawnParameters;
                }
            }
            return null;
        }
        else
            return null;
    }
}
