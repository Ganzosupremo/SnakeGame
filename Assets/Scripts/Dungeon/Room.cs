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

    public List<SpawnableObjectByLevel<EnemyDetailsSO>> enemiesByLevelList;
    public List<RoomEnemySpawnParameters> roomLevelEnemySpawnParametersList;

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
    /// Get the number of enemies that will spawn in this game level.
    /// </summary>
    /// <param name="gameLevel"></param>
    /// <returns>Returns the total number of enemies to spawn in this level.</returns>
    public int GetNumberOfEnemiesToSpawn(GameLevelSO gameLevel)
    {
        foreach (RoomEnemySpawnParameters spawnParameters in roomLevelEnemySpawnParametersList)
        {
            if (spawnParameters.gameLevel == gameLevel)
            {
                return Random.Range(spawnParameters.minTotalEnemiesToSpawn, spawnParameters.maxTotalEnemiesToSpawn);
            }
        }

        return 0;
    }

    /// <summary>
    /// Get the enemy spawn parameters for this game level.
    /// </summary>
    /// <param name="gameLevel"></param>
    /// <returns>Returns the enemy spawn parameters, returns null if none has been found.</returns>
    public RoomEnemySpawnParameters GetRoomEnemySpawnParameters(GameLevelSO gameLevel)
    {
        foreach (RoomEnemySpawnParameters spawnParameters in roomLevelEnemySpawnParametersList)
        {
            if (spawnParameters.gameLevel == gameLevel)
            {
                return spawnParameters;
            }
        }
        return null;
    }
}
