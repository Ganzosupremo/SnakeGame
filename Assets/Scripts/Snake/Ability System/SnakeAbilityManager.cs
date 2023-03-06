using SnakeGame.UI;
using SnakeGame.VisualEffects;
using System;
using UnityEngine;

namespace SnakeGame.PlayerSystem.AbilitySystem
{
    public class SnakeAbilityManager : MonoBehaviour
    {
        public GameObject AbilityEffectParticleSystem;

        private UniversalAbility ability;
        private Snake snake;
        private float cooldownTime;
        private float activeTime;
        private AbilityState state = AbilityState.Ready;

        private void Awake()
        {
            snake = GetComponent<Snake>();
        }

        private void Start()
        {
            ability = snake.snakeDetails.ability;
            AbilityEffectParticleSystem = Instantiate(AbilityEffectParticleSystem, snake.transform);
            AbilityEffectParticleSystem.SetActive(false);
        }

        private void Update()
        {
            switch (state)
            {
                case AbilityState.Ready:
                    if (snake.GetSnakeControler().GetInputActions().Snake.SpecialAbility.IsPressed())
                    {
                        ability.Activate(gameObject);
                        snake.ChangeSpriteMaterial(GameResources.Instance.outlineMaterial);
                        snake.GetSnakeControler().SetSpecialAbilityBool(true);
                        state = AbilityState.Active;
                        activeTime = ability.activeTime;
                    }

                    break;
                case AbilityState.Active:
                    if (activeTime > 0f)
                    {
                        activeTime -= Time.unscaledDeltaTime;
                        ActivateParticleEffects();
                        StartCoroutine(SpecialAbilityUI.Instance.UpdateSpecialAbilityBar(activeTime / ability.activeTime));
                    }
                    else
                    {
                        ability.BeginCooldown(gameObject);
                        DeactivateParticleEffects();
                        snake.ChangeSpriteMaterial(GameResources.Instance.litMaterial);
                        snake.GetSnakeControler().SetSpecialAbilityBool(false);
                        state = AbilityState.Cooldown;
                        cooldownTime = ability.cooldownTime;
                    }

                    break;
                case AbilityState.Cooldown:
                    if (cooldownTime > 0f)
                    {
                        cooldownTime -= Time.unscaledDeltaTime;
                        StartCoroutine(SpecialAbilityUI.Instance.UpdateSpecialAbilityCooldownBar(cooldownTime / ability.cooldownTime));
                    }
                    else
                    {
                        SpecialAbilityUI.Instance.ResetSpecialAbilityBar();
                        SpecialAbilityUI.Instance.ResetSpecialAbilityCooldownBar();
                        state = AbilityState.Ready;
                    }

                    break;
                default:
                    break;
            }
        }

        private void ActivateParticleEffects()
        {
            if (AbilityEffectParticleSystem.TryGetComponent(out SpecialAbilityEffect abilityEffect))
            {
                AbilityEffectParticleSystem.SetActive(true);
                abilityEffect.EnableParticles();
            }
        }

        private void DeactivateParticleEffects()
        {
            if (AbilityEffectParticleSystem.TryGetComponent(out SpecialAbilityEffect abilityEffect))
            {
                //AbilityEffectParticleSystem.SetActive(false);
                abilityEffect.DisableParticles();
            }
        }
    }
}
