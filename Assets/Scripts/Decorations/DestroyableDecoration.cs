using SnakeGame.AudioSystem;
using SnakeGame.HealthSystem;
using System.Collections;
using UnityEngine;

namespace SnakeGame.Decorations
{
    [DisallowMultipleComponent]
    public class DestroyableDecoration : MonoBehaviour
    {
        #region Header Health
        [Header("Health")]
        [Space(10)]
        #endregion
        [SerializeField] private int startingHealthAmount = 1;

        #region Header Sound Effect
        [Header("Sound Effect")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("The sound effect that will be played when this object is destroyed")]
        #endregion
        [SerializeField] private SoundEffectSO destroySoundEffect;

        private Animator animator;
        private BoxCollider2D boxCollider2D;
        private HealthEvent healthEvent;
        private Health health;
        private ReceiveDamageOnContact receiveDamageOnContact;
        private MoveableDecoration moveableDecoration = null;

        [SerializeField] private bool isMoveable;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            boxCollider2D= GetComponent<BoxCollider2D>();
            healthEvent = GetComponent<HealthEvent>();
            health = GetComponent<Health>();
            health.SetStartingHealth(startingHealthAmount);
            receiveDamageOnContact = GetComponent<ReceiveDamageOnContact>();
            if (isMoveable) 
                moveableDecoration = GetComponent<MoveableDecoration>();
        }

        private void OnEnable()
        {
            healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
        }

        private void OnDisable()
        {
            healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
        }

        private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
        {
            if (healthEventArgs.healthAmount <= 0f)
            {
                StartCoroutine(PlayAnimation());
            }
        }

        private IEnumerator PlayAnimation()
        {
            // Destroy the box collider
            Destroy(boxCollider2D);

            if (destroySoundEffect != null)
                SoundEffectManager.CallOnSoundEffectSelectedEvent(destroySoundEffect);

            animator.SetBool(Settings.destroy, true);

            // Let the animation play through
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName(Settings.destroyedState))
            {
                yield return null;
            }

            // Then destroy all components, but the sprite renderer
            if (moveableDecoration != null)
                Destroy(moveableDecoration);

            Destroy(animator);
            Destroy(receiveDamageOnContact);
            Destroy(health);
            Destroy(healthEvent);
            Destroy(this);
        }
    }
}