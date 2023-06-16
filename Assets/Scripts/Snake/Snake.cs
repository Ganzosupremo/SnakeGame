using Cysharp.Threading.Tasks;
using SnakeGame.AbwehrSystem;
using SnakeGame.AudioSystem;
using SnakeGame.FoodSystem;
using SnakeGame.GameUtilities;
using SnakeGame.HealthSystem;
using SnakeGame.PlayerSystem.AbilitySystem;
using SnakeGame.TimeSystem;
using SnakeGame.VisualEffects;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SnakeGame.PlayerSystem
{
    /// <summary>
    /// This class acts like an interface for other scripts
    /// </summary>
    #region Required Components
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SnakeAbilityManager))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SnakeControler))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(HealthEvent))]
    [RequireComponent(typeof(Destroy))]
    [RequireComponent(typeof(DestroyEvent))]
    [RequireComponent(typeof(MovementByVelocity))]
    [RequireComponent(typeof(MovementByVelocityEvent))]
    [RequireComponent(typeof(MovementToPositionEvent))]
    [RequireComponent(typeof(MovementToPosition))]
    [RequireComponent(typeof(Idle))]
    [RequireComponent(typeof(IdleEvent))]
    [RequireComponent(typeof(AimWeaponEvent))]
    [RequireComponent(typeof(AimWeapon))]
    [RequireComponent(typeof(SetActiveWeaponEvent))]
    [RequireComponent(typeof(ActiveWeapon))]
    [RequireComponent(typeof(FireWeaponEvent))]
    [RequireComponent(typeof(FireWeapon))]
    [RequireComponent(typeof(WeaponFiredEvent))]
    [RequireComponent(typeof(ReloadWeaponEvent))]
    [RequireComponent(typeof(ReloadWeapon))]
    [RequireComponent(typeof(WeaponReloadedEvent))]
    [RequireComponent(typeof(MaterializeEffect))]
    #endregion
    public class Snake : MonoBehaviour
    {
        #region Public Components
        [HideInInspector] public Animator animator;
        [HideInInspector] public SnakeDetailsSO snakeDetails;
        [HideInInspector] public Health health;
        [HideInInspector] public HealthEvent healthEvent;
        [HideInInspector] public Destroy destroy;
        [HideInInspector] public DestroyEvent destroyEvent;
        [HideInInspector] public MovementByVelocityEvent movementByVelocityEvent;
        [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
        [HideInInspector] public Idle idle;
        [HideInInspector] public IdleEvent idleEvent;
        [HideInInspector] public AimWeaponEvent aimWeaponEvent;
        [HideInInspector] public AimWeapon aimWeapon;
        [HideInInspector] public SetActiveWeaponEvent setActiveWeaponEvent;
        [HideInInspector] public ActiveWeapon activeWeapon;
        [HideInInspector] public FireWeaponEvent fireWeaponEvent;
        [HideInInspector] public FireWeapon fireWeapon;
        [HideInInspector] public WeaponFiredEvent weaponFiredEvent;
        [HideInInspector] public ReloadWeaponEvent reloadWeaponEvent;
        [HideInInspector] public ReloadWeapon reloadWeapon;
        [HideInInspector] public WeaponReloadedEvent weaponReloadedEvent;
        [HideInInspector] public SpriteRenderer spriteRenderer;
        [HideInInspector] public List<Weapon> weaponList = new();
        #endregion

        [SerializeField] private SoundEffectSO _LowHealthEffect;

        public List<SnakeBody> SnakeBodyList { get; private set; } = new();
        public List<Transform> SnakeSegmentsList { get; private set; } = new();
        public bool IsSnakeColliding { get; private set; }

        private int _SnakeSegmentCount;
        private MaterializeEffect _MaterializeEffect;
        private SnakeControler _SnakeControler;
        private Light2D _SnakeLight;
        private PeriodicFunction _SnakePeriodicFunc;
        private void Awake()
        {
            #region Getting Component References
            animator = GetComponent<Animator>();
            health = GetComponent<Health>();
            healthEvent = GetComponent<HealthEvent>();
            destroy = GetComponent<Destroy>();
            destroyEvent = GetComponent<DestroyEvent>();
            aimWeaponEvent = GetComponent<AimWeaponEvent>();
            aimWeapon = GetComponent<AimWeapon>();
            _SnakeControler = GetComponent<SnakeControler>();
            movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
            movementToPositionEvent = GetComponent<MovementToPositionEvent>();
            idle = GetComponent<Idle>();
            idleEvent = GetComponent<IdleEvent>();
            setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
            activeWeapon = GetComponent<ActiveWeapon>();
            fireWeaponEvent = GetComponent<FireWeaponEvent>();
            fireWeapon = GetComponent<FireWeapon>();
            weaponFiredEvent = GetComponent<WeaponFiredEvent>();
            reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
            reloadWeapon = GetComponent<ReloadWeapon>();
            weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            _MaterializeEffect = GetComponent<MaterializeEffect>();
            _SnakeLight = GetComponentInChildren<Light2D>();
            #endregion
        }

        private void Start()
        {
            AddStartingSegments();
            ChangeLightIntensity(TimeManager.Instance.CurrentTime);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.K))
            {
                GrowSnake(1);
            }
        }

        private void AddStartingSegments()
        {
            // We add more segments to the snake on start
            for (int i = 1; i < health.CurrentHealth; i++)
            {
                SnakeBody snakeBody = (SnakeBody)PoolManager.Instance.ReuseComponent(GameResources.Instance.snakeBodyPrefab.gameObject,
                this.transform.position, Quaternion.identity);
                snakeBody.gameObject.SetActive(true);
                SnakeBodyList.Add(snakeBody);

                snakeBody.transform.position = SnakeSegmentsList[_SnakeSegmentCount].position;
                SnakeSegmentsList.Add(snakeBody.transform);
                _SnakeSegmentCount++;
            }
        }

        private void OnEnable()
        {
            healthEvent.OnHealthChanged += OnHealthChanged;
            Food.OnFoodEaten += OnFoodEaten;
            TimeManager.OnTimeChanged += OnTimeChanged;
        }

        private void OnDisable()
        {
            healthEvent.OnHealthChanged -= OnHealthChanged;
            Food.OnFoodEaten -= OnFoodEaten;
            TimeManager.OnTimeChanged -= OnTimeChanged;
        }

        private void OnFoodEaten(Food food)
        {
            if (IsSnakeColliding) return;

            IsSnakeColliding = true;
            GrowSnake(food.foodSO.HealthIncrease);
        }

        private void OnTimeChanged(DayCicle dayCicle)
        {
            ChangeLightIntensity(dayCicle);
        }

        private void OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
        {
            UpdatePeriodicFunction(healthEventArgs);

            if (healthEventArgs.healthAmount <= 0)
            {
                destroyEvent.CallOnDestroy(true, 0);
                PeriodicFunction.StopAllFunc("LowHealthEffect");
                ResetSnakeSegments();
            }
        }

        private void PlayLowHealthEffect()
        {
            if (_LowHealthEffect != null)
                SoundEffectManager.CallOnSoundEffectChangedEvent(_LowHealthEffect);
        }

        /// <summary>
        /// Initialises the snake
        /// </summary>
        /// <param name="snakeDetails">The snake details to initialize</param>
        public async UniTask Initialise(SnakeDetailsSO snakeDetails)
        {
            this.snakeDetails = snakeDetails;

            SnakeSegmentsList.Add(this.transform);
            _SnakeSegmentCount = 0;

            IsSnakeColliding = false;

            SetPlayerHealth();

            CreatePlayerInitialWeapon();

            await MaterializeSnake();
        }

        /// <summary>
        /// Sets The Player Initial Weapon 
        /// </summary>
        private void CreatePlayerInitialWeapon()
        {
            weaponList.Clear();

            foreach (WeaponDetailsSO weaponDetails in snakeDetails.initialWeaponList)
            {
                //Add the weapon to the player to use
                AddWeaponToPlayer(weaponDetails);
            }
        }

        private async UniTask MaterializeSnake()
        {
            // Disable snake while the effect is in progress
            EnableSnake(false);
            
            await _MaterializeEffect.MaterializeAsync(snakeDetails.materializeShader,
                snakeDetails.materializeColor, snakeDetails.materializeTime, snakeDetails.defaultLitMaterial, spriteRenderer);
            
            // Enable the snake again
            EnableSnake(true);
        }

        private void SetPlayerHealth()
        {
            health.SetStartingHealth(snakeDetails.snakeInitialHealth);
        }

        /// <summary>
        /// Gets the player position
        /// </summary>
        /// <returns>Returns the current player position</returns>
        public Vector3 GetSnakePosition()
        {
            return transform.position;
        }

        /// <summary>
        /// Gets the snake controler script,
        /// in this script all the input is handled.
        /// </summary>
        /// <returns></returns>
        public SnakeControler GetSnakeControler()
        {
            return _SnakeControler;
        }

        public Weapon AddWeaponToPlayer(WeaponDetailsSO weaponDetails)
        {
            Weapon weapon = new()
            {
                weaponDetails = weaponDetails,
                weaponReloadTimer = 0f,
                weaponClipRemaining = weaponDetails.clipMaxCapacity,
                weaponTotalAmmoCapacity = weaponDetails.totalAmmoCapacity,
                isWeaponReloading = false
            };
            // Adds the weapon to the weapon list
            weaponList.Add(weapon);

            // Set the position of the newly added weapon in the list
            weapon.weaponListPosition = weaponList.Count;

            // Activate the newly added weapon
            setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);
            return weapon;
        }

        /// <summary>
        /// Makes the snake grows when food is eaten.
        /// </summary>
        /// <param name="increaseHealth">Can grow more than one segment if the passed int is greather than one.</param>
        private void GrowSnake(int increaseHealth)
        {
            SnakeBody snakeBody = (SnakeBody)PoolManager.Instance.ReuseComponent(GameResources.Instance.snakeBodyPrefab.gameObject,
                SnakeSegmentsList[^1].position, Quaternion.identity);

            snakeBody.gameObject.SetActive(true);
            snakeBody.transform.position = SnakeSegmentsList[_SnakeSegmentCount].position;
            
            // Add the new segment to both lists
            SnakeBodyList.Add(snakeBody);
            SnakeSegmentsList.Add(snakeBody.transform);
            _SnakeSegmentCount++;

            snakeBody.GetComponent<SpriteRenderer>().sortingOrder = -SnakeSegmentsList.Count;
            snakeBody.WaitHeadUpdateCycle(SnakeSegmentsList.Count);

            health.IncreaseHealth(increaseHealth);
            IsSnakeColliding = false;
        }

        public void UpdateSnakeSegments()
        {
            if (SnakeBodyList.Count > 0)
            {
                SnakeBodyList[0].SetTargetPosition(transform.position);

                for (int index = SnakeSegmentsList.Count - 1; index > 0; index--)
                {
                    SnakeSegmentsList[index].TryGetComponent(out SnakeBody body);
                    body.SetTargetPosition(SnakeSegmentsList[index - 1].position);
                }
            }
        }

        public void TakeOneDamage()
        {
            health.TakeDamage(1);
        }

        public void SubstractSnakeSegment()
        {
            if (SnakeSegmentsList.Count > 0 && _SnakeSegmentCount > 0)
            {
                if (!health.IsDamageable) return;

                SnakeSegmentsList[_SnakeSegmentCount].TryGetComponent(out SnakeBody snakeBody);

                snakeBody.gameObject.SetActive(false);
                SnakeSegmentsList.Remove(snakeBody.transform);
                if (SnakeBodyList.Count > 0)
                    SnakeBodyList.Remove(snakeBody);

                _SnakeSegmentCount--;

                // Reduce the multiplier when the player gets hit,
                // because the player at some point will run out of ammo
                // we do it here, so the multiplier can still be updated
                StaticEventHandler.CallMultiplierEvent(false);
                DecreaseWeaponDamage();
            }
            else
            {
                destroyEvent.CallOnDestroy(true, 0);
            }
        }

        private void ResetSnakeSegments()
        {
            for (int i = 1; i < SnakeSegmentsList.Count; i++)
            {
                SnakeSegmentsList[i].gameObject.SetActive(false);
            }

            SnakeSegmentsList.Clear();
            SnakeBodyList.Clear();
            SnakeSegmentsList.Add(this.transform);
        }

        /// <summary>
        /// Decreases the damage of the weapon when collided with something.
        /// </summary>
        private void DecreaseWeaponDamage()
        {
            Weapon currentWeapon = activeWeapon.GetCurrentWeapon();
            currentWeapon.weaponDetails.weaponCurrentAmmo.DecreaseDamage(20);
        }

        private void EnableSnake(bool isActive)
        {
            _SnakeControler.IsSnakeEnabled = isActive;
            _SnakeControler.enabled = isActive;
            fireWeapon.enabled = isActive;
        }

        public void ChangeSpriteMaterial(Material material)
        {
            spriteRenderer.material = material;
        }

        private void ChangeLightIntensity(DayCicle time)
        {
            switch (time)
            {
                case DayCicle.Morning:
                    _SnakeLight.intensity = 0f;
                    _SnakeLight.gameObject.SetActive(false);
                    break;
                case DayCicle.Afternoon:
                    _SnakeLight.gameObject.SetActive(true);
                    _SnakeLight.intensity = 0.15f;
                    break;
                case DayCicle.Evening:
                    _SnakeLight.gameObject.SetActive(true);
                    _SnakeLight.intensity = 0.35f;
                    break;
                case DayCicle.Night:
                    _SnakeLight.gameObject.SetActive(true);
                    _SnakeLight.intensity = 0.65f;
                    break;
                default:
                    break;
            }
        }

        private void UpdatePeriodicFunction(HealthEventArgs healthEventArgs)
        {
            if (healthEventArgs.healthAmount <= 3 && _LowHealthEffect != null)
            {
                //SoundEffectManager.CallOnSoundEffectChangedLoopingEvent(_LowHealthEffect, true);
                Health.CallOnLowHealthEvent();
            }
            else if (healthEventArgs.healthAmount >= 3 && _LowHealthEffect != null)
            {
                //SoundEffectManager.CallOnSoundEffectChangedLoopingEvent(_LowHealthEffect, false);
                Health.CallOnNormalHealthEvent();
            }
        }
    }
}