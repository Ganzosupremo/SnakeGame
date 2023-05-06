using SnakeGame.AudioSystem;
using SnakeGame.Foods;
using SnakeGame.GameUtilities;
using UnityEngine;

namespace SnakeGame.AbwehrSystem.Ammo
{
    [CreateAssetMenu(fileName = "AmmoDetails_", menuName = "Scriptable Objects/Weapon System/Ammo Details")]
    public class AmmoDetailsSO : BaseAmmoSO
    {
        private void OnEnable()
        {
            ammoDamage = originalAmmoDamage;
            Food.OnFoodEaten += Food_OnFoodEaten;
        }

        private void OnDisable()
        {
            ammoDamage = originalAmmoDamage;
            Food.OnFoodEaten -= Food_OnFoodEaten;
        }

        private void Food_OnFoodEaten(Food food)
        {
            BaseAmmoSO currentAmmo = GameManager.Instance.GetSnake().activeWeapon.GetCurrentAmmo();
            // Find a way to increase the damage of only the currently equiped weapon and not every single weapon
            if (isPlayerAmmo && currentAmmo == this)
                IncreaseDamage(food.foodSO.DamageIncreasePercentage);
        }
    }
}