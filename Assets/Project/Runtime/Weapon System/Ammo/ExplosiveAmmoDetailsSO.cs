using SnakeGame.AbwehrSystem.Ammo;
using SnakeGame.FoodSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame
{
    [CreateAssetMenu(fileName = "ExplosiveAmmoDetails_", menuName = "Scriptable Objects/Weapon System/Explosive Ammo Details")]
    public class ExplosiveAmmoDetailsSO : BaseAmmoSO
    {
        [Header("Explosion Ammo Settings")]
        [Space(5)]
        public float ExplosionRadius = 1f;
        public LayerMask ExplosionMask;

        protected override void OnEnable()
        {
            base.OnEnable();
            Food.OnFoodEaten += Food_OnFoodEaten;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Food.OnFoodEaten -= Food_OnFoodEaten;
        }

        private void Food_OnFoodEaten(Food food)
        {
            BaseAmmoSO currentAmmo = GameManager.Instance.GetSnake().activeWeapon.GetCurrentAmmo();
            // Find a way to increase the damage of only the currently equiped weapon and not every single weapon
            // I think this works
            if (isPlayerAmmo && currentAmmo == this)
                IncreaseDamage(food.foodSO.DamageIncreasePercentage);
        }
    }
}
