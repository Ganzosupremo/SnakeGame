using System.Collections;
using UnityEngine;
using SnakeGame.Enemies;
using SnakeGame.PlayerSystem;

namespace SnakeGame
{
    [DisallowMultipleComponent]
    public class Health : MonoBehaviour
    {
        [Tooltip("Here goes the health bar prefab on the enemies")]
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
            }
            else if (healthBar = null)
            {
                healthBar.DisableHealthBar();
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
            if (IsDamageable)
            {
                CurrentHealth -= damage;
                CallHealthEvent(damage);

                PostHitImmunity();
            }

            if (healthBar != null)
            {
                healthBar.SetHealthBarValue((float)CurrentHealth / (float)startingHealth);
            }
        }

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
        private void PostHitImmunity()
        {
            // Dont give immunity to deactivated gameobjects
            if (!gameObject.activeSelf) return;

            if (immuneAfterHit)
            {
                if (immunityCoroutine != null)
                    StopCoroutine(immunityCoroutine);

                if (snake != null)
                    snake.SubstractSnakeSegment();

                immunityCoroutine = StartCoroutine(ImmunityRoutine(immunityTime, spriteRenderer));
            }
        }

        private IEnumerator ImmunityRoutine(float immunityTime, SpriteRenderer spriteRenderer)
        {
            int blinkIterations = Mathf.RoundToInt(immunityTime / spriteBlinkInterval / 2f);

            IsDamageable = false;

            while (blinkIterations > 0)
            {
                spriteRenderer.color = Color.red;
                yield return waitForSecondsInterval;
                spriteRenderer.color = Color.white;
                yield return waitForSecondsInterval;
                blinkIterations--;
                yield return null;
            }

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
    }
}