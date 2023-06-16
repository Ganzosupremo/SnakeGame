using SnakeGame.AbwehrSystem.Ammo;
using SnakeGame.GameUtilities;
using UnityEngine;

namespace SnakeGame.AbwehrSystem
{
    [RequireComponent(typeof(SetActiveWeaponEvent))]
    [DisallowMultipleComponent]
    public class ActiveWeapon : MonoBehaviour
    {
        #region Tooltip
        [Tooltip("Populate this with the Sprite Renderer on the child weapon gameobject")]
        #endregion
        [SerializeField] private SpriteRenderer weaponSpriteRenderer;

        #region Tooltip
        //[Tooltip("Populate this with the Polygon Collider on the child weapon gameobject")]
        #endregion
        //private PolygonCollider2D weaponPolygonCollider;

        #region Tooltip
        [Tooltip("Populate this with the Transform on the WeaponFirePosition gameobject")]
        #endregion
        [SerializeField] private Transform weaponShootPosition;

        #region Tooltip
        [Tooltip("Populate this with the Transform on the WeaponFirePosition gameobject")]
        #endregion
        [SerializeField] private Transform weaponEffectPosition;

        private SetActiveWeaponEvent activeWeaponEvent;
        private Weapon currentWeapon;

        private void Awake()
        {
            activeWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        }

        private void OnEnable()
        {
            activeWeaponEvent.OnSetActiveWeapon += ActiveWeaponEvent_OnSetActiveWeapon;
        }

        private void OnDisable()
        {
            activeWeaponEvent.OnSetActiveWeapon -= ActiveWeaponEvent_OnSetActiveWeapon;
        }

        private void ActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
        {
            SetWeapon(setActiveWeaponEventArgs.weapon);
        }

        private void SetWeapon(Weapon weapon)
        {
            currentWeapon = weapon;

            // In the final game the weapon won't have a sprite,
            // just in the UI, comment this code later
            //if (weaponSpriteRenderer != null)
            //    weaponSpriteRenderer.sprite = currentWeapon.weaponDetails.weaponSprite;

            //// Set the weapon polygon collider based on the shape the weapon sprite has
            //if (weaponPolygonCollider != null && weaponSpriteRenderer.sprite != null)
            //{
            //    //Get the sprite physics shape - this returns the sprite physics shape as a list of vectors
            //    List<Vector2> spritePhysicsShapePointsList = new List<Vector2>();
            //    weaponSpriteRenderer.sprite.GetPhysicsShape(0, spritePhysicsShapePointsList);

            //    //Set the polygon collider points based on the sprite physics shape points
            //    weaponPolygonCollider.points = spritePhysicsShapePointsList.ToArray();
            //}

            // Add some offset to the fire position
            Vector3 offset = currentWeapon.weaponDetails.weaponFirePosition;
            weaponShootPosition.localPosition = offset * 1.5f;
        }

        /// <summary>
        /// Gets The Ammo Corresponding To The Specific Weapon
        /// </summary>
        public BaseAmmoSO GetCurrentAmmo()
        {
            return currentWeapon.weaponDetails.weaponCurrentAmmo;
        }

        /// <summary>
        /// Gets The Current Weapon
        /// </summary>
        /// <returns>Returns the current weapon</returns>
        public Weapon GetCurrentWeapon()
        {
            return currentWeapon;
        }

        /// <summary>
        /// Gets The Current Weapon Fire Position
        /// </summary>
        /// <returns>Returns The Weapon Fire Position</returns>
        public Vector3 GetFirePosition()
        {
            if (gameObject.activeSelf)
                return weaponShootPosition.position;
            else
                return Vector3.zero;
        }

        /// <summary>
        /// Gets the Fire Effect Position Of The Current Weapon
        /// </summary>
        /// <returns>Returns The Fire Effect Of The Weapon</returns>
        public Vector3 GetWeaponFireEffectPosition()
        {
            if (gameObject.activeSelf)
                return weaponEffectPosition.position;
            else
                return  Vector3.zero;
        }

        /// <summary>
        /// Removes The Current Weapon
        /// </summary>
        public void RemoveCurrentWeapon()
        {
            currentWeapon = null;
        }

        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            //HelperUtilities.ValidateCheckNullValue(this, nameof(weaponSpriteRenderer), weaponSpriteRenderer);
            //HelperUtilities.ValidateCheckNullValue(this, nameof(weaponPolygonCollider), weaponPolygonCollider);
            HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootPosition), weaponShootPosition);
            HelperUtilities.ValidateCheckNullValue(this, nameof(weaponEffectPosition), weaponEffectPosition);
        }
#endif
        #endregion
    }
}

