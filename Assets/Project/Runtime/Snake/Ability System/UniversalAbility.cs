using SnakeGame.AudioSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SnakeGame.PlayerSystem.AbilitySystem
{
    public class UniversalAbility : ScriptableObject
    {
        public string AbilityName = "";
        [Range(1f, 10f)]
        public float ActiveTime = 1f;
        [Range(1f, 10f)]
        public float CooldownTime = 1f;
        [Range(1, 4)]
        public int AbilityCost = 1;
        public SoundEffectSO OnAbilityActiveSound;
        internal Snake Snake;

        /// <summary>
        /// Activates the selected ability, this method is called once per activation.
        /// </summary>
        /// <param name="parent"></param>
        public virtual void Activate(GameObject parent) 
        {
            SnakeAbilityManager.CallOnAbilityActiveEvent(ActiveTime, CooldownTime);
        }

        /// <summary>
        /// This method should be called when the ability enters the cooldown state,
        /// it's just called once.
        /// </summary>
        /// <param name="parent"></param>
        public virtual void Cooldown(GameObject parent)
        {
            SnakeAbilityManager.CallOnAbilityInactiveEvent(ActiveTime, CooldownTime);
        }

        /// <summary>
        /// This method should be called when the ability needs to run some code multiple times on Update.
        /// </summary>
        /// <param name="parent"></param>
        public virtual void UpdateAbilityOnCooldown(GameObject parent)
        {
            SnakeAbilityManager.CallOnAbilityInactiveEvent(ActiveTime, CooldownTime);
        }

        public void PlaySoundEffect()
        {
            if (OnAbilityActiveSound != null)
                SoundEffectManager.CallOnSoundEffectChangedEvent(OnAbilityActiveSound);
        }

        public void InitIfNeeded(GameObject parent)
        {
            if (Snake == null && GameManager.Instance == null && parent != null)
                Snake = parent.GetComponent<Snake>();
            else if (GameManager.Instance != null)
                Snake = GameManager.Instance.GetSnake();
        }
    }
}
