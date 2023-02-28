using SnakeGame.AStarPathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame
{
    public class MoveableDecoration : MonoBehaviour
    {

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            AStar.BuildPath(GameManager.Instance.GetCurrentRoom(), Vector3Int.zero, new(1, 1, 0));
        }
    }
}
