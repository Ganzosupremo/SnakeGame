using Cysharp.Threading.Tasks;
using SnakeGame.AbwehrSystem;
using SnakeGame.Foods;
using SnakeGame.PlayerSystem.AbilitySystem;
using SnakeGame.TimeSystem;
using SnakeGame.VisualEffects;
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
        #region Components
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
        public List<SnakeBody> SnakeBodyList { get; private set; } = new();
        public List<Transform> SnakeSegmentsList { get; private set; } = new();
        public bool IsSnakeColliding { get; private set; }

        private int snakeSegmentCount;
        private MaterializeEffect materializeEffect;
        private SnakeControler snakeControler;
        private Light2D snakeLight;

        private void Awake()
        {
            #region Getting Component References
            animator = GetComponent<Animator>();
            health = GetComponent<Health>();
            healthEvent = GetComponent<HealthEvent>();
            destroy = GetComponent<Destroy>();
            destroyEvent = GetComponent<DestroyEvent>();
            aimWeaponEvent = GetComponent<AimWeaponEvent>();
            snakeControler = GetComponent<SnakeControler>();
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
            materializeEffect = GetComponent<MaterializeEffect>();
            snakeLight = GetComponentInChildren<Light2D>();
            #endregion
        }

        private void Start()
        {
            AddStartingSegments();
            ChangeLightIntensity(TimeManager.Instance.CurrentTime);
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

                Transform segment = snakeBody.transform;
                segment.position = SnakeSegmentsList[snakeSegmentCount].position;
                SnakeSegmentsList.Add(segment);
                snakeSegmentCount++;
            }
        }

        private void OnEnable()
        {
            healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
            Food.OnFoodEaten += Food_OnFoodEaten;
            TimeManager.OnTimeChanged += TimeManager_OnTimeChanged;
        }

        private void OnDisable()
        {
            healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
            Food.OnFoodEaten -= Food_OnFoodEaten;
            TimeManager.OnTimeChanged -= TimeManager_OnTimeChanged;
        }

        private void Food_OnFoodEaten(Food food)
        {
            if (!IsSnakeColliding)
            {
                IsSnakeColliding = true;
                GrowSnake(food.foodSO.HealthIncrease);
                //IncreaseWeaponDamage(food.foodSO.DamageIncreasePercentage);
            }
        }

        private void TimeManager_OnTimeChanged(DayCicle dayCicle)
        {
            ChangeLightIntensity(dayCicle);
        }

        private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
        {
            if (healthEventArgs.healthAmount <= 0f)
            {
                destroyEvent.CallOnDestroy(true, 0);
                ResetSnakeSegments();
            }
        }

        //private void OnTriggerEnter2D(Collider2D other)
        //{
        //    if (other.TryGetComponent(out Food food) && other.CompareTag("Food") && !IsSnakeColliding)
        //    {
        //        IsSnakeColliding = true;
        //        GrowSnake(food.foodSO.HealthIncrease);
        //        IncreaseWeaponDamage(food.foodSO.DamageIncreasePercentage);
        //    }
        //}

        /// <summary>
        /// Initialises the snake
        /// </summary>
        /// <param name="snakeDetails">The snake details to initialize</param>
        public async UniTask Initialize(SnakeDetailsSO snakeDetails)
        {
            this.snakeDetails = snakeDetails;

            SnakeSegmentsList.Add(this.transform);
            snakeSegmentCount = 0;

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
            
            await materializeEffect.Materialize(snakeDetails.materializeShader,
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
            return snakeControler;
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
            for (int i = 0; i <= increaseHealth; i++)
            {
                SnakeBody snakeBody = (SnakeBody)PoolManager.Instance.ReuseComponent(GameResources.Instance.snakeBodyPrefab.gameObject,
                    SnakeSegmentsList[^1].position, Quaternion.identity);

                snakeBody.gameObject.SetActive(true);
                SnakeBodyList.Add(snakeBody);

                snakeBody.GetComponent<SpriteRenderer>().sortingOrder = -SnakeBodyList.Count;
                SnakeBodyList[snakeSegmentCount].WaitHeadUpdateCycle(SnakeBodyList.Count);

                Transform segmentTransform = snakeBody.transform;
                segmentTransform.position = SnakeSegmentsList[snakeSegmentCount].position;
                SnakeSegmentsList.Add(segmentTransform);
                snakeSegmentCount++;
            }
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
                    SnakeSegmentsList[index].GetComponent<SnakeBody>().SetTargetPosition(SnakeSegmentsList[index - 1].position);
                }
            }
        }

        public void TakeOneDamage()
        {
            health.TakeDamage(1);
        }

        public void SubstractSnakeSegment()
        {
            if (SnakeSegmentsList.Count > 0 && snakeSegmentCount > 0)
            {
                if (!health.IsDamageable) return;

                SnakeSegmentsList[snakeSegmentCount].TryGetComponent(out SnakeBody snakeBody);

                snakeBody.gameObject.SetActive(false);
                SnakeSegmentsList.RemoveAt(snakeSegmentCount);
                if (SnakeBodyList.Count > 0)
                    SnakeBodyList.Remove(snakeBody);

                snakeSegmentCount--;
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
            if (currentWeapon.weaponDetails.weaponCurrentAmmo.DecreaseDamage(20))
            {
                StartCoroutine(GameManager.Instance.ShowMessageRoutine($"Damage Decreased By {20}%.", 1.5f));
            }
            else
            {
                StartCoroutine(GameManager.Instance.ShowMessageRoutine($"Damage of this Weapon At Lowest Posible.", 1.5f));
            }
        }

        /// <summary>
        /// Increase the damage of the weapon when a food was eated.
        /// </summary>
        //private void IncreaseWeaponDamage(int percentageToincrease)
        //{
        //    Weapon currentWeapon = activeWeapon.GetCurrentWeapon();
        //    UniTask<bool> ds = currentWeapon.weaponDetails.weaponCurrentAmmo.IncreaseDamage(percentageToincrease);
        //    if (ds.)
        //    {
        //        StartCoroutine(GameManager.Instance.ShowMessage($"Damage of this Weapon Increased By {percentageToincrease}%", 1.5f));
        //    }
        //    else
        //    {
        //        StartCoroutine(GameManager.Instance.ShowMessage($"Max Damage with this Weapon Reached!", 1.5f));
        //    }
        //    IsSnakeColliding = false;
        //}

        private void EnableSnake(bool isActive)
        {
            snakeControler.IsSnakeEnabled = isActive;
            snakeControler.enabled = isActive;
            fireWeapon.enabled = isActive;
        }

        public void ChangeSpriteMaterial(Material material)
        {
            spriteRenderer.material = material;
        }

        public void ChangeLightIntensity(DayCicle time)
        {
            switch (time)
            {
                case DayCicle.Morning:
                    snakeLight.intensity = 0f;
                    break;
                case DayCicle.Afternoon:
                    snakeLight.intensity = 0.1f;
                    break;
                case DayCicle.Evening:
                    snakeLight.intensity = 0.3f;
                    break;
                case DayCicle.Night:
                    snakeLight.intensity = 0.6f;
                    break;
                default:
                    break;
            }
        }
    }
}