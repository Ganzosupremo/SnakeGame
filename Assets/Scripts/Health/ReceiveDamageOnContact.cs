using SnakeGame;
using SnakeGame.GameUtilities;
using UnityEngine;

[RequireComponent(typeof(Health))]
[DisallowMultipleComponent]
public class ReceiveDamageOnContact : MonoBehaviour
{
    #region Tooltip
    [Tooltip("The amount of damage to receive, this can override the damage receive" +
        " from the contact damage dealer")]
    #endregion
    [SerializeField] private int damageOnContact;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    public void TakeDamageOnContact(int damageAmount = 0)
    {
        if (damageOnContact > 0)
            damageAmount = damageOnContact;

        health.TakeDamage(damageAmount);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(damageOnContact), damageOnContact, true);
    }
#endif
    #endregion
}
