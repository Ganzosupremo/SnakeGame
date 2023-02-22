using System.Collections;
using UnityEngine;
using SnakeGame;
using SnakeGame.VisualEffects;
using SnakeGame.SoundsSystem;

[RequireComponent(typeof(MaterializeEffect))]
[RequireComponent(typeof(Destroy))]
[RequireComponent(typeof(DestroyEvent))]
public class Food : MonoBehaviour
{
    // The grid dimensions - deprecated
    private int gridWidth = 20;
    private int gridHeight = 20;
    private long score = 0;

    private FoodSO foodSO;
    private MaterializeEffect materializeEffect;
    private SpriteRenderer spriteRenderer;
    
    // This is the trigger collider, detects when the player comes near the food and eats it
    private CircleCollider2D triggerCollider2D;
    
    // This collider prevents the food for passing trough walls or something
    [SerializeField] private CircleCollider2D solidCollider2D;

    // The position of the food - deprecated
    int foodX = 0;
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
        score = foodSO.score;
        score += Random.Range(1, 452);

        StartCoroutine(MaterializeFood());
    }

    private IEnumerator MaterializeFood()
    {
        EnableFood(false);

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(foodSO.materializeShader, foodSO.materiliazeColor,
            foodSO.materializeTime, foodSO.defaultLitMaterial, spriteRenderer));

        EnableFood(true);
    }

    /// <summary>
    /// Was used in a previous scipt, 
    /// however this method is no longer used.
    /// </summary>
    /// <returns></returns>
    public GameObject GenerateFood()
    {
        foodX = Random.Range(0, gridWidth);
        foodY = Random.Range(0, gridHeight);
        return Instantiate(this.gameObject, new Vector2(foodX, foodY), Quaternion.identity);
    }

    private void EnableFood(bool isActive)
    {
        triggerCollider2D.enabled = isActive;
        solidCollider2D.enabled = isActive;
    }

    private void PlaySoundEffect()
    {
        if (foodSO.soundEffect == null) return;
        SoundEffectManager.Instance.PlaySoundEffect(foodSO.soundEffect);
    }

    private void DisableFood()
    {
        DestroyEvent destroyEvent = GetComponent<DestroyEvent>();
        destroyEvent.CallOnDestroy(true, score);
    }
}
