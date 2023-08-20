using Cysharp.Threading.Tasks;
using SnakeGame.AudioSystem;
using SnakeGame.Debuging;
using SnakeGame.HealthSystem;
using SnakeGame.VisualEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SnakeGame.PlayerSystem.AbilitySystem
{
    [CreateAssetMenu(fileName = "BurstDamageAbility_", menuName = "Scriptable Objects/Player/Burst Damage")]
    public class BurstDamageSO : UniversalAbility
    {
        [Header("References")]
        public float DamageRadius;
        public LayerMask ObjectsToDamage;
        public int Damage;
        public bool DebugActive;

        [Space]
        [Header("Visual Effect")]
        public AmmoHitEffectSO BurtsHitEffect;

        public void DebugA()
        {
            if (!DebugActive) return;
            InitIfNeeded(null);
            Gizmos.DrawWireSphere(Snake.transform.position, DamageRadius);
        }

        public override async void Activate(GameObject parent)
        {
            InitIfNeeded(parent);

            bool trailNotNull = IsBurstTrailNull();

            if (trailNotNull)
                Snake.BurstAbilityTrail.gameObject.SetActive(true);

            PlaySoundEffect();

            Health snakeHealth = parent.GetComponent<Health>();
            snakeHealth.TakeDamage(AbilityCost);

            Collider2D[] damagedObjects = Physics2D.OverlapCircleAll(parent.transform.position, DamageRadius, ObjectsToDamage);

            for (int i = 0; i < damagedObjects.Length; i++)
            {
                if (trailNotNull)
                {

                    await MoveTrailRenderer(damagedObjects[i]);

                    SetVisualEffect(damagedObjects[i].transform.position);

                    ResetTrailTransform();
                }

                if (damagedObjects[i].TryGetComponent(out Health health))
                {
                    health.TakeDamage(Damage);
                }
            }
        }

        private void SetVisualEffect(Vector3 effectPosition)
        {
            AmmoHitEffect ammoHit = (AmmoHitEffect)PoolManager.Instance.ReuseComponent(BurtsHitEffect.ammoHitEffectPrefab, effectPosition);
            ammoHit.InitialiseAmmoHitEffect(BurtsHitEffect);
            ammoHit.gameObject.SetActive(true);
        }

        private async UniTask MoveTrailRenderer(Collider2D collider2D)
        {
            float time = 0f;
            Vector3 spawnPosition = Snake.BurstAbilityTrail.transform.position;

            while (time < 1f)
            {
                Snake.BurstAbilityTrail.transform.position = Vector3.Lerp(spawnPosition, collider2D.transform.position, time);
                time += Time.deltaTime / Snake.BurstAbilityTrail.time;

                await UniTask.NextFrame();
            }
        }

        private void ResetTrailTransform()
        {
            Snake.BurstAbilityTrail.transform.position = Snake.transform.position;
        }

        private bool IsBurstTrailNull()
        {
            if (Snake.BurstAbilityTrail != null) return true;

            return false;
        }

        public override void Cooldown(GameObject parent)
        {
            InitIfNeeded(parent);

            Snake.BurstAbilityTrail.gameObject.SetActive(false);

            this.Log($"Ability on Cooldown {CooldownTime}.");
        }
    }
}
