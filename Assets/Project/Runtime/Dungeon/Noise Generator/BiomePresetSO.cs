using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SnakeGame.Dungeon.NoiseGenerator
{
    [CreateAssetMenu(fileName = "BiomePreset_", menuName = "Scriptable Objects/Dungeon/NoiseMap/Biome Preset")]
    public class BiomePresetSO : ScriptableObject
    {
        public Tile[] biomeTiles;
        public float minHeight;
        public float minMoisture;
        public float minHeat;

        public Tile GetTile()
        {
            return biomeTiles[Random.Range(0, biomeTiles.Length)];
        }

        public bool MatchCondition(float height, float moisture, float heat)
        {
            return height >= minHeight && moisture >= minMoisture && heat >= minHeat;
        }
    }
}
