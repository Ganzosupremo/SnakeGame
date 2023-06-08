using System;
using SnakeGame;
using UnityEngine;

namespace SnakeGame.AbwehrSystem
{
    [DisallowMultipleComponent]
    public class FireWeaponEvent : MonoBehaviour
    {
        public event Action<FireWeaponEvent, FireWeaponEventArgs> OnFire;

        public void CallOnFireEvent(bool hasFired, bool firedPreviousFrame, AimDirection aimDirection,
            float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
        {
            OnFire?.Invoke(this, new FireWeaponEventArgs()
            {
                hasFired = hasFired,
                firedPreviousFrame = firedPreviousFrame,
                aimDirection = aimDirection,
                aimAngle = aimAngle,
                weaponAimAngle = weaponAimAngle,
                weaponAimDirectionVector = weaponAimDirectionVector
            });
        }
    }

    public class FireWeaponEventArgs : EventArgs
    {
        public bool hasFired;
        public bool firedPreviousFrame;
        public AimDirection aimDirection;
        public float aimAngle;
        public float weaponAimAngle;
        public Vector3 weaponAimDirectionVector;
    }
}