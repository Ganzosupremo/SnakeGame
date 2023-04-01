using SnakeGame.UI;
using UnityEngine;

namespace SnakeGame.PlayerSystem.AbilitySystem
{
    [CreateAssetMenu(fileName = "FlashAbility_", menuName = "Scriptable Objects/Player/Flash Ability")]
    public class FlashSO : UniversalAbility
    {
        [Range(1f, 20f)]
        public float MinVelocity;
        [Range(1f, 20f)]
        public float MaxVelocity;
        public bool IsInvincible = true;

        private float actualMinVelocity;
        private float actualMaxVelocity;

        public override void Activate(GameObject parent)
        {
            Snake snake = parent.GetComponent<Snake>();
            actualMinVelocity = snake.GetSnakeControler().GetMoveSpeed();
            actualMaxVelocity = snake.GetSnakeControler().GetMoveSpeed();

            Health health = parent.GetComponent<Health>();
            health.TakeDamage(1);
            
            snake.GetSnakeControler().SetMovementVelocity(MinVelocity, MaxVelocity);
            health.SetIsDamageable(IsInvincible);
        }

        public override void Cooldown(GameObject parent)
        {
            Snake snake = parent.GetComponent<Snake>();
            Health health = parent.GetComponent<Health>();

            snake.GetSnakeControler().SetMovementVelocity(actualMinVelocity, actualMaxVelocity);
            health.SetIsDamageable(!IsInvincible);
        }
    }
}
