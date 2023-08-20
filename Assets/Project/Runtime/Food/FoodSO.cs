using SnakeGame;
using SnakeGame.AudioSystem;
using UnityEngine;

namespace SnakeGame.FoodSystem
{
    [CreateAssetMenu(fileName = "Food_", menuName = "Scriptable Objects/Food/Snake Food")]
    public class FoodSO : UniversalFood
    {
        #region Tooltip
        [Tooltip("The name for this type of food")]
        #endregion
        public string foodName;

        public SoundEffectSO SoundEffect;

        #region Materialize Effect
        [Header("Materialize Effect")]
        [Space(10)]
        #endregion

        public float materializeTime;

        public Material defaultLitMaterial;

        public Shader materializeShader;

        [ColorUsage(true, true)]
        public Color materiliazeColor;

        #region Game Scoring
        [Header("Game Scoring")]
        [Space(10)]
        [Range(10, 1000)]
        [Tooltip("The points this type of food will give to the player when eaten. " +
            "Up to 2000 points")]
        #endregion
        public long score;
    }
}