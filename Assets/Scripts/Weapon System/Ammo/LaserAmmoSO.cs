using SnakeGame.FoodSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame.AbwehrSystem.Ammo
{
    [CreateAssetMenu(fileName = "LaserAmmoDetails_", menuName = "Scriptable Objects/Weapon System/Laser Ammo Details")]
    public class LaserAmmoSO : BaseAmmoSO
    {
        [Space(5)]
        [Header("Laser Ammo Setttings")]
        [Tooltip("Defines the layers this ammo should hit.")]
        public LayerMask HitMask;

        //public bool HasLineRenderer;

        //public Material LineRendererMaterial;

        //[Range(0.1f, 2f)]
        //public float LineRendererStartWidth;
        //[Range(0.1f, 2f)]
        //public float LineRendererEndWidth;

        //public Gradient LineGradient;

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
