using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SnakeGame.Dungeon.NoiseGenerator
{
    public class NoiseMap : MonoBehaviour
    {
        #region Header
        [Header("Map Settings")]
        #endregion
        public BiomePresetSO[] biomes;
        /// <summary>
        /// For the moment keep the map presets here, but in the future
        /// the GameManager will keep them and keep track of the current map.
        /// </summary>
        public MapPresetSO mapPreset;

        #region Header
        [Space(10)]
        [Header("Dimensions")]
        #endregion
        public int width = 50;
        public int height = 50;
        public float scale = 1.0f;
        public Vector2 offset;

        public Tilemap groundTilemap;
        public Vector2Int startPosition;

        public void GenerateMap(MapPresetSO mapPreset)
        {
            // Specify the number of tiles beyond the width and height
            // to generate the noise maps to avoid the player seeing black voids
            // at the end of the map.
            int extra = 50;
            int extendedWidth = width + extra * 2;
            int extendedHeight = height + extra * 2;
            //Vector2 extendedOffset = offset + new Vector2(-extra, -extra);

            // height map
            mapPreset.heightMap = NoiseGenerator.Generate(width, height, scale, mapPreset.heightWaves, offset);
            // moisture map
            mapPreset.moistureMap = NoiseGenerator.Generate(width, height, scale, mapPreset.moistureWaves, offset);
            // heat map
            mapPreset.heatMap = NoiseGenerator.Generate(width, height, scale, mapPreset.heatWaves, offset);
            
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    if (groundTilemap == null) return;

                    Vector3Int cellPosition;
                    if (startPosition != Vector2Int.zero)
                    {
                        Vector3 startCellPosition = groundTilemap.WorldToCell((Vector3Int)startPosition);
                        cellPosition = new((int)startCellPosition.x + x, (int)startCellPosition.y + y, 0);
                    }
                    else
                    {
                        cellPosition = new(x + extendedWidth, y + extendedHeight, 0);
                    }

                    Tile biomeTile = GetBiome(mapPreset.heightMap[x, y], mapPreset.moistureMap[x, y], mapPreset.heatMap[x, y]).GetTile();
                    groundTilemap.SetTile(cellPosition, biomeTile);
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
