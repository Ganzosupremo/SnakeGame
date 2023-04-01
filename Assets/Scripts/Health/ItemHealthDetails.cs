using SnakeGame.ProceduralGenerationSystem;
using UnityEngine;

namespace SnakeGame
{
    /// <summary>
    /// This struct define the amount of health an item can have
    /// for the current game level.
    /// </summary>
    [System.Serializable]
    public struct ItemHealthDetails
    {
        public GameLevelSO gameLevel;
        public int healthAmount;
        /// <summary>
        /// The health amount will change based on the difficulty,
        /// so to reset the health amount to it's original value, this variable will be used.
        /// </summary>
        [Tooltip("The health amount will change based on the difficulty, " +
            "so to reset the health amount to it's original value, this variable will be used")]
        public int defaultHealthAmount;
    }
}