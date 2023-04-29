using SnakeGame.AudioSystem;
using SnakeGame.Debuging;
using SnakeGame.GameUtilities;
using SnakeGame.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SnakeGame.AbwehrSystem.Ammo
{
    [DisallowMultipleComponent]
    public class ExplosiveAmmo : MonoBehaviour, IFireable
    {
        #region Tooltip
        [Tooltip("Populate with the child component TrailRenderer, that is found in the ammo prefab")]
        #endregion
        [SerializeField] private TrailRenderer _TrailRenderer;
        #region Tooltip
        [Tooltip("The Radius this explosion will have.")]
        #endregion
        [SerializeField] private float _ExplosionRadius = 10f;
        #region Tooltip
        [Tooltip("Specify the layers this explosion will deal damage to.")]
        #endregion
        [SerializeField] private LayerMask _ExplosionMask;
        
        private float m_AmmoRange = 0;
        private float m_AmmoSpeed;

        private Vector3 m_AimDirectionVector;
        private float m_FireDirectionAngle;
        private SpriteRenderer m_SpriteRenderer;
        private AmmoDetailsSO m_AmmoDetails;

        private float m_AmmoChargeTimer;
        private bool m_HasAmmoMaterialSet = false;
        private bool m_OverrideAmmoMovement;
        private bool m_IsColliding = false;

        // Start is called before the first frame update
        void Awake()
        {
           m_SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            MoveAmmo();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (m_IsColliding) return;

            Implode(Physics2D.OverlapCircleAll(transform.position, _ExplosionRadius, _ExplosionMask));

            ActivateAmmoEffects();

            DisableAmmo();
        }

        //private void OnDrawGizmos()
        //{
        //    Gizmos.DrawWireSphere(transform.position, _ExplosionRadius);
        //}

        private void MoveAmmo()
        {
            if (m_AmmoChargeTimer > 0)
            {
                m_AmmoChargeTimer -= Time.deltaTime;
                return;
            }
            else if (!m_HasAmmoMaterialSet)
            {
                SetAmmoMaterial(m_AmmoDetails.ammoMaterial);
                m_HasAmmoMaterialSet = true;
            }

            //Don't move the ammo if the movement has been overriden, meaning this ammo is part of an ammo pattern
            if (!m_OverrideAmmoMovement)
            {
                //Calculate distance vector to move the bullet
                Vector3 distanceVector = m_AmmoSpeed * Time.deltaTime * m_AimDirectionVector;

                transform.position += distanceVector;

                //Disable after the max range has been reached
                m_AmmoRange -= distanceVector.magnitude;

                if (m_AmmoRange < 0)
                {
                    //if (ammoDetails.isPlayerAmmo)
                    //    //Call the multiply score event
                    //    StaticEventHandler.CallMultiplierEvent(false);

                    DisableAmmo();
                }
            }
        }

        private void Implode(Collider2D[] others)
        {
            foreach (var hit in others)
            {
                m_IsColliding = true;
                //if (!hit.TryGetComponent(out Health health)) return;

                if (hit.TryGetComponent(out Health health))
                {
                    // If the Health component is atached to an enemy
                    if (health.enemy)
                    {
                        health.TakeDamage(m_AmmoDetails.ammoDamage);
                        if (health.enemy.enemyDetails.hitSoundEffect == null) return;
                        SoundEffectManager.CallOnSoundEffectSelectedEvent(health.enemy.enemyDetails.hitSoundEffect);
                    }
                    // If the health component is atached to the player
                    else
                    {
                        health.TakeDamage(3);
                    }
                }
                else
                    return;
            }
        }
        public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
        {
            #region Ammo
            this.m_AmmoDetails = ammoDetails;

            m_IsColliding = false;

            //Sets the fire direction
            SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

            m_SpriteRenderer.sprite = ammoDetails.ammoSprite;

            if (ammoDetails.ammoChargeTime > 0)
            {
                m_AmmoChargeTimer = ammoDetails.ammoChargeTime;
                SetAmmoMaterial(ammoDetails.ammoChargeMaterial);
                m_HasAmmoMaterialSet = false;
            }
            else
            {
                m_AmmoChargeTimer = 0f;
                SetAmmoMaterial(ammoDetails.ammoMaterial);
                m_HasAmmoMaterialSet = true;
            }
            // Set the ammo Range
            m_AmmoRange = ammoDetails.ammoRange;
            // Set ammo speed
            this.m_AmmoSpeed = ammoSpeed;
            // Override the ammo movement
            this.m_OverrideAmmoMovement = overrideAmmoMovement;

            gameObject.SetActive(true);
            #endregion

            #region Ammo Trail
            if (ammoDetails.hasAmmoTrail)
            {
                _TrailRenderer.gameObject.SetActive(true);
                _TrailRenderer.emitting = true;
                _TrailRenderer.material = ammoDetails.ammoTrailMaterial;
                _TrailRenderer.startWidth = ammoDetails.ammoTrailStartWidth;
                _TrailRenderer.endWidth = ammoDetails.ammoTrailEndWidth;
                _TrailRenderer.time = ammoDetails.ammoTrailLifetime;
            }
            else
            {
                _TrailRenderer.emitting = false;
                _TrailRenderer.gameObject.SetActive(false);
            }
            #endregion
        }

        private void ActivateAmmoEffects()
        {
            // Process if there is a hit effect & prefab
            if (m_AmmoDetails.ammoHitEffect != null && m_AmmoDetails.ammoHitEffect.ammoHitEffectPrefab != null)
            {
                // Get ammo hit effect gameobject from the pool with particle system component
                AmmoHitEffect hitEffect = (AmmoHitEffect)PoolManager.Instance.ReuseComponent
                    (m_AmmoDetails.ammoHitEffect.ammoHitEffectPrefab, transform.position, Quaternion.identity);

                // Set hit effect
                hitEffect.SetAmmoHitEffect(m_AmmoDetails.ammoHitEffect);

                // Set gameobject active (the particle system is set to automatically disable the
                // gameobject once finished)
                hitEffect.gameObject.SetActive(true);

                PlayCollisionSoundEffect();
            }
        }

        private void PlayCollisionSoundEffect()
        {
            if (m_AmmoDetails.CollisionSoundEffect != null)
                SoundEffectManager.CallOnSoundEffectSelectedEvent(m_AmmoDetails.CollisionSoundEffect);
        }

        private void SetAmmoMaterial(Material ammoMaterial)
        {
            m_SpriteRenderer.material = ammoMaterial;
        }

        /// <summary>
        /// Set The Ammo Fire Direction Based On The Input Angle And Direction Adjusted By The Random Spread.
        /// </summary>
        private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
        {
            float spreadRandomAngle = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

            //Get a random toggle between 1 or -1
            int spreadRandomToggle = Random.Range(0, 2) * 2 - 1;

            if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
            {
                m_FireDirectionAngle = aimAngle;
            }
            else
            {
                m_FireDirectionAngle = weaponAimAngle;
            }

            // Adjust the bullet fire angle with the random spread
            m_FireDirectionAngle += spreadRandomToggle * spreadRandomAngle;

            //Set the bullet rotation if any
            transform.eulerAngles = new Vector3(0f, 0f, m_FireDirectionAngle);

            //Set the bullet fire direction
            m_AimDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(m_FireDirectionAngle);
        }

        private void DisableAmmo()
        {
            gameObject.SetActive(false);
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}
