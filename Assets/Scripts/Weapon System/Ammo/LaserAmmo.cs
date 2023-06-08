using Cysharp.Threading.Tasks;
using SnakeGame.AudioSystem;
using SnakeGame.Debuging;
using SnakeGame.VisualEffects;
using System;
using System.Collections;
using UnityEngine;

namespace SnakeGame.AbwehrSystem.Ammo
{
    public class LaserAmmo : BaseAmmo
    {
        [SerializeField] private LayerMask _HitLayer;
        
        private LaserAmmoSO _LaserAmmoDetails;

        protected override void Awake()
        {
            _SpriteRenderer = null;
        }

        // TODO tomorrow
        // sound effects
        // add some effects on ammo collision
        // and add some effects when the weapon is recharging before firing the ammo

        //private void Update()
        //{
        //    MoveAmmo();
        //}

        public override void InitialiseAmmo(BaseAmmoSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
        {
            #region Laser Ammo
            _LaserAmmoDetails = (LaserAmmoSO)ammoDetails;
            _AmmoDetails = ammoDetails;

            _IsColliding = false;
            SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

            gameObject.SetActive(true);

            // Set the ammo Range
            _AmmoRange = ammoDetails.ammoRange;
            // Set ammo speed
            this._AmmoSpeed = ammoSpeed;
            // Override the ammo movement
            this._OverrideAmmoMovement = overrideAmmoMovement;

            #endregion

            Transform weaponRotationPointTransform = GameManager.Instance.GetSnake().aimWeapon.WeaponRotationPointTransform;
            #region Ammo Trail
            if (ammoDetails.hasAmmoTrail)
            {
                trailRenderer.gameObject.SetActive(true);
                trailRenderer.emitting = true;
                trailRenderer.transform.position = weaponRotationPointTransform.position;
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

            CastRaycast(weaponRotationPointTransform, weaponAimDirectionVector);
        }

        private async void CastRaycast(Transform weaponRotationPointTransform, Vector3 weaponAimDirection)
        {
            //RaycastHit2D hit = Physics2D.Raycast(weaponRotationPointTransform.position, weaponAimDirection, _AmmoRange, _HitLayer);
            //Array.Clear(raycasts, 0, 4);
            RaycastHit2D[] raycasts = Physics2D.RaycastAll(weaponRotationPointTransform.position, _AimDirectionVector, _AmmoRange, _LaserAmmoDetails.HitMask);

            foreach (RaycastHit2D raycast in raycasts)
            {
                if (raycast)
                    DealDamage(raycast.collider);

                //this.Log(raycast.collider);
                //this.Log(raycast);
                //Debug.DrawLine(weaponRotationPointTransform.position, raycast.point, Color.blue, 0.69f);
                await MoveAmmoTrail(raycast);
                ActivateAmmoHitEffect(raycast.point);
                PlayCollisionSoundEffect();
            }
            await UniTask.Delay(500);

            gameObject.SetActive(false);
        }

        private async UniTask MoveAmmoTrail(RaycastHit2D raycast)
        {
            float time = 0f;
            Vector3 spawnPosition = trailRenderer.transform.position;

            while (time < 1f)
            {
                trailRenderer.transform.position = Vector3.Lerp(spawnPosition, raycast.point, time);
                time += Time.deltaTime / trailRenderer.time;

                await UniTask.NextFrame();
            }
        }

        protected override void ActivateAmmoHitEffect(Vector3 spawnPosition)
        {
            if (_LaserAmmoDetails == null) return;

            // Proceed if there is a hit effect & prefab
            if (_LaserAmmoDetails.ammoHitEffect != null && _LaserAmmoDetails.ammoHitEffect.ammoHitEffectPrefab != null)
            {
                // Get ammo hit effect gameobject from the pool with particle system component
                AmmoHitEffect hitEffect = (AmmoHitEffect)PoolManager.Instance.ReuseComponent
                    (_LaserAmmoDetails.ammoHitEffect.ammoHitEffectPrefab, spawnPosition, Quaternion.identity);

                // Set hit effect
                hitEffect.SetAmmoHitEffect(_LaserAmmoDetails.ammoHitEffect, spawnPosition);

                // Set gameobject active (the particle system is set to automatically disable the
                // gameobject once finished)
                hitEffect.gameObject.SetActive(true);
            }
        }

        protected override void PlayCollisionSoundEffect()
        {
            if (_LaserAmmoDetails.CollisionSoundEffect != null)
                SoundEffectManager.CallOnSoundEffectSelectedEvent(_LaserAmmoDetails.CollisionSoundEffect);
        }
    }
}
