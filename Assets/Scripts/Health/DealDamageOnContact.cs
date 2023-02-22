using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DealDamageOnContact : MonoBehaviour
{
    #region Header DEAL EMOTIONAL DAMAGE
    [Header("DEAL EMOTIONAL DAMAGE!!!")]
    #endregion

    #region Tooltip
    [Tooltip("the damage to deal (can be overridden by the receiver)")]
    #endregion
    [SerializeField] private int dealDamageOnContact;

    #region Tooltip
    [Tooltip("Specify which layers should be affected by the touch damage")]
    #endregion
    [SerializeField] private LayerMask layerMask;
    private bool isColliding = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isColliding) return;

        DealDamage(collision);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isColliding) return;

        DealDamage(other);
    }

    private void DealDamage(Collider2D other)
    {
        // If the object isn't in the specified layer then return (we're using bitshift notation to compare)
        int collisionLayerMask = 1 << other.gameObject.layer;

        if ((layerMask.value & collisionLayerMask) == 0) return;

        
        if (other.gameObject.TryGetComponent(out ReceiveDamageOnContact receiveTouchDamage))
        {
            isColliding = true;

            // Reset the touch collision after the cooldown
            Invoke(nameof(ResetTouchCollider), Settings.touchDamagaCooldown);

            receiveTouchDamage.TakeDamageOnContact(dealDamageOnContact);
        }
    }

    private void ResetTouchCollider()
    {
        isColliding = false;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(dealDamageOnContact), dealDamageOnContact, true);
    }
#endif
    #endregion
}
