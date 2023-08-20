using SnakeGame.AudioSystem;
using SnakeGame.HealthSystem;
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
            InitIfNeeded(parent);

            PlaySoundEffect();

            actualMinVelocity = Snake.GetSnakeControler().GetMoveSpeed();
            actualMaxVelocity = Snake.GetSnakeControler().GetMoveSpeed();

            Health health = parent.GetComponent<Health>();
            health.TakeDamage(AbilityCost);

            Snake.GetSnakeControler().SetMovementVelocity(MinVelocity, MaxVelocity);
            health.SetIsDamageable(IsInvincible);
        }

        public override void Cooldown(GameObject parent)
        {
            InitIfNeeded(parent);

            Health health = parent.GetComponent<Health>();

            Snake.GetSnakeControler().SetMovementVelocity(actualMinVelocity, actualMaxVelocity);
            health.SetIsDamageable(!IsInvincible);
        }
    }
}
