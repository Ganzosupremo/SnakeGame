using SnakeGame.HealthSystem;
using UnityEngine;

namespace SnakeGame.PlayerSystem.AbilitySystem
{
    [CreateAssetMenu(fileName = "SlowDownTimeAbility_", menuName = "Scriptable Objects/Player/Slow Down Time Ability")]
    public class SlowDownTimeSO : UniversalAbility
    {
        [Range(0.01f, 1f)]
        public float SlownDownFactor = 0.05f;

        private float m_FixedDeltaTime;

        public override void Activate(GameObject parent)
        {
            //SnakeAbilityManager.CallOnAbilityActiveEvent();
            m_FixedDeltaTime = Time.fixedDeltaTime;
            Health health = parent.GetComponent<Health>();
            health.TakeDamage(1);
            SlownDownTime();
        }

        public override void Cooldown(GameObject parent)
        {
            //SnakeAbilityManager.CallOnAbilityInactiveEvent(ActiveTime, CooldownTime);
            Time.fixedDeltaTime = m_FixedDeltaTime;
        }

        public override void UpdateAbilityOnCooldown(GameObject parent)
        {
            //SnakeAbilityManager.CallOnAbilityInactiveEvent();
            Time.timeScale += (1f / CooldownTime) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
        }

        private void SlownDownTime()
        {
            Time.timeScale = SlownDownFactor;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }
}
