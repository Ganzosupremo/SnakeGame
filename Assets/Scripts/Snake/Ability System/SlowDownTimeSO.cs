using UnityEngine;

namespace SnakeGame.PlayerSystem.AbilitySystem
{
    [CreateAssetMenu(fileName = "SlowDownTimeAbility_", menuName = "Scriptable Objects/Player/Slow Down Time Ability")]
    public class SlowDownTimeSO : UniversalAbility
    {
        [Range(0f, 1f)]
        public float newTimeValue;

        public override void Activate(GameObject parent)
        {
            Health health = parent.GetComponent<Health>();
            health.TakeDamage(1);

            Time.timeScale = newTimeValue;
        }

        public override void BeginCooldown(GameObject parent)
        {
            Time.timeScale = 1f;
        }
    }
}
