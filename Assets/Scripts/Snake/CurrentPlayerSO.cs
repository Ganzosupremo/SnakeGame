using UnityEngine;

[CreateAssetMenu(fileName = "CurrentSnake", menuName = "Scriptable Objects/Player/Current Snake")]
public class CurrentPlayerSO : ScriptableObject
{
    public SnakeDetailsSO snakeDetails;
    public string snakeName;
}
