using System.Collections;
using UnityEngine;
using SnakeGame.VisualEffects;
using SnakeGame.SoundsSystem;
using System;

namespace SnakeGame.Foods
{
    [RequireComponent(typeof(MaterializeEffect))]
    [RequireComponent(typeof(Destroy))]
    [RequireComponent(typeof(DestroyEvent))]
    public class Food : MonoBehaviour
    {
        // The grid dimensions - deprecated
        [Obsolete]
        private int gridWidth = 20;
        [Obsolete]
        private int gridHeight = 20;

        [HideInInspector] public FoodSO foodSO;
        private MaterializeEffect materializeEffect;
        private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer minimapSpriteRenderer;
        private Sprite foodSprite;
        private long score = 0;

        // This is the trigger collider, detects when the player comes near the food and eats it
        private CircleCollider2D triggerCollider2D;
        // This collider prevents the food for passing trough walls or something
        [SerializeField] private CircleCollider2D solidCollider2D;

        // The position of the food
        [Obsolete]
        int foodX = 0;
        [Obsolete]
        int foodY = 0;

        private void Awake()
        {
            materializeEffect = GetComponent<MaterializeEffect>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            triggerCollider2D = GetComponent<CircleCollider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Settings.playerTag))
            {
                PlaySoundEffect();

                DisableFood();
            }
        }

        public void InitializeFood(FoodSO foodSO, GameLevelSO gameLevel)
        {
            this.foodSO = foodSO;

            foodSprite = foodSO.FoodSprite;
            spriteRenderer.sprite = foodSprite;
            minimapSpriteRenderer.sprite = this.foodSO.MinimapFoodSprite;

            score = foodSO.score;

            StartCoroutine(MaterializeFood());
        }

        private IEnumerator MaterializeFood()
        {
            EnableFood(false);

            yield return StartCoroutine(materializeEffect.MaterializeRoutine(foodSO.materializeShader, foodSO.materiliazeColor,
                foodSO.materializeTime, foodSO.defaultLitMaterial, spriteRenderer));

            EnableFood(true);
        }

        [Obsolete("Was an early test, but it's not used anymore.")]
        public GameObject GenerateFood()
        {
            foodX = UnityEngine.Random.Range(0, gridWidth);
            foodY = UnityEngine.Random.Range(0, gridHeight);
            return Instantiate(this.gameObject, new Vector2(foodX, foodY), Quaternion.identity);
        }

        private void EnableFood(bool isActive)
        {
            triggerCollider2D.enabled = isActive;
            solidCollider2D.enabled = isActive;
        }

        private void PlaySoundEffect()
        {
            if (foodSO.SoundEffect == null) return;
            SoundEffectManager.Instance.PlaySoundEffect(foodSO.SoundEffect);
        }

        private void DisableFood()
        {
            DestroyEvent destroyEvent = GetComponent<DestroyEvent>();
            destroyEvent.CallOnDestroy(true, score);
        }
    }
}