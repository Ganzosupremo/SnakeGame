using SnakeGame.AudioSystem;
using SnakeGame.HealthSystem;
using UnityEngine;

namespace SnakeGame.AbwehrSystem.Ammo
{
    [DisallowMultipleComponent]
    public class ExplosiveAmmo : BaseAmmo
    {
        private ExplosiveAmmoDetailsSO m_ExplosiveAmmoDetail;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            m_ExplosiveAmmoDetail = (ExplosiveAmmoDetailsSO)_AmmoDetails;
        }

        // Update is called once per frame
        void Update()
        {
            MoveAmmo();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_IsColliding) return;

            DealDamage(Physics2D.OverlapCircleAll(transform.position, m_ExplosiveAmmoDetail.ExplosionRadius, m_ExplosiveAmmoDetail.ExplosionMask));
            ActivatHitEffect();
            DisableAmmo();
            PlayCollisionSoundEffect();
        }

        //private void OnDrawGizmos()
        //{
        //    Gizmos.DrawWireSphere(transform.position, _ExplosionRadius);
        //}

        protected override void DealDamage(params Collider2D[] others)
        {
            foreach (var hit in others)
            {
                if (hit.TryGetComponent(out Health health))
                {
                    // If the Health component is atached to an enemy
                    if (health.enemy != null)
                    {
                        health.TakeDamage(m_ExplosiveAmmoDetail.ammoDamage);
                        if (health.enemy.enemyDetails.hitSoundEffect == null) return;
                        SoundEffectManager.CallOnSoundEffectSelectedEvent(health.enemy.enemyDetails.hitSoundEffect);
                    }
                    // If the health component is atached to the player
                    else
                    {
                        if (m_ExplosiveAmmoDetail.isPlayerAmmo)
                            health.TakeDamage(3);
                        else
                            health.TakeDamage(m_ExplosiveAmmoDetail.ammoDamage);
                    }
                }
            }
        }
    }
}
