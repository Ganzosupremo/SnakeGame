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

        public static event Action<SnakeAbilityEventArgs> OnAbilityActive;
        public static event Action<SnakeAbilityEventArgs> OnAbilityInactive;

        private UniversalAbility _ability;
        private Snake _snake;
        private float _cooldownTime;
        private float _activeTime;
        private AbilityState _state = AbilityState.Ready;

        private void Awake()
        {
            _snake = GetComponent<Snake>();
        }

        private void Start()
        {
            //// Choose a special ability at random
            //_snake.snakeDetails.ability = _snake.snakeDetails.AbilitiesArray[UnityEngine.Random.Range(0, _snake.snakeDetails.AbilitiesArray.Length)];
            
            _ability = _snake.snakeDetails.ability;
            //AbilityEffectParticleSystem = Instantiate(AbilityEffectParticleSystem, _snake.transform);
            //AbilityEffectParticleSystem.SetActive(false);
        }

        private void Update()
        {
            HandleAbilityState();
        }

        private void OnDrawGizmos()
        {
            if (_ability != null)
            {
                BurstDamageSO burstDamage = (BurstDamageSO)_ability;

                burstDamage.DebugA();
            }
        }

        private void HandleAbilityState()
        {
            switch (_state)
            {
                case AbilityState.Ready:

                    if (_snake.GetSnakeControler().IsSnakeEnabled && _snake.GetSnakeControler().GetInputActions().Snake.SpecialAbility.IsPressed() && !GameManager.Instance.IsFading)
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
            _activeTime = _ability.ActiveTime;
            CallOnAbilityActiveEvent(_activeTime, _ability.CooldownTime);
            _ability.Activate(gameObject);
            _snake.ChangeSpriteMaterial(GameResources.Instance.outlineMaterial);
            _snake.GetSnakeControler().SetSpecialAbilityBool(true);
            _state = AbilityState.Active;
        }

        private void AbilityOnActiveTime()
        {
            if (_activeTime > 0f)
            {
                _activeTime -= Time.unscaledDeltaTime;
                //ActivateParticleEffects();
                StartCoroutine(SpecialAbilityUI.Instance.UpdateSpecialAbilityBar(_activeTime / _ability.ActiveTime));
            }
            else
            {
                CallOnAbilityInactiveEvent(_activeTime, _cooldownTime);
                _ability.Cooldown(gameObject);
                //DeactivateParticleEffects();
                _snake.ChangeSpriteMaterial(GameResources.Instance.litMaterial);
                _snake.GetSnakeControler().SetSpecialAbilityBool(false);
                _cooldownTime = _ability.CooldownTime;
                _state = AbilityState.Cooldown;
            }
        }

        private void AbilityOnCooldownTime()
        {
            if (_cooldownTime > 0f)
            {
                _cooldownTime -= Time.unscaledDeltaTime;
                _ability.UpdateAbilityOnCooldown(gameObject);
                StartCoroutine(SpecialAbilityUI.Instance.UpdateSpecialAbilityCooldownBar(_cooldownTime / _ability.CooldownTime));
            }
            else
            {
                SpecialAbilityUI.Instance.ResetSpecialAbilityBar();
                SpecialAbilityUI.Instance.ResetSpecialAbilityCooldownBar();
                _state = AbilityState.Ready;
            }
        }

        public static void CallOnAbilityActiveEvent(float activeTime = 1f, float cooldownTime = 1f)
        {
            OnAbilityActive?.Invoke(new SnakeAbilityEventArgs
            {
                ActiveTime = activeTime,
                CooldownTime = cooldownTime,
            });
        }

        public static void CallOnAbilityInactiveEvent(float activeTime, float cooldownTime)
        {
            OnAbilityInactive?.Invoke(new SnakeAbilityEventArgs
            {
                ActiveTime = activeTime,
                CooldownTime = cooldownTime,
            });
        }

        //private void ActivateParticleEffects()
        //{
        //    if (AbilityEffectParticleSystem.TryGetComponent(out SpecialAbilityEffect abilityEffect))
        //    {
        //        abilityEffect.EnableParticles();
        //    }
        //}

        //private void DeactivateParticleEffects()
        //{
        //    if (AbilityEffectParticleSystem.TryGetComponent(out SpecialAbilityEffect abilityEffect))
        //    {
        //        abilityEffect.DisableParticles();
        //    }
        //}
    }

    public class SnakeAbilityEventArgs : EventArgs
    {
        public float ActiveTime;
        public float CooldownTime;
        public AbilityState State;
    }
}
