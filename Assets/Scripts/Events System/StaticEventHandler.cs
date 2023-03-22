using SnakeGame.Dungeon.NoiseGenerator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame
{
    public static class StaticEventHandler
    {
        #region ON ROOM CHANGED EVENT
        public static event Action<RoomChangedEventArgs> OnRoomChanged;

        public static void CallRoomChangedEvent(Room room)
        {
            OnRoomChanged?.Invoke(new RoomChangedEventArgs() { room = room });
        }
        #endregion

        #region ON MAP GENERATED
        public static event Action<MapGeneratedEventArgs> OnMapGenerated;

        public static void CallMapGeneratedEvent(int gridSize, int width, int height, MapPresetSO mapPreset)
        {
            OnMapGenerated?.Invoke(new MapGeneratedEventArgs()
            {
                gridSize = gridSize,
                width = width,
                height = height,
                mapPreset = mapPreset
            });
        }
        #endregion

        #region ON ROOM ENEMIES DEFEATED EVENT
        public static event Action<RoomEnemiesDefeatedArgs> OnRoomEnemiesDefeated;

        public static void CallRoomEnemiesDefeatedEvent(Room room)
        {
            OnRoomEnemiesDefeated?.Invoke(new RoomEnemiesDefeatedArgs() { room = room });
        }
        #endregion

        #region ON POINTS SCORED EVENT
        public static event Action<PointsScoredArgs> OnPointsScored;

        public static void CallPointsScoredEvent(long points)
        {
            OnPointsScored?.Invoke(new PointsScoredArgs()
            {
                score = points
            });
        }
        #endregion

        #region ON SCORE CHANGED EVENT
        public static event Action<ScoreChangedArgs> OnScoreChanged;

        public static void CallScoreChangedEvent(long score, int multiplier)
        {
            OnScoreChanged?.Invoke(new ScoreChangedArgs()
            {
                score = score,
                multiplier = multiplier
            });
        }
        #endregion

        #region ON MULTIPLIER EVENT
        public static event Action<MultiplierArgs> OnMultiplier;

        public static void CallMultiplierEvent(bool shouldMultiply)
        {
            OnMultiplier?.Invoke(new MultiplierArgs()
            {
                multiplier = shouldMultiply
            });
        }
        #endregion
    }

    public class RoomChangedEventArgs : EventArgs
    {
        public Room room;
    }

    public class MapGeneratedEventArgs : EventArgs
    {
        public int gridSize;
        public int width;
        public int height;
        public MapPresetSO mapPreset;
    }

    public class RoomEnemiesDefeatedArgs : EventArgs
    {
        public Room room;
    }

    public class PointsScoredArgs : EventArgs
    {
        public long score;
    }

    public class ScoreChangedArgs : EventArgs
    {
        public long score;
        public int multiplier;
    }

    public class MultiplierArgs : EventArgs
    {
        public bool multiplier;
    }
}