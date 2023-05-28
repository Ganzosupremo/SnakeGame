using SnakeGame.GameUtilities;
using UnityEngine;

namespace SnakeGame.AbwehrSystem.Ammo
{
    public class AmmoPattern : BaseAmmo
    {
        #region Tooltip
        [Tooltip("Populate the array with individual child ammo gameobjects")]
        #endregion
        [SerializeField] private BaseAmmo[] ammoArray;

        private float m_ammoRange;
        private float m_ammoSpeed;
        private Vector3 m_fireDirectionVector;
        private float m_fireDirectionAngle;
        private BaseAmmoSO m_ammoDetails;
        private float m_ammoChargeTimer;

        private void Update()
        {
            MoveAmmo();
        }

        protected override void MoveAmmo()
        {
            if (m_ammoChargeTimer > 0f)
            {
                m_ammoChargeTimer -= Time.deltaTime;
                return;
            }

            // Calculate the vector to move the bullet to
            Vector3 distanceVector = m_fireDirectionVector * m_ammoSpeed * Time.deltaTime;
            transform.position += distanceVector;

            //Rotate the ammo pattern
            transform.Rotate(new Vector3(0f, 0f, m_ammoDetails.ammoRotationSpeed * Time.deltaTime * 0.3f));

            // Disable the bullet after it has reached it's max range
            m_ammoRange -= distanceVector.magnitude;
            if (m_ammoRange < 0f)
            {
                DisableAmmo();
            }
        }

        public override void InitialiseAmmo(BaseAmmoSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
        {
            m_ammoDetails = ammoDetails;
            m_ammoSpeed = ammoSpeed;

            //Sets the fire direction of the ammo/bullet
            SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

            m_ammoRange = ammoDetails.ammoRange;

            gameObject.SetActive(true);

            //Loop through all ammo objects and initialise them
            foreach (BaseAmmo ammo in ammoArray)
            {
                ammo.InitialiseAmmo(ammoDetails, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector, true);
            }

            //Set the ammo charge timer, this will hold the ammo briefly before being fired
            if (ammoDetails.ammoChargeTime > 0f)
            {
                m_ammoChargeTimer = ammoDetails.ammoChargeTime;
            }
            else
            {
                m_ammoChargeTimer = 0f;
            }
        }

        /// <summary>
        /// Set The Ammo Fire Direction Based On The Direction Adjusted By The Random Spread
        /// </summary>
        protected override void SetFireDirection(BaseAmmoSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
        {
            float spreadRandomAngle = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

            // Get a random toggle between 1 or -1
            int spreadRandomToggle = Random.Range(0, 2) * 2 - 1;

            if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
            {
                m_fireDirectionAngle = aimAngle;
            }
            else
            {
                m_fireDirectionAngle = weaponAimAngle;
            }

            // Adjust the bullet fire angle with the random spread
            m_fireDirectionAngle += spreadRandomToggle * spreadRandomAngle;

            // Set the bullet fire direction
            m_fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(m_fireDirectionAngle);
        }

        #region Validation
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoArray), ammoArray);
        }
#endif
        #endregion
    }
}
