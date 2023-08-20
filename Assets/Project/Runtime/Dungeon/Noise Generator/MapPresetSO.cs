using SnakeGame.GameUtilities;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame.Dungeon.NoiseGenerator
{
    [CreateAssetMenu(fileName = "MapPreset_", menuName = "Scriptable Objects/Dungeon/NoiseMap/Map Preset")]
    public class MapPresetSO : ScriptableObject
    {
        #region Header MAP CONFIGURATION
        [Header("MAP SETTINGS")]
        #endregion Header MAP CONFIGURATION
        public string levelName;

        #region Header
        [Space(10)]
        [Header("Height Map")]
        #endregion
        public Wave[] heightWaves;
        [HideInInspector] public float[,] heightMap;
        #region Header
        [Space(10)]
        [Header("Moisture Map")]
        #endregion
        public Wave[] moistureWaves;
        [HideInInspector] public float[,] moistureMap;
        #region Header
        [Space(10)]
        [Header("Heat Map")]
        #endregion
        public Wave[] heatWaves;
        [HideInInspector] public float[,] heatMap;
        #region Header
        [Space(10)]
        [Header("Spawn Positions")]
        [Tooltip("Make sure you don't go out of the width and height of this map, " +
            "don't put positions near the borders.")]
        #endregion
        public List<Vector2Int> spawnPositionsList;

        #region Commented Code
        //#region Header Enemy Details
        //[Space(10)]
        //[Header("Enemy Spawn Details")]
        //#endregion
        //#region Tooltip
        //[Tooltip("Populate with all the enemies that can be spawned on this level map, including the ratio of this enemy type" +
        //    " that will be spawned," +
        //    " a higher ratio means it will spawn often than the other with lower ratios.")]
        //#endregion
        //public List<SpawnableObject<EnemyDetailsSO>> enemiesByLevelList;
        //#region Tooltip
        //[Tooltip("Populate with the spawn parameters for the enemies")]
        //#endregion
        //public List<ItemSpawnParameters> enemySpawnParemetersList;
        //public bool IsClearOfEnemies { get; set; } = false;

        //#region Header Food Spawn Details
        //[Space(10)]
        //[Header("Food Spawn Details")]
        //[Space(5)]
        //#endregion
        //#region Tooltip
        //[Tooltip("Populate with all the kinds of differents foods" +
        //    " that can be spawned on this level map. Include also the ratio of the different foods," +
        //    " a higher ratio, means it will spawn often than the other with lower ratios.")]
        //#endregion
        //public List<SpawnableObject<FoodSO>> foodByLevelList;
        //#region Tooltip
        //[Tooltip("Populate with the spawn parameters for the types of food")]
        //#endregion
        //public List<ItemSpawnParameters> foodSpawnParametersList;

        //public int GetNumberOfItemsToSpawn(int index = 1)
        //{
        //    if (index == 1)
        //    {
        //        foreach (ItemSpawnParameters spawnParameters in enemySpawnParemetersList)
        //        {
        //            return Random.Range(spawnParameters.minTotalItemsToSpawn, spawnParameters.maxTotalItemsToSpawn);
        //        }
        //        return 0;
        //    }
        //    else if (index == 2)
        //    {
        //        foreach (ItemSpawnParameters spawnParameters in foodSpawnParametersList)
        //        {
        //            return Random.Range(spawnParameters.minTotalItemsToSpawn, spawnParameters.maxTotalItemsToSpawn);
        //        }
        //        return 0;
        //    }
        //    else
        //        return 0;
        //}

        //public ItemSpawnParameters GetRoomItemSpawnParameters(int index = 1)
        //{
        //    if (index == 1)
        //    {
        //        foreach (ItemSpawnParameters spawnParameters in enemySpawnParemetersList)
        //        {
        //            return spawnParameters;
        //        }
        //        return null;
        //    }
        //    else if (index == 2)
        //    {
        //        foreach (ItemSpawnParameters spawnParameters in foodSpawnParametersList)
        //        {
        //            return spawnParameters;
        //        }
        //        return null;
        //    }
        //    else
        //        return null;
        //}
        #endregion
    }
}
