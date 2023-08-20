using UnityEngine;

namespace SnakeGame.FoodSystem
{
    public class UniversalFood : ScriptableObject
    {
        public int HealthIncrease = 1;
        [Range(0.1f, 1f)]
        public float DamageIncreasePercentage = 0.2f;

        public GameObject FoodPrefab;
        public Sprite FoodSprite;
        public Sprite MinimapFoodSprite;
    }
}
