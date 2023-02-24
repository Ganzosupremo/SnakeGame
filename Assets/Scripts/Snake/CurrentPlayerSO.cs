using UnityEngine;

namespace SnakeGame.PlayerSystem
{
    [CreateAssetMenu(fileName = "CurrentSnake", menuName = "Scriptable Objects/Player/Current Snake")]
    public class CurrentPlayerSO : ScriptableObject
    {
        public SnakeDetailsSO snakeDetails;
        public string snakeName;
    }
}