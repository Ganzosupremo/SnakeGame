using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace SnakeGame.GameUtilities
{
    /// <summary>
    /// Defines a <see cref="ChanceBoundaries"/> struct, in which the <see cref="T"/> is the object to spawn
    /// and the 'lowBoundaryValue' and 'highBoundaryValue' are defined for every object to spawn.
    /// This creates a table of objects that can be spawn and this table is used to know which
    /// object to spawn randomly.
    /// </summary>
    /// <typeparam name="T">The type of object to add to the table and spawn</typeparam>
    public class RandomSpawnableObject<T>
    {
        private struct ChanceBoundaries
        {
            public T SpawnableObject;
            public int LowBoundaryValue;
            public int HighBoundaryValue;
        }

        private int m_TotalValueRatio = 0;
        private List<ChanceBoundaries> m_ChanceBoundariesList = new();
        private List<SpawnableObjectByLevel<T>> m_SpawnableObjectByLevelList;

        public RandomSpawnableObject(List<SpawnableObjectByLevel<T>> spawnableObjectByLevelList)
        {
            m_SpawnableObjectByLevelList = spawnableObjectByLevelList;
        }

        /// <summary>
        /// Gets a random item to spawn
        /// </summary>
        /// <returns>Returns the object T that has been randomly selected</returns>
        public T GetRandomItem()
        {
            int upperBoundary = -1;
            m_TotalValueRatio = 0;
            m_ChanceBoundariesList.Clear();
            T spawnableObject = default;

            foreach (SpawnableObjectByLevel<T> spawnableObjectByLevel in m_SpawnableObjectByLevelList)
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
                        m_TotalValueRatio += spawnableObjectRatio.spawnRatio;

                        // Add spawnable object to the list
                        m_ChanceBoundariesList.Add(new ChanceBoundaries()
                        {
                            SpawnableObject = spawnableObjectRatio.dungeonObject,
                            LowBoundaryValue = lowerBoundary,
                            HighBoundaryValue = upperBoundary,
                        });

                    }
                }
            }

            if (m_ChanceBoundariesList.Count == 0) return default;

            // Define a value to look up
            int lookUpValue = Random.Range(0, m_TotalValueRatio);

            // Loop to get the randomly selected spawnable object details
            foreach (ChanceBoundaries spawnChance in m_ChanceBoundariesList)
            {
                if (lookUpValue >= spawnChance.LowBoundaryValue && lookUpValue <= spawnChance.HighBoundaryValue)
                {
                    spawnableObject = spawnChance.SpawnableObject;
                    break;
                }
            }

            return spawnableObject;
        }
    }
}