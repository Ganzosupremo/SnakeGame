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

    //public List<SpawnableObjectByLevel<EnemyDetailsSO>> enemiesByLevelList;
    //public List<RoomEnemySpawnParemeters> roomLevelEnemySpawnParametersList;

    public List<string> childRoomIDList;
    public string parentRoomID;
    //public List<Doorway> doorwayList;
    public bool isPositioned = false;
    public InstantiatedRoom instantiatedRoom;
    public bool isLit;
    public bool isClearOfEnemies = false;
    public bool isPreviouslyVisited = false;
}
