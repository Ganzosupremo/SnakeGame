namespace SnakeGame.GameUtilities
{
    /// <summary>
    /// Defines the ratio on which the object T is gonna spawn
    /// </summary>
    /// <typeparam name="T">The object to spawn</typeparam>
    [System.Serializable]
    public class SpawnableObjectRatio<T>
    {
        public T dungeonObject;
        public int spawnRatio;
    }
}