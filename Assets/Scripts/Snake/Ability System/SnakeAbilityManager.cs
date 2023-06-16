using SnakeGame.Debuging;
using SnakeGame.UI;
using SnakeGame.VisualEffects;
using System;
using UnityEngine;

namespace SnakeGame.PlayerSystem.AbilitySystem
{
    public class SnakeAbilityManager : MonoBehaviour
    {
        public GameObject AbilityEffectParticleSystem;

        public static event Action OnAbilityActive;
        public static event Action<SnakeAbilityEventArgs> OnAbilityInactive;

        private UniversalAbility m_Ability;
        private Snake m_Snake;
        private float m_CooldownTime;
        private float m_ActiveTime;
        private AbilityState m_State = AbilityState.Ready;

        private void Awake()
        {
            m_Snake = GetComponent<Snake>();
        }

        private void Start()
        {
            // Choose a special ability at random
            m_Snake.snakeDetails.ability = m_Snake.snakeDetails.AbilitiesArray[UnityEngine.Random.Range(0, m_Snake.snakeDetails.AbilitiesArray.Length)];
            
            m_Ability = m_Snake.snakeDetails.ability;
            AbilityEffectParticleSystem = Instantiate(AbilityEffectParticleSystem, m_Snake.transform);
            AbilityEffectParticleSystem.SetActive(false);
        }

        private void Update()
        {
            HandleAbilityState();
        }

        private void HandleAbilityState()
        {
            switch (m_State)
            {
                case AbilityState.Ready:

                    if (m_Snake.GetSnakeControler().GetInputActions().Snake.SpecialAbility.IsPressed())
                        ActivateAbility();

                    break;
                case AbilityState.Active:

                    AbilityOnActiveTime();

                    break;
                case AbilityState.Cooldown:

                    AbilityOnCooldownTime();

                    break;
                default:
                    break;
            }
        }



        private void ActivateAbility()
        {
            CallOnAbilityActiveEvent();
            m_Ability.Activate(gameObject);
            m_Snake.ChangeSpriteMaterial(GameResources.Instance.outlineMaterial);
            m_Snake.GetSnakeControler().SetSpecialAbilityBool(true);
            m_ActiveTime = m_Ability.ActiveTime;
            m_State = AbilityState.Active;
        }

        private void AbilityOnActiveTime()
        {
            if (m_ActiveTime > 0f)
            {
                m_ActiveTime -= Time.unscaledDeltaTime;
                //ActivateParticleEffects();
                StartCoroutine(SpecialAbilityUI.Instance.UpdateSpecialAbilityBar(m_ActiveTime / m_Ability.ActiveTime));
            }
            else
            {
                CallOnAbilityInactiveEvent(m_ActiveTime, m_CooldownTime);
                m_Ability.Cooldown(gameObject);
                //DeactivateParticleEffects();
                m_Snake.ChangeSpriteMaterial(GameResources.Instance.litMaterial);
                m_Snake.GetSnakeControler().SetSpecialAbilityBool(false);
                m_CooldownTime = m_Ability.CooldownTime;
                m_State = AbilityState.Cooldown;
            }
        }

        private void AbilityOnCooldownTime()
        {
            if (m_CooldownTime > 0f)
            {
                m_CooldownTime -= Time.unscaledDeltaTime;
                m_Ability.UpdateAbilityOnCooldown(gameObject);
                StartCoroutine(SpecialAbilityUI.Instance.UpdateSpecialAbilityCooldownBar(m_CooldownTime / m_Ability.CooldownTime));
            }
            else
            {
                SpecialAbilityUI.Instance.ResetSpecialAbilityBar();
                SpecialAbilityUI.Instance.ResetSpecialAbilityCooldownBar();
                m_State = AbilityState.Ready;
            }
        }

        public static void CallOnAbilityActiveEvent()
        {
            OnAbilityActive?.Invoke();
        }

        public static void CallOnAbilityInactiveEvent(float activeTime, float cooldownTime)
        {
            OnAbilityInactive?.Invoke(new SnakeAbilityEventArgs
            {
                ActiveTime = activeTime,
                CooldownTime = cooldownTime,
            });
        }

        private void ActivateParticleEffects()
        {
            if (AbilityEffectParticleSystem.TryGetComponent(out SpecialAbilityEffect abilityEffect))
            {
                abilityEffect.EnableParticles();
            }
        }

        private void DeactivateParticleEffects()
        {
            if (AbilityEffectParticleSystem.TryGetComponent(out SpecialAbilityEffect abilityEffect))
            {
                abilityEffect.DisableParticles();
            }
        }
    }

    public class SnakeAbilityEventArgs : EventArgs
    {
        public float ActiveTime;
        public float CooldownTime; 
    }
}
