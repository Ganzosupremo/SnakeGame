using SnakeGame.Enemies;
using SnakeGame.FoodSystem;
using SnakeGame.GameUtilities;
using SnakeGame.AudioSystem;
using System.Collections.Generic;
using UnityEngine;
using SnakeGame.NodeGraph;

namespace SnakeGame.ProceduralGenerationSystem
{
    public class Room
    {
        public string id;
        public string templateID;
        public GameObject roomTilemapPrefab;
        public RoomNodeTypeSO roomNodeType;

        public MusicSO normalMusic;
        public MusicSO battleMusic;

        public int roomWidth;
        public int roomHeight;
        /// <summary>
        /// The lower bounds in World space
        /// </summary>
        public Vector2Int lowerBounds;
        /// <summary>
        /// The upper bounds in World space
        /// </summary>
        public Vector2Int upperBounds;
        /// <summary>
        /// The lower bounds in Local space
        /// </summary>
        public Vector2Int tilemapLowerBounds;
        /// <summary>
        /// The upper bounds in Local space
        /// </summary>
        public Vector2Int tilemapUpperBounds;
        public Vector2Int[] spawnPositionArray;

        public List<SpawnableObjectByLevel<EnemyDetailsSO>> EnemiesByLevelList { get; set; }
        public List<RoomItemSpawnParameters> RoomLevelEnemySpawnParametersList { get; set; }

        public List<SpawnableObjectByLevel<FoodSO>> FoodsByLevelList { get; set; }
        public List<RoomItemSpawnParameters> RoomLevelFoodSpawnParametersList { get; set; }

        public List<string> ChildRoomIDList;
        public string ParentRoomID;
        public List<Doorway> doorwayList;
        public bool IsPositioned { get; set; } = false;
        public InstantiatedRoom InstantiatedRoom;
        public bool IsLit { get; set; } = false;
        public bool IsClearOfEnemies { get; set; } = false;
        public bool IsPreviouslyVisited { get; set; } = false;

        public Room()
        {
            ChildRoomIDList = new();
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
        public int GetNumberOfItemsToSpawn(GameLevelSO gameLevel, bool retrieveEnemyData = true)
        {
            if (retrieveEnemyData)
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
            else
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
        }

        /// <summary>
        /// Get item spawn parameters  with the index
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
}