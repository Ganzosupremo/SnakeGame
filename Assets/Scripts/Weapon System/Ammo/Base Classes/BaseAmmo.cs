using SnakeGame.AudioSystem;
using SnakeGame.GameUtilities;
using SnakeGame.HealthSystem;
using SnakeGame.Interfaces;
using SnakeGame.VisualEffects;
using UnityEngine;

namespace SnakeGame.AbwehrSystem.Ammo
{
    /// <summary>
    /// This is the base ammo class that all the different ammo classes can derive from.
    /// It handles the repetitive and necessary code for the different ammo types to work.
    /// </summary>
    public abstract class BaseAmmo : MonoBehaviour, IFireable
    {
        #region Tooltip
        [Tooltip("Populate with the child component TrailRenderer, that is found in the ammo prefab")]
        #endregion
        [SerializeField] protected TrailRenderer trailRenderer;

        [Tooltip("The max range of the ammo, once the ammo reaches this range, it'll be disabled.")]
        protected float _AmmoRange = 0;
        [Tooltip("The speed of the ammo. A random value is calculated between the min and max speed.")]
        protected float _AmmoSpeed;
        [Tooltip("The fire direction.")]
        protected Vector3 _AimDirectionVector;
        [Tooltip("The angle in which the ammo was fired.")]
        protected float _FireDirectionAngle;

        protected SpriteRenderer _SpriteRenderer;
        protected BaseAmmoSO _AmmoDetails;

        protected float _AmmoChargeTimer;
        protected bool _HasAmmoMaterialSet = false;
        protected bool _OverrideAmmoMovement;

        [Tooltip("As OnTriggerEnter2D can be called several times in a second, this is used " +
            "to avoid dealing double damage to the same entity.")]
        protected bool _IsColliding = false;

