using UnityEngine;
using UnityEngine.Rendering;

namespace SnakeGame.Dungeon.NoiseGenerator
{
    public static class NoiseGenerator
    {
        /// <summary>
        /// Generates the noise
        /// </summary>
        /// <param name="width">Width of the noise map</param>
        /// <param name="height">Height of the noise map</param>
        /// <param name="scale">Overall scale so we can zoom in or out if needed</param>
        /// <param name="waves">Array of different waves to generate the noise map</param>
        /// <param name="offset">Horizontal and vertical offset if needed</param>
        /// <returns></returns>
        public static float[,] Generate(int width, int height, float scale, Wave[] waves, Vector2 offset)
        {
            float[,] noiseMap = new float[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // calculate the sample positions
                    float samplePosX = (float)x * scale + offset.x;
                    float samplePosY = (float)y * scale + offset.y;

                    float normalization = 0.0f;
                    // loop through each wave
                    foreach (Wave wave in waves)
                    {
                        // sample the perlin noise taking into consideration amplitude and frequency
                        noiseMap[x, y] += wave.amplitude * Mathf.PerlinNoise(samplePosX * wave.frequency + wave.seed, samplePosY * wave.frequency + wave.seed);
                        normalization += wave.amplitude;
                    }
                    // normalize the value
                    noiseMap[x, y] /= normalization;
                }
            }

            return noiseMap;
        }
    }

    [System.Serializable]
    public class Wave
    {
        /// <summary>
        /// The amount to offsett the noise so that 
        /// it's not sampling the same area for everything.
        /// </summary>
        [Tooltip("The amount to offsett the noise so that it's not sampling the same area for everything.")]
        public float seed;
        /// <summary>
        /// the scale of the noise map we’ll be sampling, 
        /// a higher frequency will result in a more bumpy and noisy output.
        /// </summary>
        [Tooltip("the scale of the noise map we’ll be sampling, a higher frequency will result in a more bumpy and noisy output.")]
        public float frequency;
        /// <summary>
        ///  Defines the size or intensity of the output
        /// </summary>
        [Tooltip("Defines the size or intensity of the output")]
        public float amplitude;
    }
}
