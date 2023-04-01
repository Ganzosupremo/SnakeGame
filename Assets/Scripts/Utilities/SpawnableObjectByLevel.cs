using SnakeGame.ProceduralGenerationSystem;
using System.Collections.Generic;

namespace SnakeGame.GameUtilities
{
    /// <summary>
    /// Defines the objects to spawn based on the <seealso cref="GameLevelSO"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to spawn</typeparam>
    [System.Serializable]
    public class SpawnableObjectByLevel<T>
    {
        public GameLevelSO gameLevel;
        public List<SpawnableObjectRatio<T>> spawnableObjectRatioList;
    }

    /// <summary>
    /// Spawns objects based on it's ratio
    /// </summary>
    /// <typeparam name="T">The type of object to spawn</typeparam>
    [System.Serializable]
    public class SpawnableObject<T>
    {
        public List<SpawnableObjectRatio<T>> spawnableObjectRatios;
    }
}

