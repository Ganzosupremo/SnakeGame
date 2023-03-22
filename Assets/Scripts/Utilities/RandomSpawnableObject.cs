using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace SnakeGame.GameUtilities
{
    /// <summary>
    /// Defines a <see cref="ChanceBoundaries"/> struct, in which the <see cref="T"/> is the object to spawn
    /// and the 'lowBoundaryValue' and 'highBoundaryValue' are defined for every object to spawn
    /// this creates a table of objects that can be spawn and this table is used to know which
    /// object to spawn randomly.
    /// </summary>
    /// <typeparam name="T">The type of object to add to the table and spawn</typeparam>
    public class RandomSpawnableObject<T>
    {
        private struct ChanceBoundaries
        {
            public T spawnableObject;
            public int lowBoundaryValue;
            public int highBoundaryValue;
        }

        private int totalValueRatio = 0;
        private List<ChanceBoundaries> chanceBoundariesList = new();
        private List<SpawnableObjectByLevel<T>> spawnableObjectByLevelList;
        private List<SpawnableObject<T>> spawnableObjects;

        public RandomSpawnableObject(List<SpawnableObjectByLevel<T>> spawnableObjectByLevelList)
        {
            this.spawnableObjectByLevelList = spawnableObjectByLevelList;
        }

        public RandomSpawnableObject(List<SpawnableObject<T>> spawnableObjects)
        {
            this.spawnableObjects = spawnableObjects;
        }

        /// <summary>
        /// Gets a random item to spawn
        /// </summary>
        /// <returns>Returns the object T that has been randomly selected</returns>
        public T GetRandomItem()
        {
            int upperBoundary = -1;
            totalValueRatio = 0;
            chanceBoundariesList.Clear();
            T spawnableObject = default(T);

            foreach (SpawnableObjectByLevel<T> spawnableObjectByLevel in spawnableObjectByLevelList)
            {
                // Check if it is the current level
                if (spawnableObjectByLevel.gameLevel == GameManager.Instance.GetCurrentDungeonLevel())
                {
                    foreach (SpawnableObjectRatio<T> spawnableObjectRatio in spawnableObjectByLevel.spawnableObjectRatioList)
                    {
                        // Calculate lower and upper boundary values
                        int lowerBoundary = upperBoundary + 1;
                        upperBoundary = lowerBoundary + spawnableObjectRatio.spawnRatio - 1;

                        // Update the totalValueRatio
                        totalValueRatio += spawnableObjectRatio.spawnRatio;

                        // Add spawnable object to the list
                        chanceBoundariesList.Add(new ChanceBoundaries()
                        {
                            spawnableObject = spawnableObjectRatio.dungeonObject,
                            lowBoundaryValue = lowerBoundary,
                            highBoundaryValue = upperBoundary,
                        });

                    }
                }
            }

            if (chanceBoundariesList.Count == 0) return default;

            // Define a value to look up
            int lookUpValue = Random.Range(0, totalValueRatio);

            // Loop to get the randomly selected spawnable object details
            foreach (ChanceBoundaries spawnChance in chanceBoundariesList)
            {
                if (lookUpValue >= spawnChance.lowBoundaryValue && lookUpValue <= spawnChance.highBoundaryValue)
                {
                    spawnableObject = spawnChance.spawnableObject;
                    break;
                }
            }

            return spawnableObject;
        }

        /// <summary>
        /// Gets a random item to spawn in the current map
        /// </summary>
        /// <returns>Returns the object T that has been randomly selected</returns>
        public T RandomMapItem
        {
            get
            {
                int upperBoundary = -1;
                totalValueRatio = 0;
                chanceBoundariesList.Clear();
                T spawnableObject = default(T);

                foreach (SpawnableObject<T> spawnableObjects in spawnableObjects)
                {
                    // Check if it is the current level
                    //if (spawnableObjects.gameLevel == GameManager.Instance.GetCurrentDungeonLevel())
                    //{
                    foreach (SpawnableObjectRatio<T> spawnableObjectRatio in spawnableObjects.spawnableObjectRatios)
                    {
                        // Calculate lower and upper boundary values
                        int lowerBoundary = upperBoundary + 1;
                        upperBoundary = lowerBoundary + spawnableObjectRatio.spawnRatio - 1;

                        // Update the totalValueRatio
                        totalValueRatio += spawnableObjectRatio.spawnRatio;

                        // Add spawnable object to the list
                        chanceBoundariesList.Add(new ChanceBoundaries()
                        {
                            spawnableObject = spawnableObjectRatio.dungeonObject,
                            lowBoundaryValue = lowerBoundary,
                            highBoundaryValue = upperBoundary,
                        });
                    }
                    //}
                }

                if (chanceBoundariesList.Count == 0) return default;

                // Define a value to look up
                int lookUpValue = Random.Range(0, totalValueRatio);

                // Loop to get the randomly selected spawnable object details
                foreach (ChanceBoundaries spawnChance in chanceBoundariesList)
                {
                    if (lookUpValue >= spawnChance.lowBoundaryValue && lookUpValue <= spawnChance.highBoundaryValue)
                    {
                        spawnableObject = spawnChance.spawnableObject;
                        break;
                    }
                }
                return spawnableObject;
            }
        }
    }
}