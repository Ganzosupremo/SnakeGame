using SnakeGame.AudioSystem;
using SnakeGame.GameUtilities;
using SnakeGame.Interfaces;
using UnityEngine;

namespace SnakeGame.AbwehrSystem.Ammo
{
    public class BaseAmmo : MonoBehaviour, IFireable
    {
        #region Tooltip
        [Tooltip("Populate with the child component TrailRenderer, that is found in the ammo prefab")]
        #endregion
        [SerializeField] private TrailRenderer trailRenderer;

        protected float ammoRange = 0;
        protected float ammoSpeed;
        protected Vector3 aimDirectionVector;
        protected float fireDirectionAngle;
        protected SpriteRenderer spriteRenderer;
        protected BaseAmmoSO ammoDetails;
        protected float ammoChargeTimer;
        protected bool hasAmmoMaterialSet = false;
        protected bool overrideAmmoMovement;
        protected bool isColliding = false;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            MoveAmmo();
        }

        public void MoveAmmo()
        {
            if (ammoChargeTimer > 0)
            {
                ammoChargeTimer -= Time.deltaTime;
                return;
            }
            else if (!hasAmmoMaterialSet)
            {
                SetAmmoMaterial(ammoDetails.ammoMaterial);
                hasAmmoMaterialSet = true;
            }

            //Don't move the ammo if the movement has been overriden, meaning this ammo is part of an ammo pattern
            if (!overrideAmmoMovement)
            {
                //Calculate distance vector to move the bullet
                Vector3 distanceVector = ammoSpeed * Time.deltaTime * aimDirectionVector;

                transform.position += distanceVector;

                //Disable after the max range has been reached
                ammoRange -= distanceVector.magnitude;

                if (ammoRange < 0)
                {
                    //if (ammoDetails.isPlayerAmmo)
                    //    //Call the multiply score event
                    //    StaticEventHandler.CallMultiplierEvent(false);

                    DisableAmmo();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isColliding) return;

            DealDamage(other);

            ActivateAmmoEffects();

            DisableAmmo();
        }

        /// <summary>
        /// This Method Initialises The Ammo So It Can Be Fired, Using The Specified Variables.
        /// If This Ammo Is Part Of An Ammo Pattern, The Ammo Movement Can Be Overriden By Setting The Bool overrideAmmoMovement To True
        /// </summary>
        public void InitialiseAmmo(BaseAmmoSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
        {
            #region Ammo
            this.ammoDetails = ammoDetails;

            isColliding = false;

            //Sets the fire direction
            SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

            spriteRenderer.sprite = ammoDetails.ammoSprite;

            if (ammoDetails.ammoChargeTime > 0)
            {
                ammoChargeTimer = ammoDetails.ammoChargeTime;
                SetAmmoMaterial(ammoDetails.ammoChargeMaterial);
                hasAmmoMaterialSet = false;
            }
            else
            {
                ammoChargeTimer = 0f;
                SetAmmoMaterial(ammoDetails.ammoMaterial);
                hasAmmoMaterialSet = true;
            }
            // Set the ammo Range
            ammoRange = ammoDetails.ammoRange;
            // Set ammo speed
            this.ammoSpeed = ammoSpeed;
            // Override the ammo movement
            this.overrideAmmoMovement = overrideAmmoMovement;

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
        private void SetFireDirection(BaseAmmoSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
        {
            float spreadRandomAngle = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

            //Get a random toggle between 1 or -1
            int spreadRandomToggle = Random.Range(0, 2) * 2 - 1;

            if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
            {
                fireDirectionAngle = aimAngle;
            }
            else
            {
                fireDirectionAngle = weaponAimAngle;
            }

            // Adjust the bullet fire angle with the random spread
            fireDirectionAngle += spreadRandomToggle * spreadRandomAngle;

            //Set the bullet rotation if any
            transform.eulerAngles = new Vector3(0f, 0f, fireDirectionAngle);

            //Set the bullet fire direction
            aimDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
        }

        public void DealDamage(Collider2D other)
        {
            //other.TryGetComponent(out Health health);

            if (!other.TryGetComponent(out Health health)) return;
            //bool enemyHit = false;
            isColliding = true;
            health.TakeDamage(ammoDetails.ammoDamage);

            //if (health.enemy != null)
            //    enemyHit = true;

            if (health.enemy == null) return;
            if (health.enemy.enemyDetails.hitSoundEffect == null) return;
            SoundEffectManager.CallOnSoundEffectSelectedEvent(health.enemy.enemyDetails.hitSoundEffect);

            //If is player ammo then update the multiplier in the UI
            //if (ammoDetails.isPlayerAmmo)
            //{
            //    if (enemyHit)
            //    {
            //        //Update the multiplier by 1
            //        StaticEventHandler.CallMultiplierEvent(true);
            //    }
            //    else
            //    {
            //        //Reduce the multiplier by 1
            //        StaticEventHandler.CallMultiplierEvent(false);
            //    }
            //}
        }
        public void ActivateAmmoEffects()
        {
            // Process if there is a hit effect & prefab
            if (ammoDetails.ammoHitEffect != null && ammoDetails.ammoHitEffect.ammoHitEffectPrefab != null)
            {
                // Get ammo hit effect gameobject from the pool with particle system component
                AmmoHitEffect hitEffect = (AmmoHitEffect)PoolManager.Instance.ReuseComponent
                    (ammoDetails.ammoHitEffect.ammoHitEffectPrefab, transform.position, Quaternion.identity);

                // Set hit effect
                hitEffect.SetAmmoHitEffect(ammoDetails.ammoHitEffect);

                // Set gameobject active (the particle system is set to automatically disable the
                // gameobject once finished)
                hitEffect.gameObject.SetActive(true);

                PlayCollisionSoundEffect();
            }
        }

        private void PlayCollisionSoundEffect()
        {
            if (ammoDetails.CollisionSoundEffect != null)
            {
                SoundEffectManager.CallOnSoundEffectSelectedEvent(ammoDetails.CollisionSoundEffect);
            }
        }

        public void DisableAmmo()
        {
            gameObject.SetActive(false);
        }

        private void SetAmmoMaterial(Material ammoMaterial)
        {
            spriteRenderer.material = ammoMaterial;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValue(this, nameof(trailRenderer), trailRenderer);
        }
#endif
        #endregion
    }
}
