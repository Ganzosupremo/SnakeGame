namespace SnakeGame.Dungeon.NoiseGenerator
{
    public class BiomeTempData
    {
        public BiomePresetSO biome;
        public BiomeTempData(BiomePresetSO preset)
        {
            biome = preset;
        }

        public float GetDiffValue(float height, float moisture, float heat)
        {
            return (height - biome.minHeight) + (moisture - biome.minMoisture) + (heat - biome.minHeat);
        }
    }
}
