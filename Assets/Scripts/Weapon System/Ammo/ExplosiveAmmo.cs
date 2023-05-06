using SnakeGame.AudioSystem;
using UnityEngine;

namespace SnakeGame.AbwehrSystem.Ammo
{
    [DisallowMultipleComponent]
    public class ExplosiveAmmo : BaseAmmo
    {
        #region Tooltip
        [Tooltip("The Radius this explosion will have.")]
        #endregion
        [SerializeField] private float _ExplosionRadius = 10f;
        #region Tooltip
        [Tooltip("Specify the layers this explosion will deal damage to.")]
        #endregion
        [SerializeField] private LayerMask _ExplosionMask;

        private ExplosiveAmmoDetailsSO m_ExplosiveAmmoDetail;
        private void Start()
        {
            m_ExplosiveAmmoDetail = (ExplosiveAmmoDetailsSO)ammoDetails;
        }

        // Update is called once per frame
        void Update()
        {
            MoveAmmo();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isColliding) return;

            Implode(Physics2D.OverlapCircleAll(transform.position, m_ExplosiveAmmoDetail.ExplosionRadius, m_ExplosiveAmmoDetail.ExplosionMask));

            ActivateAmmoEffects();

            DisableAmmo();
        }

        //private void OnDrawGizmos()
        //{
        //    Gizmos.DrawWireSphere(transform.position, _ExplosionRadius);
        //}

        private void Implode(params Collider2D[] others)
        {
            foreach (var hit in others)
            {
                if (hit.TryGetComponent(out Health health))
                {
                    // If the Health component is atached to an enemy
                    if (health.enemy != null)
                    {
                        health.TakeDamage(ammoDetails.ammoDamage);
                        if (health.enemy.enemyDetails.hitSoundEffect == null) return;
                        SoundEffectManager.CallOnSoundEffectSelectedEvent(health.enemy.enemyDetails.hitSoundEffect);
                    }
                    // If the health component is atached to the player
                    else
                    {
                        health.TakeDamage(3);
                    }
                }
            }
        }
    }
}
