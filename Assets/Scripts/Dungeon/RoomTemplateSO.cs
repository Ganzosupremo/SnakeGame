using SnakeGame.Debuging;
using SnakeGame.Enemies;
using SnakeGame.FoodSystem;
using SnakeGame.GameUtilities;
using SnakeGame.AudioSystem;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SnakeGame.ProceduralGenerationSystem
{
    /// <summary>
    /// This is the template for the room that the dungeon builder will use to build the level
    /// </summary>
    [CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Objects/Dungeon/Room Template")]
    public class RoomTemplateSO : ScriptableObject
    {
        [HideInInspector] public string guid;

        #region Header ROOM PREFAB
        [Header("ROOM PREFAB")]
        [Space(10)]
        #endregion Header ROOM PREFAB
        #region Tooltip

        [Tooltip("The gameobject prefab for the room (this will contain all the tilemaps for the room and environment game objects")]

        #endregion Tooltip
        public GameObject prefab;
        // this is used to regenerate the guid if the SO is copied and the prefab is changed
        [HideInInspector] public GameObject previousPrefab;

        #region Header ROOM PREFAB
        [Header("MUSIC")]
        [Space(10)]
        #endregion Header ROOM PREFAB
        public MusicSO normalMusic;
        public MusicSO battleMusic;

        #region Header ROOM CONFIGURATION
        [Header("ROOM CONFIGURATION")]
        [Space(10)]
        #endregion Header ROOM CONFIGURATION
        #region Tooltip
        [Tooltip("The room node type SO. The room node types correspond to the room nodes used in the room node graph. The exceptions being with corridors. In the room node graph there is just one corridor type 'Corridor'. For the room templates there are 2 corridor node types - CorridorNS and CorridorEW.")]
        #endregion Tooltip
        public RoomNodeTypeSO roomNodeType;

        #region Tooltip
        [Tooltip("If you imagine a rectangle around the room tilemap that just completely encloses it, the room lower bounds represent the bottom left corner of that rectangle. This should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for that bottom left corner (Note: this is the local tilemap position and NOT world position")]
        #endregion Tooltip
        public Vector2Int lowerBounds;

        #region Tooltip
        [Tooltip("If you imagine a rectangle around the room tilemap that just completely encloses it, the room upper bounds represent the top right corner of that rectangle. This should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for that top right corner (Note: this is the local tilemap position and NOT world position")]
        #endregion Tooltip
        public Vector2Int upperBounds;

        #region Tooltip
        [Space(10)]
        [Header("Doorways")]
        [Tooltip("There should be a maximum of four doorways for a room - one for each compass direction.  These should have a consistent 3 tile opening size, with the middle tile position being the doorway coordinate 'position'")]
        #endregion Tooltip
        public List<Doorway> doorwayList;

        #region Tooltip
        [Space(10)]
        [Header("Spawn Positions")]
        [Tooltip("Each possible spawn position used to spawn things on the room in local coordinates should be added to this array")]
        #endregion Tooltip
        public Vector2Int[] spawnPositionArray;

        #region Header Enemy Details
        [Header("ENEMY SPAWN SETTINGS")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("Populate with all the enemies that can be spawned in this room by game level, including the ratio of this enemy type" +
            " that will be spawned," +
            " a higher ratio means it will spawn often than the other with lower ratios.")]
        #endregion
        public List<SpawnableObjectByLevel<EnemyDetailsSO>> enemiesByLevelList;

        #region Tooltip
        [Tooltip("Populate with the spawn parameters for the enemies")]
        #endregion
        public List<RoomItemSpawnParameters> roomEnemySpawnParemetersList;

        #region Header Food Spawn Details
        [Header("FOOD SPAWN SETTINGS")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("Populate with all the kinds of differents foods" +
            " that can be spawned in this room at this game level. Include also the ratio of the different foods," +
            " a higher ratio, means it will spawn often than the other with lower ratios.")]
        #endregion
        public List<SpawnableObjectByLevel<FoodSO>> foodByLevelList;
        #region Tooltip
        [Tooltip("Populate with the spawn parameters for the types of food")]
        #endregion
        public List<RoomItemSpawnParameters> roomFoodSpawnParametersList;

        /// <summary>
        /// Returns the list of Entrances for the room template
        /// </summary>
        public List<Doorway> GetDoorwayList()
        {
            return doorwayList;
        }

        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            // Set unique GUID if empty or the prefab changes
            if (guid == "" || previousPrefab != prefab)
            {
                guid = GUID.Generate().ToString();
                previousPrefab = prefab;
                EditorUtility.SetDirty(this);
            }
            HelperUtilities.ValidateCheckNullValue(this, nameof(prefab), prefab);
            HelperUtilities.ValidateCheckNullValue(this, nameof(battleMusic), battleMusic);
            HelperUtilities.ValidateCheckNullValue(this, nameof(normalMusic), normalMusic);
            HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeType), roomNodeType);
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);
            
            CheckEnemyParams();
            CheckFoodParams();
            // Check spawn positions if populated
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
        }

        private void CheckEnemyParams()
        {
            #region Check all the parameters for the enemies are defined
            // Check if enemies are gonna spawn in a room and that really all the necessary details
            // have been populated.
            if (enemiesByLevelList.Count > 0 || roomEnemySpawnParemetersList.Count > 0)
            {
                HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemiesByLevelList), enemiesByLevelList);
                HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomEnemySpawnParemetersList), roomEnemySpawnParemetersList);

                foreach (RoomItemSpawnParameters enemySpawnParameters in roomEnemySpawnParemetersList)
                {
                    HelperUtilities.ValidateCheckNullValue(this, nameof(enemySpawnParameters.gameLevel), enemySpawnParameters.gameLevel);
                    HelperUtilities.ValidateCheckPositiveRange(this, nameof(enemySpawnParameters.minTotalItemsToSpawn), enemySpawnParameters.minTotalItemsToSpawn,
                        nameof(enemySpawnParameters.maxTotalItemsToSpawn), enemySpawnParameters.maxTotalItemsToSpawn, true);

                    HelperUtilities.ValidateCheckPositiveRange(this, nameof(enemySpawnParameters.minSpawnInterval), enemySpawnParameters.minSpawnInterval,
                        nameof(enemySpawnParameters.maxSpawnInterval), enemySpawnParameters.maxSpawnInterval, true);

                    HelperUtilities.ValidateCheckPositiveRange(this, nameof(enemySpawnParameters.minConcurrentItems), enemySpawnParameters.minConcurrentItems,
                        nameof(enemySpawnParameters.maxConcurrentItems), enemySpawnParameters.maxConcurrentItems, true);

                    bool isEnemyTypeListForDungeonLevelFound = true;

                    foreach (SpawnableObjectByLevel<EnemyDetailsSO> spawnableObjectsByLevel in enemiesByLevelList)
                    {
                        if (spawnableObjectsByLevel.gameLevel == enemySpawnParameters.gameLevel &&
                            spawnableObjectsByLevel.spawnableObjectRatioList.Count > 0)
                            isEnemyTypeListForDungeonLevelFound = true;

                        HelperUtilities.ValidateCheckNullValue(this, nameof(spawnableObjectsByLevel.gameLevel), spawnableObjectsByLevel.gameLevel);

                        foreach (SpawnableObjectRatio<EnemyDetailsSO> spawnableObjectRatio in spawnableObjectsByLevel.spawnableObjectRatioList)
                        {
                            HelperUtilities.ValidateCheckNullValue(this, nameof(spawnableObjectRatio.dungeonObject), spawnableObjectRatio.dungeonObject);
                            HelperUtilities.ValidateCheckPositiveValue(this, nameof(spawnableObjectRatio.spawnRatio), spawnableObjectRatio.spawnRatio, false);
                        }
                    }

                    if (!isEnemyTypeListForDungeonLevelFound && enemySpawnParameters.gameLevel != null)
                    {
                        this.Log($"No types of enemies specified for the dungeon level {enemySpawnParameters.gameLevel.levelName}, "
                            + $"located in the gameobject {name}");
                    }
                }
            }
            #endregion
        }

        private void CheckFoodParams()
        {
            #region Check all the parameters for food are defined
            // Check if food is gonna spawn in this room at this game level and that every detail
            // is populated in the room template.
            if (foodByLevelList.Count > 0 || roomFoodSpawnParametersList.Count > 0)
            {
                HelperUtilities.ValidateCheckEnumerableValues(this, nameof(foodByLevelList), foodByLevelList);
                HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomFoodSpawnParametersList), roomFoodSpawnParametersList);

                foreach (RoomItemSpawnParameters foodSpawnParameters in roomFoodSpawnParametersList)
                {
                    HelperUtilities.ValidateCheckNullValue(this, nameof(foodSpawnParameters.gameLevel), foodSpawnParameters.gameLevel);
                    HelperUtilities.ValidateCheckPositiveRange(this, nameof(foodSpawnParameters.minTotalItemsToSpawn), foodSpawnParameters.minTotalItemsToSpawn,
                        nameof(foodSpawnParameters.maxTotalItemsToSpawn), foodSpawnParameters.maxTotalItemsToSpawn, true);

                    HelperUtilities.ValidateCheckPositiveRange(this, nameof(foodSpawnParameters.minSpawnInterval), foodSpawnParameters.minSpawnInterval,
                        nameof(foodSpawnParameters.maxSpawnInterval), foodSpawnParameters.maxSpawnInterval, true);

                    HelperUtilities.ValidateCheckPositiveRange(this, nameof(foodSpawnParameters.minConcurrentItems), foodSpawnParameters.minConcurrentItems,
                        nameof(foodSpawnParameters.maxConcurrentItems), foodSpawnParameters.maxConcurrentItems, true);

                    bool isFoodTypeListForDungeonLevelFound = true;

                    foreach (SpawnableObjectByLevel<FoodSO> spawnableObjectsByLevel in foodByLevelList)
                    {
                        if (spawnableObjectsByLevel.gameLevel == foodSpawnParameters.gameLevel &&
                            spawnableObjectsByLevel.spawnableObjectRatioList.Count > 0)
                            isFoodTypeListForDungeonLevelFound = true;

                        HelperUtilities.ValidateCheckNullValue(this, nameof(spawnableObjectsByLevel.gameLevel), spawnableObjectsByLevel.gameLevel);

                        foreach (SpawnableObjectRatio<FoodSO> spawnableObjectRatio in spawnableObjectsByLevel.spawnableObjectRatioList)
                        {
                            HelperUtilities.ValidateCheckNullValue(this, nameof(spawnableObjectRatio.dungeonObject), spawnableObjectRatio.dungeonObject);
                            HelperUtilities.ValidateCheckPositiveValue(this, nameof(spawnableObjectRatio.spawnRatio), spawnableObjectRatio.spawnRatio, false);
                        }
                    }

                    if (!isFoodTypeListForDungeonLevelFound && foodSpawnParameters.gameLevel != null)
                    {
                        this.Log($"No type of food was specified for the game level {foodSpawnParameters.gameLevel.levelName}, "
                            + $"located in the gameobject {name}.");
                    }
                }
            }
            #endregion
        }
#endif
        #endregion
    }
}