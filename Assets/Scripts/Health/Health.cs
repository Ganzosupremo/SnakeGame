using System.Collections;
using UnityEngine;

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
            //int healthIncrease = Mathf.RoundToInt(startingHealth * amountToIncrease / 100);
            //int totalHealth = currentHealth + healthIncrease;

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
    }
}