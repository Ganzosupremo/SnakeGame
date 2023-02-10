using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SnakeGame.Dungeon.NoiseGenerator.Tests
{
    public class NoiseGenerationTEst : MonoBehaviour
    {
        public BiomePresetSO[] biomes;
        public int width = 50;
        public int height = 50;
        public float scale = 1.0f;
        public Vector2 offset;

        public Tilemap groundTilemap;
        public Vector2Int lowerBounds;

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

        private Grid grid;

        private void Awake()
        {
            grid = GetComponentInChildren<Grid>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                GenerateMap((Vector3Int)lowerBounds);
            }
        }

        private void GenerateMap(Vector3Int startPosition)
        {
            // height map
            heightMap = NoiseGenerator.Generate(width, height, scale, heightWaves, offset);
            // moisture map
            moistureMap = NoiseGenerator.Generate(width, height, scale, moistureWaves, offset);
            // heat map
            heatMap = NoiseGenerator.Generate(width, height, scale, heatWaves, offset);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (groundTilemap != null)
                    {
                        Tile biomeTile = GetBiome(heightMap[x, y], moistureMap[x, y], heatMap[x, y]).GetTile();
                        Vector3 worldPos = groundTilemap.CellToWorld(startPosition);
                        groundTilemap.SetTile(new Vector3Int((int)(worldPos.x + x + 1), (int)(worldPos.y - y + 1), 0), biomeTile);
                    }
                }
            }
        }

        private BiomePresetSO GetBiome(float height, float moisture, float heat)
        {
            List<BiomeTempData> biomeTemp = new();
            foreach (BiomePresetSO biome in biomes)
            {
                if (biome.MatchCondition(height, moisture, heat))
                {
                    biomeTemp.Add(new BiomeTempData(biome));
                }
            }

            float curVal = 0.0f;
            BiomePresetSO biomeToReturn = null;
            foreach (BiomeTempData biome in biomeTemp)
            {
                if (biomeToReturn == null)
                {
                    biomeToReturn = biome.biome;
                    curVal = biome.GetDiffValue(height, moisture, heat);
                }
                else
                {
                    if (biome.GetDiffValue(height, moisture, heat) < curVal)
                    {
                        biomeToReturn = biome.biome;
                        curVal = biome.GetDiffValue(height, moisture, heat);
                    }
                }
            }

            if (biomeToReturn == null)
                biomeToReturn = biomes[0];

            return biomeToReturn;
        }
    }
}
