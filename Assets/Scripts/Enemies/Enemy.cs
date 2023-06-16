using SnakeGame.AbwehrSystem;
using SnakeGame.AudioSystem;
using SnakeGame.Debuging;
using SnakeGame.HealthSystem;
using SnakeGame.ProceduralGenerationSystem;
using SnakeGame.TimeSystem;
using SnakeGame.VisualEffects;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SnakeGame.Enemies
{
    #region Required Components
    [RequireComponent(typeof(EnemyMovementAI))]
    [RequireComponent(typeof(EnemyWeaponAI))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(HealthEvent))]
    [RequireComponent(typeof(DestroyEvent))]
    [RequireComponent(typeof(Destroy))]
    [RequireComponent(typeof(IdleEvent))]
    [RequireComponent(typeof(Idle))]
    [RequireComponent(typeof(MaterializeEffect))]
    [RequireComponent(typeof(MovementToPositionEvent))]
    [RequireComponent(typeof(MovementToPosition))]
    [RequireComponent(typeof(SetActiveWeaponEvent))]
    [RequireComponent(typeof(ActiveWeapon))]
    [RequireComponent(typeof(AimWeaponEvent))]
    [RequireComponent(typeof(AimWeapon))]
    [RequireComponent(typeof(FireWeaponEvent))]
    [RequireComponent(typeof(FireWeapon))]
    [RequireComponent(typeof(WeaponFiredEvent))]
    [RequireComponent(typeof(ReloadWeaponEvent))]
    [RequireComponent(typeof(ReloadWeapon))]
    [RequireComponent(typeof(WeaponReloadedEvent))]
    [RequireComponent(typeof(SortingGroup))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PolygonCollider2D))]
    [RequireComponent(typeof(DealDamageOnContact))]
    [RequireComponent(typeof(ReceiveDamageOnContact))]
    #endregion
    public class Enemy : MonoBehaviour
    {
        #region Public Components
        [HideInInspector] public EnemyDetailsSO enemyDetails;
        [HideInInspector] public SpriteRenderer[] spriteRendererArray;
        [HideInInspector] public AimWeaponEvent aimWeaponEvent;
        [HideInInspector] public FireWeaponEvent fireWeaponEvent;
        [HideInInspector] public FireWeapon fireWeapon;

        [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
        [HideInInspector] public IdleEvent idleEvent;
        [HideInInspector] public EnemyMovementAI enemyMovementAI;
        #endregion

        [SerializeField] private TextMeshPro _BossNameUI;

        /// <summary>
        /// Used to get the physics shape
        /// </summary>
        private SpriteRenderer m_EnemySpriteRenderer;

        private MaterializeEffect m_MaterializeEffect;
        private PolygonCollider2D m_TriggerCollider;

        private SetActiveWeaponEvent m_SetActiveWeaponEvent;
        private HealthEvent m_HealthEvent;
        private Health m_Health;
        private Light2D m_EnemyLight;

        private void Awake()
        {
            #region Getting References
            enemyMovementAI = GetComponent<EnemyMovementAI>();
            m_HealthEvent = GetComponent<HealthEvent>();
            m_Health = GetComponent<Health>();
            movementToPositionEvent = GetComponent<MovementToPositionEvent>();

            aimWeaponEvent = GetComponent<AimWeaponEvent>();
            fireWeaponEvent = GetComponent<FireWeaponEvent>();
            fireWeapon = GetComponent<FireWeapon>();
            m_SetActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
            idleEvent = GetComponent<IdleEvent>();

            m_EnemySpriteRenderer = GetComponent<SpriteRenderer>();
            m_MaterializeEffect = GetComponent<MaterializeEffect>();
            m_TriggerCollider = GetComponent<PolygonCollider2D>();
            spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
            m_EnemyLight = GetComponentInChildren<Light2D>();
            #endregion
        }

        private void Start()
        {
            ChangeLightIntensity(TimeManager.Instance.CurrentTime);
        }

        private void OnEnable()
        {
            m_HealthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
            TimeManager.OnTimeChanged += OnTimeChanged;
        }

        private void OnDisable()
        {
            m_HealthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
            TimeManager.OnTimeChanged -= OnTimeChanged;
        }

        private void OnTimeChanged(DayCicle cicle)
        {
            ChangeLightIntensity(cicle);
        }

        private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
        {
            if (healthEventArgs.healthAmount <= 0f)
                EnemyDestroyed();
        }

        public void InitialiseEnemy(EnemyDetailsSO enemyDetails, int enemySpawnNumber, GameLevelSO gameLevel)
        {
            this.enemyDetails = enemyDetails;

            SetBossNameOnText();

            SetEnemySprite();

            SetEnemyMovementUpdateFrame(enemySpawnNumber);

            SetEnemyStartingHealth(gameLevel);

            SetEnemyStartingWeapon();

            SetPolygonColliderShape();

            MaterializeEnemy();
        }

        private void SetBossNameOnText()
        {
            if (_BossNameUI != null)
                _BossNameUI.text = $"<{enemyDetails.enemyName.ToUpper().Color("red")}>";
        }

        private void SetEnemySprite()
        {
            if (enemyDetails.EnemySprite != null)
            {
                m_EnemySpriteRenderer.sprite = enemyDetails.EnemySprite;
            }

            m_EnemySpriteRenderer.color = enemyDetails.enemyColor;
        }

        /// <summary>
        /// Set the enemy movement update frame
        /// </summary>
        private void SetEnemyMovementUpdateFrame(int enemySpawnNumber)
        {
            //Set the frame on which the enemy will process it's updates
            enemyMovementAI.UpdateFramesNumber(enemySpawnNumber % Settings.targetFramesToSpreadPathfindingOver);
        }

        /// <summary>
        /// Sets the starting health for the enemy on this specific game level
        /// </summary>
        /// <param name="gameLevel"></param>
        private void SetEnemyStartingHealth(GameLevelSO gameLevel)
        {
            foreach (ItemHealthDetails itemHealth in enemyDetails.enemyHealthDetailsArray)
            {
                if (itemHealth.gameLevel == gameLevel)
                {
                    m_Health.SetStartingHealth(itemHealth.healthAmount);
                    return;
                }
            }

            m_Health.SetStartingHealth(Settings.defaultEnemyHealth);
        }

        /// <summary>
        /// Set the enemy starting weapon with the weapon details SO
        /// </summary>
        private void SetEnemyStartingWeapon()
        {
            // Proceed if the enemy has a weapon
            if (enemyDetails.enemyWeapon != null)
            {
                Weapon weapon = new()
                {
                    weaponDetails = enemyDetails.enemyWeapon,
                    weaponReloadTimer = 0f,
                    weaponClipRemaining = enemyDetails.enemyWeapon.clipMaxCapacity,
                    weaponTotalAmmoCapacity = enemyDetails.enemyWeapon.totalAmmoCapacity,
                    isWeaponReloading = false
                };

                //Set the weapon for the enemy
                m_SetActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);
            }
        }

        private void SetPolygonColliderShape()
        {
            if (m_TriggerCollider != null)
            {
                List<Vector2> polygonVertices = new();
                m_EnemySpriteRenderer.sprite.GetPhysicsShape(0, polygonVertices);

                m_TriggerCollider.points = polygonVertices.ToArray();
            }
        }

        private async void MaterializeEnemy()
        {
            EnableEnemy(false);

            await m_MaterializeEffect.MaterializeAsync(enemyDetails.enemyMaterializeShader, enemyDetails.enemyMaterializeColor,
                enemyDetails.enemyMaterializeTime, enemyDetails.standardEnemyMaterial, m_EnemySpriteRenderer);

            EnableEnemy(true);
        }

        private void EnableEnemy(bool isEnabled)
        {
            // Enable/Disable the colliders
            m_TriggerCollider.enabled = isEnabled;

            // Enable/Disable the enemy movement AI
            enemyMovementAI.enabled = isEnabled;

            // Enable/Disable the fire weapon
            fireWeapon.enabled = isEnabled;
        }

        private void EnemyDestroyed()
        {
            if (enemyDetails.deathSoundEffect != null)
                SoundEffectManager.CallOnSoundEffectChangedEvent(enemyDetails.deathSoundEffect);

            if (TryGetComponent(out DestroyEvent destroyEvent))
                destroyEvent.CallOnDestroy(false, 0);
        }

        private void ChangeLightIntensity(DayCicle cicle)
        {
            if (m_EnemyLight == null) return;

            switch (cicle)
            {
                case DayCicle.Morning:
                    m_EnemyLight.gameObject.SetActive(false);
                    break;
                case DayCicle.Afternoon:
                    m_EnemyLight.gameObject.SetActive(true);
                    m_EnemyLight.intensity = 0.1f;
                    break;
                case DayCicle.Evening:
                    m_EnemyLight.gameObject.SetActive(true);
                    m_EnemyLight.intensity = 0.3f;
                    break;
                case DayCicle.Night:
                    m_EnemyLight.gameObject.SetActive(true);
                    m_EnemyLight.intensity = 0.6f;
                    break;
                default:
                    break;
            }
        }
    }
}