        protected virtual void Awake()
        {
            _SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Moves the ammo.
        /// Disables the ammo automatically if it has not hit a collision object and when it has reached it's max range.
        /// </summary>
        protected virtual void MoveAmmo()
        {
            if (_AmmoChargeTimer > 0)
            {
                _AmmoChargeTimer -= Time.deltaTime;
                return;
            }
            else if (!_HasAmmoMaterialSet)
            {
                SetAmmoMaterial(_AmmoDetails.ammoMaterial == null ? GameResources.Instance.litMaterial : _AmmoDetails.ammoMaterial);
                _HasAmmoMaterialSet = true;
            }

            //Don't move the ammo if the movement has been overriden, meaning this ammo is part of an ammo pattern
            if (!_OverrideAmmoMovement)
            {
                //Calculate distance vector to move the bullet
                Vector3 distanceVector = _AmmoSpeed * Time.deltaTime * _AimDirectionVector;

                transform.position += distanceVector;

                // Rotate the ammo
                transform.Rotate(new Vector3(0f, 0f, _AmmoDetails.ammoRotationSpeed * Time.deltaTime * 10f));

                //Disable after the max range has been reached
                _AmmoRange -= distanceVector.magnitude;

                if (_AmmoRange < 0)
                    DisableAmmo();
            }
        }

        /// <summary>
        /// This Method Initialises The Ammo So It Can Be Fired, Using The Specified Variables.
        /// If This Ammo Is Part Of An Ammo Pattern, The Ammo Movement Can Be Overriden By Setting The Bool overrideAmmoMovement To True
        /// </summary>
        public virtual void InitialiseAmmo(BaseAmmoSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
        {
            #region Ammo
            _AmmoDetails = ammoDetails;

            _IsColliding = false;

            //Sets the fire direction
            SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

            if (_SpriteRenderer != null)
                _SpriteRenderer.sprite = ammoDetails.ammoSprite;

            if (ammoDetails.ammoChargeTime > 0)
            {
                _AmmoChargeTimer = ammoDetails.ammoChargeTime;
                SetAmmoMaterial(ammoDetails.ammoChargeMaterial);
                _HasAmmoMaterialSet = false;
            }
            else
            {
                _AmmoChargeTimer = 0f;
                SetAmmoMaterial(ammoDetails.ammoMaterial);
                _HasAmmoMaterialSet = true;
            }

            // Set the ammo Range
            _AmmoRange = ammoDetails.ammoRange;
            // Set ammo speed
            this._AmmoSpeed = ammoSpeed;
            // Override the ammo movement
            this._OverrideAmmoMovement = overrideAmmoMovement;

            gameObject.SetActive(true);
            #endregion

            #region Ammo Trail
            if (ammoDetails.hasAmmoTrail)
            {
                trailRenderer.gameObject.SetActive(true);
                trailRenderer.emitting = true;
                trailRenderer.material = ammoDetails.ammoTrailMaterial;
                trailRenderer.startWidth = ammoDetails.ammoTrailStartWidth;
                trailRenderer.endWidth = ammoDetails.ammoTrailEndWidth;
                trailRenderer.time = ammoDetails.ammoTrailLifetime;
            }
            else
            {
                trailRenderer.emitting = false;
                trailRenderer.gameObject.SetActive(false);
            }
            #endregion
        }

        /// <summary>
        /// Set The Ammo Fire Direction Based On The Input Angle And Direction Adjusted By The Random Spread.
        /// </summary>
        protected virtual void SetFireDirection(BaseAmmoSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
        {
            float spreadRandomAngle = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

            // Get a random toggle between 1 or -1
            int spreadRandomToggle = Random.Range(0, 2) * 2 - 1;

            if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
                _FireDirectionAngle = aimAngle;
            else
                _FireDirectionAngle = weaponAimAngle;

            // Adjust the bullet fire angle with the random spread
            _FireDirectionAngle += spreadRandomToggle * spreadRandomAngle;

            //Set the bullet rotation if any
            transform.eulerAngles = new Vector3(0f, 0f, _FireDirectionAngle);

            //Set the bullet fire direction
            _AimDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(_FireDirectionAngle);
        }

        /// <summary>
        /// Deals damage on collision.
        /// Deals damage on a single entity.
        /// </summary>
        /// <param name="other"></param>
        protected virtual void DealDamage(Collider2D other)
        {
            if (!other.TryGetComponent(out Health health)) return;

            //bool enemyHit = false;
            _IsColliding = true;
            health.TakeDamage(_AmmoDetails.ammoDamage);

            if (health.enemy == null) return;
            if (health.enemy.enemyDetails.hitSoundEffect == null) return;
            SoundEffectManager.CallOnSoundEffectChangedEvent(health.enemy.enemyDetails.hitSoundEffect);
        }

        /// <summary>
        /// Deals damage on collision.
        /// Deals damage on more than one entity at the same time.
        /// </summary>
        /// <param name="others"></param>
        protected virtual void DealDamage(params Collider2D[] others)
        {
            foreach (Collider2D other in others)
            {
                _IsColliding = true;

                if (other.TryGetComponent(out Health health))
                {
                    // If the Health component is atached to an enemy
                    if (health.enemy != null)
                    {
                        health.TakeDamage(_AmmoDetails.ammoDamage);
                        if (health.enemy.enemyDetails.hitSoundEffect == null) return;
                        SoundEffectManager.CallOnSoundEffectChangedEvent(health.enemy.enemyDetails.hitSoundEffect);
                    }
                }
            }
        }

        /// <summary>
        /// Enables, if any, the ammo hit visual effects
        /// </summary>
        protected virtual void ActivateAmmoHitEffect()
        {
            if (_AmmoDetails == null) return;

            // Process if there is a hit effect & prefab
            if (_AmmoDetails.ammoHitEffect != null && _AmmoDetails.ammoHitEffect.ammoHitEffectPrefab != null)
            {
                // Get ammo hit effect gameobject from the pool with particle system component
                AmmoHitEffect hitEffect = (AmmoHitEffect)PoolManager.Instance.ReuseComponent
                    (_AmmoDetails.ammoHitEffect.ammoHitEffectPrefab, transform.position, Quaternion.identity);

                // Set hit effect
                hitEffect.InitialiseAmmoHitEffect(_AmmoDetails.ammoHitEffect);

                // Set gameobject active (the particle system is set to automatically disable the
                // gameobject once finished)
                hitEffect.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Enables, if any, the ammo hit visual effects
        /// </summary>
        protected virtual void ActivateAmmoHitEffect(Vector3 spawnPosition) { }

        protected virtual void PlayCollisionSoundEffect()
        {
            if (_AmmoDetails.CollisionSoundEffect != null)
                SoundEffectManager.CallOnSoundEffectChangedEvent(_AmmoDetails.CollisionSoundEffect);
        }

        protected virtual void DisableAmmo()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Sets the ammo material to the specified material
        /// </summary>
        /// <param name="ammoMaterial"></param>
        protected virtual void SetAmmoMaterial(Material ammoMaterial)
        {
            if (_SpriteRenderer != null)
                _SpriteRenderer.material = ammoMaterial;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        #region Validation
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValue(this, nameof(trailRenderer), trailRenderer);
        }
#endif
        #endregion
    }
}
