using UnityEngine;
using SnakeGame.AbwehrSystem.Ammo;

namespace SnakeGame.Interfaces
{
    public interface IFireable
    {
        /// <summary>
        /// Initialises all the necessary details for the ammo to work
        /// </summary>
        /// <param name="ammoDetails"></param>
        /// <param name="aimAngle"></param>
        /// <param name="weaponAimAngle"></param>
        /// <param name="ammoSpeed"></param>
        /// <param name="weaponAimDirectionVector"></param>
        /// <param name="overrideAmmoMovement">If the current ammo needs to be reloaded before firing,
        /// for example with ammo patterns, the pattern first spawns in the game, then stays in place 
        /// for a brief moment and then goes to the fire direction</param>
        void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle,
            float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false);

        GameObject GetGameObject();
    }
}
