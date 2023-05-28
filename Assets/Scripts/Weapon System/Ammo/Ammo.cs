using UnityEngine;

namespace SnakeGame.AbwehrSystem.Ammo
{
    [DisallowMultipleComponent]
    public class Ammo : BaseAmmo
    {
        protected override void Awake()
        {
            base.Awake();
        }

        private void Update()
        {
            MoveAmmo();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_IsColliding) return;

            DealDamage(other);

            ActivatHitEffect();

            DisableAmmo();

            if (!other.CompareTag(Settings.EnemyTag))
                PlayCollisionSoundEffect();
        }
    }
}