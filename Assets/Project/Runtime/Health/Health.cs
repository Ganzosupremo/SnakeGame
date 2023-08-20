using System.Collections;
using UnityEngine;
using SnakeGame.Enemies;
using SnakeGame.PlayerSystem;
using Cysharp.Threading.Tasks;
using SnakeGame.AudioSystem;
using SnakeGame.GameUtilities;
using System;
using SnakeGame.Debuging;

namespace SnakeGame.HealthSystem
{
    [DisallowMultipleComponent]
    public class Health : MonoBehaviour
    {
        public static event Action OnLowHealth;
        public static event Action OnNormalHealth;

        [Tooltip("Here goes the health bar prefab for the enemies")]
        [SerializeField] private HealthBar healthBar;

        public int CurrentHealth { get; private set; }

        private int startingHealth;
        private HealthEvent healthEvent;
        private Snake snake;
        private Coroutine immunityCoroutine;
        private bool immuneAfterHit = false;
        private float immunityTime;
        private SpriteRenderer spriteRenderer = null;
        private const float spriteBlinkInterval = 0.3f;
        private WaitForSeconds waitForSecondsInterval = new(spriteBlinkInterval);

        [HideInInspector] public Enemy enemy;
        public bool IsDamageable { get; private set; }

        private void Awake()
        {
            healthEvent = GetComponent<HealthEvent>();
            IsDamageable = true;
        }

        private void Start()
        {
            CallHealthEvent(0);

            TryGetComponent(out enemy);
            TryGetComponent(out snake);

            if (snake != null && snake.snakeDetails.isImmuneAfterHit)
            {
                immuneAfterHit = true;
                immunityTime = snake.snakeDetails.immunityTime;
                spriteRenderer = snake.spriteRenderer;
            }
            else if (enemy != null && enemy.enemyDetails.immuneAfterHit)
            {
                immuneAfterHit = true;
                immunityTime = enemy.enemyDetails.immunityTime;
                spriteRenderer = enemy.spriteRendererArray[0];
            }

            if (enemy != null && enemy.enemyDetails.isHealthBarDisplayed && healthBar != null)
            {
                healthBar.EnableHealthBar();
                
                if (enemy.enemyDetails.IsBoss) 
                    healthBar.SetBossName($"<{enemy.enemyDetails.enemyName}>".Color("red"));
            }
            else if (healthBar = null)
            {
                healthBar.DisableHealthBar();
                healthBar.SetBossName(string.Empty);
            }
        }

        /// <summary>
        /// Sets the starting health of an item
        /// </summary>
        /// <param name="startingHealth">The initial health amount</param>
        public void SetStartingHealth(int startingHealth)
        {
            this.startingHealth = startingHealth;
            CurrentHealth = startingHealth;
        }

        public void TakeDamage(int damage)
        {
            if (!IsDamageable) return;
            
            CurrentHealth -= damage;
            CallHealthEvent(damage);

            PostHitImmunity(damage);

            if (healthBar != null)
                healthBar.SetHealthBarValue((float)CurrentHealth / (float)startingHealth);
        }

        /// <summary>
        /// Increases the Health with the amount specified
        /// </summary>
        /// <param name="amountToIncrease">The amount to increase the health to.</param>
        public void IncreaseHealth(int amountToIncrease)
        {
            CurrentHealth += amountToIncrease;

            CallHealthEvent(0);
        }

        public void CallHealthEvent(int damageAmount)
        {
            healthEvent.CallOnHealthChanged((float)CurrentHealth / (float)startingHealth, CurrentHealth, damageAmount);
        }

        /// <summary>
        /// Give immunity after hit
        /// </summary>
        private void PostHitImmunity(int damageTaken = 0)
        {
            // Dont give immunity to deactivated gameobjects
            if (!gameObject.activeSelf) return;

            if (immuneAfterHit)
            {
                if (immunityCoroutine != null)
                    StopCoroutine(immunityCoroutine);

                if (snake != null)
                    snake.SubstractSnakeSegment(damageTaken);

                //await InmunityAsync(immunityTime, spriteRenderer);
                immunityCoroutine = StartCoroutine(ImmunityRoutine(immunityTime, spriteRenderer));
            }
        }

        private IEnumerator ImmunityRoutine(float immunityTime, SpriteRenderer spriteRenderer)
        {
            int blinkIterations = Mathf.RoundToInt(immunityTime / spriteBlinkInterval / 2f);

            Color originalColor = spriteRenderer.color;

            IsDamageable = false;

            while (blinkIterations > 0)
            {
                spriteRenderer.color = Color.red;
                yield return waitForSecondsInterval;
                spriteRenderer.color = originalColor;
                yield return waitForSecondsInterval;
                blinkIterations--;
                yield return null;
            }

            spriteRenderer.color = originalColor;
            IsDamageable = true;
        }

        private async UniTask InmunityAsync(float inmunityTime, SpriteRenderer spriteRenderer)
        {
            int blinkIterations = Mathf.RoundToInt(inmunityTime / spriteBlinkInterval / 2f);

            Color originalColor = spriteRenderer.color;
            IsDamageable = false;

            while (blinkIterations > 0)
            {
                spriteRenderer.color = Color.red;
                await UniTask.Delay((int)spriteBlinkInterval * 1000);

                spriteRenderer.color = originalColor;
                await UniTask.Delay((int)blinkIterations * 1000);
                blinkIterations--;
            }

            spriteRenderer.color = originalColor;
            IsDamageable = true;
        }

        /// <summary>
        /// Sets the <see cref="IsDamageable"/> property to the passed bool.
        /// NOTE: It does so in the inverse way, so for example, if the passed bool
        ///  is false, then the <see cref="IsDamageable"/> property will be true. If the passed 
        ///  bool is true the <see cref="IsDamageable"/> property will be false.
        ///  When the <see cref="IsDamageable"/> property is false, means that this 
        ///  object is invincible to any damage, if it's true, that means that 
        ///  this object can receive any damage.
        /// </summary>
        /// <param name="isDamageable"></param>
        public void SetIsDamageable(bool isDamageable)
        {
            IsDamageable = !isDamageable;
        }

        public static void CallOnLowHealthEvent()
        {
            OnLowHealth?.Invoke();
        }

        public static void CallOnNormalHealthEvent()
        {
            OnNormalHealth?.Invoke();
        }
    }
}