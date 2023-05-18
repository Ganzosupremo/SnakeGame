using UnityEngine;

namespace SnakeGame.FoodSystem
{
    public class UniversalFood : ScriptableObject
    {
        public int HealthIncrease = 1;
        public int DamageIncreasePercentage = 20;

        public GameObject FoodPrefab;
        public Sprite FoodSprite;
        public Sprite MinimapFoodSprite;
    }
}
